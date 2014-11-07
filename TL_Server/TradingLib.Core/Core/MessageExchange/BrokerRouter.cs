using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;

using TradingLib.Common;

using System.Threading;

namespace TradingLib.Core
{

    /* 关于无法平仓的死循环逻辑
     * 当前帐户持有仓位,同时下有反向委托，系统识别该委托为平仓
     * 该委托由于某些原因一直处于place submited状态 没有正常提交到broker
     * broker正常send的委托均处于open,partfilled等状态
     * 
     * A.客户端下市价平仓时【postflag为unknow自动识别】,br进行持仓计算，会将place submited的委托计入未成交平仓委托，broker会自动撤单然后当所有未成交平仓委托撤单后再提交最新的平仓委托
     * 而由于place submited委托不在broker管理访问，导致用于无法撤出，同时最新待提交的为委托处于place状态一直没有正式通过broker.send进行发送，而且每提交一次委托 未成交队列中就增加一条委托记录
     * 
     * B.客户端下市价平仓时【postflag为close强制】，br进行持仓计算，会将place submited的委托计入未成交平仓委托，导致返回可平持仓数量不足。
     * 
     * 因此在msgexch中有单独的定时程序进行超时 place submit状态的委托进行处理，防止帐户陷入死循环状态。
     * 
     * 理论上place submit的委托是不会长时间存在的，正常工作状态下这些状态只会存在很短的时间。
     * 
     * 
     * 
     * 
     * 
     * 
     * */
    /// <summary>
    /// 成交路由中心
    /// 用于承接系统的交易,提交委托,取消委托等
    /// 1.交易路由中心,将进来的委托按照一定规则找到对应的交易接口并通过交易接口进行对应的交易操作 
    /// 2.交易路由收到交易接口返回的回报时,将这些回报转发给TradingServer
    /// 3.交易路由内置组合委托中心,用于提供交易所所不支持的委托类型
    /// 4.交易路由中心内置委托智能处理器,该处理器用于处理自动撤单(市价平仓),反手等事务
    /// 
    /// 5.交易路由中心操作接口是线程安全的,内置2个队列一个队列用于处理交易系统提交上来的委托以及其他操作,排队处理
    ///   另一个队列处理成交接口返回过来的回报,将这些回报逐一返回给交易系统
    /// </summary>
    public class BrokerRouter:BaseSrvObject
    {
        public const string PROGRAM = "BrokerRouter";

        /// <summary>
        /// 获得交易所对应的交易通道 
        /// </summary>
        public event LookupBroker LookupBrokerEvent;
        IBroker lookupBroker(string exchange)
        {
            if (LookupBrokerEvent != null)
                return LookupBrokerEvent(exchange);
            return null;
        }

        /// <summary>
        /// 获得模拟成交Broker
        /// </summary>
        /// <returns></returns>
        IBroker GetSimBroker()
        {
            return TLCtxHelper.Ctx.RouterManager.DefaultSimBroker;
        }

        TIFEngine _tifengine;
        private ClearCentre _clearCentre;
        private DataFeedRouter _datafeedRouter;

        OrderTransactionHelper _ordHelper;

        /// <summary>
        /// 数据路由,用于让交易接口得到需要的tick数据等信息
        /// </summary>
        public DataFeedRouter DataFeedRouter { get { return _datafeedRouter; } 
            set { 
                _datafeedRouter = value;
                //_datafeedRouter.GotTickEvent += new TickDelegate(_tifengine.GotTick);
                
                   } }
        public BrokerRouter(ClearCentre c):base("BrokerRouter")
        {
            _clearCentre = c;
            _tifengine = new TIFEngine();
            //_tifengine.SendDebugEvent +=new DebugDelegate(msgdebug);
            _tifengine.SendOrderEvent += new OrderDelegate(route_SendOrder);
            _tifengine.SendCancelEvent += new LongDelegate(route_CancelOrder);
            

            _ordHelper = new OrderTransactionHelper("BrokerRouter");
            //_ordHelper.SendDebugEvent +=new DebugDelegate(msgdebug);
            _ordHelper.SendOrderEvent += new OrderDelegate(broker_sendorder);
           

            StartProcessMsgOut();
        }

        #region 通过Order选择对应的broker交易通道
        //通过Order或者orderId选择对应的Broker
        /// <summary>
        /// 查找委托对应的交易通道
        /// 这里可以复杂化加入负责的路由只能
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public IBroker SelectBroker(Order o)
        {
            
            //如果是模拟交易则通过模拟broker发送信息
            if (_clearCentre[o.Account].OrderRouteType == QSEnumOrderTransferType.SIM)
                return GetSimBroker();
            else
            {
                string ex = o.oSymbol.SecurityFamily.Exchange.Index;//_clearCentre.getMasterSecurity(o.symbol).DestEx;//在清算中心通过symbol找到对应的品种，然后就可以得到其交易所代码
                return lookupBroker(ex);
            }
        }

        public IBroker SelectBroker(long oid)
        {
            //如果是模拟交易则通过模拟broker发送信息
            Order o = _clearCentre.SentOrder(oid);//通过orderID在清算中心找到对应的Order
            return SelectBroker(o);
        }

        #endregion 

        //加载某个交易通道
        /// <summary>
        /// 将某个Broker加载到系统
        /// </summary>
        /// <param name="broker"></param>
        public void LoadBroker(IBroker broker)
        { 
            //将Broker的事件触发绑定到本地函数回调
            broker.GotOrderEvent +=new OrderDelegate(GotOrder);
            broker.GotCancelEvent +=new LongDelegate(GotCancel);
            broker.GotFillEvent +=new FillDelegate(GotFill);

            broker.GotOrderErrorEvent +=new OrderErrorDelegate(GotOrderError);
            //获得某个symbol的tick数据
            broker.GetSymbolTickEvent += new GetSymbolTickDel(broker_GetSymbolTickEvent);
            //数据路由中Tick事件驱动交易通道中由Tick部分
            DataFeedRouter.GotTickEvent += new TickDelegate(broker.GotTick);
            //将清算中心绑定到交易通道
            broker.ClearCentre = new ClearCentreAdapterToBroker(_clearCentre);
            
        }

        

        

        #region Broker向本地回报操作
        /// <summary>
        /// 交易接口查询某个symbol的当前最新Tick快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Tick broker_GetSymbolTickEvent(string symbol)
        {
            try
            {
                return DataFeedRouter.TickTracker[symbol];
            }
            catch (Exception ex)
            {
                debug(PROGRAM + ":get symbol tick snapshot error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return null;
            }
        }

        public event OrderErrorDelegate GotOrderErrorEvent;
        /// <summary>
        /// 当交易通道有Order错误信息时,进行处理
        /// </summary>
        void GotOrderError(Order order, RspInfo error)
        {
            debug("Reply ErrorOrder To MessageExch:" + order.GetOrderInfo() + " ErrorTitle:" + error.ErrorMessage, QSEnumDebugLevel.INFO);
            _errorordernotifycache.Write(new OrderErrorPack(order,error));
        }

        /// <summary>
        /// 发送内部产生的委托错误
        /// </summary>
        /// <param name="o"></param>
        /// <param name="errortitle"></param>
        void GotOrderErrorNotify(Order o, string errortitle)
        {
            RspInfo info = RspInfoImpl.Fill(errortitle);
            o.comment = info.ErrorMessage;
            GotOrderError(o, info);

            //debug("Reply ErrorOrder To MessageExch:" + o.ToString() + " ErrorTitle:" + errortitle, QSEnumDebugLevel.INFO);
            //ErrorOrderNotify notify = ResponseTemplate<ErrorOrderNotify>.SrvSendNotifyResponse(o.Account);
            //notify.Order = new OrderImpl(o);
            //notify.RspInfo.Fill(errortitle);
            //notify.Order.comment = notify.RspInfo.ErrorMessage;
            //_errorordernotifycache.Write(notify);
        }

        /// <summary>
        /// 当有成交时候回报msgexch
        /// </summary>
        public event FillDelegate GotFillEvent;
        void GotFill(Trade fill)
        {
            if (fill != null && fill.isValid)
            {
                debug("Reply Fill To MessageExch:" + fill.GetTradeInfo(), QSEnumDebugLevel.INFO);
                _fillcache.Write(new TradeImpl(fill));
            }
        }

        /// <summary>
        /// 委托正确回报时回报msgexch
        /// </summary>
        public event OrderDelegate GotOrderEvent;
        void GotOrder(Order o)
        {
            if (o != null && o.isValid)
            {
                debug("Reply Order To MessageExch:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
                _ordercache.Write(new OrderImpl(o));
            }
        }
        /// <summary>
        /// 撤单正确回报时回报msgexch
        /// </summary>
        public event LongDelegate GotCancelEvent;
        void GotCancel(long oid)
        {
            debug("Reply Cancel To MessageExch:"+oid.ToString());
            _cancelcache.Write(oid);
        }

        #endregion 

        /// <summary>
        /// ansyaserver->tlserver_mq->tradingserver->Brokerrouter-X>Broker(order/cancel/trade/)
        /// ansyaserver->tlserver_mq->tradingserver->Brokerrouter->ordermessage
        /// ordermessage和simbroker原先的问题一样,ansycserver的消息一直通过处理brokerrouter,brokerrouter如果又直接返回消息
        /// 进入tradingserver会形成消息闭路，我们有必要将消息进行缓存中断
        /// </summary>
        #region 对外转发交易信息
        const int buffersize = 1000;
        RingBuffer<Order> _ordercache = new RingBuffer<Order>(buffersize);
        RingBuffer<long> _cancelcache = new RingBuffer<long>(buffersize);
        RingBuffer<Trade> _fillcache = new RingBuffer<Trade>(buffersize);//成交缓存
        RingBuffer<OrderErrorPack> _errorordernotifycache = new RingBuffer<OrderErrorPack>(buffersize);//委托消息缓存

        Thread msgoutthread = null;
        bool msgoutgo = false;
        void msgoutprocess()
        {
            while (msgoutgo)
            {
                try
                {
                    //转发委托
                    while (_ordercache.hasItems)
                    {
                        if (GotOrderEvent != null)
                            GotOrderEvent(_ordercache.Read());
                    }
                    //转发成交
                    while (_fillcache.hasItems & !_ordercache.hasItems)
                    {
                        Trade fill = _fillcache.Read();
                        _tifengine.GotFill(fill);
                        if (GotFillEvent != null)
                            GotFillEvent(fill);
                    }
                    //转发取消
                    while (_cancelcache.hasItems & !_ordercache.hasItems & !_fillcache.hasItems)
                    {
                        long oid = _cancelcache.Read();
                        _tifengine.GotCancel(oid);
                        _ordHelper.GotCancel(oid);//发单辅助引擎得到委托
                        if (GotCancelEvent != null)
                            GotCancelEvent(oid);
                    }
                    //转发委托错误
                    while (_errorordernotifycache.hasItems & !_cancelcache.hasItems & !_ordercache.hasItems & !_fillcache.hasItems)
                    {
                        OrderErrorPack error = _errorordernotifycache.Read();
                        if (GotOrderErrorEvent != null)
                            GotOrderErrorEvent(error.Order, error.RspInfo);
                    }
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    debug(PROGRAM + ":process message out error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
        }
        void StartProcessMsgOut()
        {
            if(msgoutgo) return;
            msgoutgo = true;
            msgoutthread = new Thread(msgoutprocess);
            msgoutthread.IsBackground = true;
            msgoutthread.Name = "BrokerRouter MessageOut Thread";
            msgoutthread.Start();
            ThreadTracker.Register(msgoutthread);

        }

        void StopProcessMsgOut()
        {
            if (!msgoutgo) return;
            ThreadTracker.Unregister(msgoutthread);
            msgoutgo = false;
            int mainwait = 0;
            while (msgoutthread.IsAlive && mainwait < 10)
            {
                Thread.Sleep(1000);
                mainwait++;
            }
            msgoutthread.Abort();
            msgoutthread = null;
        }
        #endregion


        #region 本地向Broker发情的交易请求
        /// <summary>
        /// 向Broker发送Order,TradingServer统一通过 BrokerRouter 发送委托,BrokerRouter 则在本地按一定的规则找到对应的
        /// 交易接口将委托发送出去
        /// 
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order o)
        {
            try
            {
                
                //检查通过,则通过该broker发送委托 拒绝的委托通过 ordermessage对外发送
                if (o.Status != QSEnumOrderStatus.Reject)
                {
                    //按照委托方式发送委托直接发送或通过本地委托模拟器进行发送
                    debug("Send  Order To Broker Side:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
                    broker_sendorder(o);
                    //调用broker_sendorder对外发送委托 如果委托状态为拒绝 则表明委托被broker_send部分拒绝了，我们不用再标记委托状态为submited 因此不用再进入下一步
                    if (o.Status != QSEnumOrderStatus.Reject)
                    {
                        o.Status = QSEnumOrderStatus.Submited;//委托状态修正为 已经提交到Broker
                        GotOrder(o);
                    }
                }
            }
            catch (Exception ex)
            {
                debug("BrokerRouter Send Order Error:" + (o==null ?"Null":o.ToString()),QSEnumDebugLevel.ERROR);
                debug(ex.ToString());
            }
        }


        void broker_sendorder(Order o)
        {
            //检查TIF设定，然后根据需要是否要通过TIFEngine来处理Order
            switch (o.TIF)
            {
                case "DAY":
                    route_SendOrder(o);
                    break;
                default:
                    _tifengine.SendOrder(o);//问题1:相关交易通道没有开启但是我们已经将委托交由TIFEngine管理，会产生屡次取消或者取消错误
                    break;
            }
        }

        void route_SendOrder(Order o)
        {
            try
            {
                IBroker broker = SelectBroker(o);
                if (broker != null && broker.IsLive)
                {
                    broker.SendOrder(o);
                }
                else
                {
                    //如果没有交易通道则拒绝该委托
                    o.Status = QSEnumOrderStatus.Reject;
                    GotOrderErrorNotify(o, "EXECUTION_BROKER_NOT_FOUND");
                    debug("没有可以交易的通道 |" + o.ToString(), QSEnumDebugLevel.WARNING);
                }
            }
            catch (Exception ex)
            {
                debug(PROGRAM + ":向broker发送委托错误:" + ex.ToString() + ex.StackTrace.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        /// <summary>
        /// 向broker取消一个order
        /// </summary>
        /// <param name="oid"></param>
        public void CancelOrder(long val)
        {
            try
            {
                debug(PROGRAM + ":Route Cancel to Broker Side:" + val.ToString(), QSEnumDebugLevel.INFO);
                route_CancelOrder(val);
            }
            catch (Exception ex)
            {
                debug("BrokerRouter CancelOrder Error:" + val.ToString(), QSEnumDebugLevel.ERROR);
                debug(ex.ToString());

            }
        
        }
        //通过路由选择器将委托取消发送出去
        void route_CancelOrder(long val)
        {
            try
            {
                //debug("取消委托到这里...", QSEnumDebugLevel.MUST);
                Order o = _clearCentre.SentOrder(val);//通过orderID在清算中心找到对应的Order
                IBroker broker = SelectBroker(o);
                if (broker != null && broker.IsLive)
                {
                    //debug("取消委托到这里...broker.cancelorder", QSEnumDebugLevel.MUST);
                    broker.CancelOrder(o.id);
                }
                else
                {
                    //如果没有交易通道则拒绝该委托
                    o.Status = QSEnumOrderStatus.Reject;
                    GotOrderErrorNotify(o, "EXECUTION_BROKER_NOT_FOUND");
                    debug(PROGRAM + ":没有可以交易的通道 |" + o.ToString(), QSEnumDebugLevel.WARNING);
                }
            }
            catch (Exception ex)
            {
                debug(PROGRAM + ":向broker发送取消委托出错:" + ex.ToString() + ex.StackTrace.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        #endregion

        public void Reset()
        {
            _ordHelper.Clear();
            _tifengine.Clear();
            //重启模拟交易
            IBroker b = GetSimBroker();
            if (b == null) return;
            b.Stop();
            Thread.Sleep(1000);
            b.Start();

        }

        public void Start()
        {
            StartProcessMsgOut();
            _ordHelper.Start();
            _tifengine.Start();

        }

        public void Stop()
        {

            _ordHelper.Stop();
            _tifengine.Stop();
            StopProcessMsgOut();
        }

        public override void Dispose()
        {
            base.Dispose();

        }
    }
}
