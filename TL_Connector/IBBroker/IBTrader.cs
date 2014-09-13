using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using TradingLib.Core;
using TradingLib.API;
using Krs.Ats.IBNet;
using Krs.Ats.IBNet.Contracts;


namespace Broker.IB
{
    public class IBTrader:IBroker
    {
        public string Title
        {
            get { return "IB盈透交易通道"; }
        }

        public void Show(object panel)
        {
            _moniter.ShowInPanel(panel);
        }
        //该Broker支持的交易所
        IExchange[] _exlist = null;
        public IExchange[] ExchangeSupported { get { return _exlist; } }
        //该broker支持的证券类型
        //SecurityType[] _sectypelist = null;
        //public SecurityType[] SecuritySupported { get { return _sectypelist; } }

        string _srvIP = "127.0.0.1";
        int _srvPort = 4001;
        int _clientID = 11;

        IBHelper _ibhelper = new IBHelper();

        public event IConnecterParamDel Connected;
        public event IConnecterParamDel Disconnected;

        /// <summary>
        /// 查询某个Symbol对应的Ticksnapshot用于交易通道使用
        /// </summary>
        public event GetSymbolTickDel GetSymbolTickEvent;
        Tick GetSymbolTick(string symbol)
        {
            if (GetSymbolTickEvent != null)
                return GetSymbolTickEvent(symbol);
            return null;
        }
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        /// <summary>
        /// 当CTP接口有成交回报时,我们通知客户端
        /// </summary>
        public event FillDelegate GotFillEvent;
        void GotFill(Trade f)
        {
            if (GotFillEvent != null)
                GotFillEvent(f);
        }
        /// <summary>
        /// 当CTP接口有委托回报时,我们通知客户端
        /// </summary>
        public event OrderDelegate GotOrderEvent;
        void GotOrder(TradeLink.API.Order o)
        {
            if (GotOrderEvent != null)
                GotOrderEvent(o);
        }
        /// <summary>
        /// 当CTP接口有取消交易回报时,我们通知客户端
        /// </summary>
        public event LongDelegate GotCancelEvent;
        void GotCancel(long oid)
        {
            if (GotCancelEvent != null)
                GotCancelEvent(oid);
        }
        public event OrderMessageDel GotOrderMessageEvent;
        void GotOrderMessage(TradeLink.API.Order o, string msg)
        {
            if (GotOrderMessageEvent != null)
                GotOrderMessageEvent(o, msg);
        }

        private IClearCentre _clearCentre;
        public IClearCentre ClearCentre
        {
            get
            {
                return _clearCentre;
            }
            set
            {
                _clearCentre = value;
            }
        }

        fmAccountMoniter _moniter = new fmAccountMoniter();
        OrderActionTransaction _orderActionTrans = new OrderActionTransaction();
        public IBTrader()
        {
            
            _orderActionTrans.SendDebugEvent +=new DebugDelegate(debug);
            _orderActionTrans.GotOrderEvent += new OrderDelegate(SendOrder);
        }
        IBClient client;
        bool _connected=false;
        public void Start()
        {
            if (!_connected)
            {
                client = new IBClient();
                client.ThrowExceptions = true;
                bindEvent();
                debug("Connecting to IB.");
                client.Connect(_srvIP, _srvPort, _clientID);
                _orderActionTrans.Start();
                //demoRegisterSymbol();
            }
            
        }

        void bindEvent()
        {
            client.Error += client_Error;
            client.NextValidId += client_NextValidId;
            //client.UpdateMarketDepth += client_UpdateMktDepth;
            //client.RealTimeBar += client_RealTimeBar;
            client.OrderStatus += client_OrderStatus;
            client.ExecDetails += client_ExecDetails;
            
        }

        int _orderId = 0;
        int NextOrderId {
            get
            {
                int id = _orderId;
                _orderId++;
                return id;
            }
        }
        void client_NextValidId(object sender, NextValidIdEventArgs e)
        {
            debug("交易通道连接成功! Next Valid Id: " + e.OrderId);
            //NextOrderId = e.OrderId;
            _orderId = e.OrderId;
            _connected = true;
            if (Connected != null)
                Connected(this);
        }

        void client_ExecDetails(object sender, ExecDetailsEventArgs e)
        {
            string s = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                e.Contract.Symbol, e.Execution.AccountNumber, e.Execution.ClientId, e.Execution.Exchange, e.Execution.ExecutionId,
                e.Execution.Liquidation, e.Execution.OrderId, e.Execution.PermId, e.Execution.Price, e.Execution.Shares, e.Execution.Side, e.Execution.Time);
            debug("成交回报:"+s);

            TradeLink.API.Order o = getOrderViaIBOrderRef(e.OrderId);
            Trade f;
            if (o != null)
            {
                //反向生成fill正确传达给相应的操盘终端
                decimal xprice = (decimal)e.Execution.Price;//成交价格
                int xsize = e.Execution.Shares;//成交数量
                string xtime = e.Execution.Time;
                //DateTime dtime = DateTime.ParseExact(xtime, "yyyyMMdd HH:mm:ss", new System.Globalization.CultureInfo("zh-CN", true));
                DateTime dtime = DateTime.Now;

                xsize = (e.Execution.Side == ExecutionSide.Bought ? 1 : -1) * xsize;//local fill数量是带方向的
                f = (Trade)(new OrderImpl(o));
                f.xsize = xsize;
                f.xprice = xprice;
                f.xdate = Util.ToTLDate(dtime);
                f.xtime = Util.ToTLTime(dtime);
                f.Broker = this.GetType().FullName;
                f.BrokerKey = e.Execution.PermId.ToString();//成交唯一编号
                debug("new fill:" + f.ToString());

                //如果Order没有回报给客户端即回报了成交,则我们需要自己判断,然偶先返回委托回报再返回成交回报
                if (!isOrderAck(e.OrderId))
                {
                    GotOrder(o);
                    AckOrder(e.OrderId);
                }
                GotOrderMessage(o, TradingLib.API.OrderStatus.Accepted + "|正在成交");
                GotFill(f);
                //GotOrderMessage(o,e.Execution.Side==ExecutionSide.Bought?"买入":"卖出" +" "+f.symbol+ f.UnsignedSize.ToString()+"手");

            }

        }


        void client_OrderStatus(object sender, OrderStatusEventArgs e)
        {
            int ordId = e.OrderId;
            Krs.Ats.IBNet.OrderStatus ordStatus = e.Status;
            int unfilled = e.Remaining;
            int filled = e.Filled;
            int clientId = e.ClientId;
            int permId = e.PermId;
            debug("ordId:"+ordId.ToString() +" OrderStatus:" + ordStatus.ToString() +" Unfilled:" + unfilled.ToString() +" Filled:" +filled.ToString() + " ClientID"+clientId.ToString()+" permId:"+permId.ToString());
            //根据返回的委托状态我们来进行相关数据处理
            TradeLink.API.Order o = getOrderViaIBOrderRef(ordId);
            if (o == null) return;
            
            switch (ordStatus)
            { 
                //IB报单逻辑会产生先发送成交然后再发送委托状态
                //或者委托状态直接为已经成交没有经历submited的中间状态等
                case Krs.Ats.IBNet.OrderStatus.Submitted://委托被提交等待成交
                    if (isOrderAck(ordId)) return;
                    GotOrder(o);
                    AckOrder(ordId);
                    GotOrderMessage(o,TradingLib.API.OrderStatus.Accepted+"|等待成交");
                    break;
                case Krs.Ats.IBNet.OrderStatus.Filled:
                    if (isOrderAck(ordId)) return;
                    GotOrder(o);
                    AckOrder(ordId);
                    if(unfilled >0)
                        GotOrderMessage(o,"部分成交");
                    else
                        GotOrderMessage(o,"全部成交");
                    break;

                case Krs.Ats.IBNet.OrderStatus.Canceled://委托被取消
                    _orderActionTrans.GotCancel(o.id);
                    GotCancel(o.id);
                    GotOrderMessage(o, "委托被取消");
                    break;
                case Krs.Ats.IBNet.OrderStatus.Inactive:
                    GotOrderMessage(o, "委托被拒绝");
                    break;


                
            }
        }

        void client_Error(object sender, ErrorEventArgs e)
        {
            //debug("Error ID"+.ErrorCode.ToString()+"Error: " + e.ErrorMsg);
            string errorID = e.ErrorCode.ToString();
            switch (errorID)
            {
                case "2104":
                    //onMktDataConnectOK();
                    break;
                case "201"://委托被拒绝
                    break;
                default:
                    debug("tickID:" + e.TickerId +" Error ID" + e.ErrorCode.ToString() + " Error: " + e.ErrorMsg);
                    break;

            }
        }

        public void Stop()
        {
            debug("关闭IB交易通道");
            if (_connected)
            {
                _connected = false;
                //client.Stop();
                client.Disconnect();
                _orderActionTrans.Stop();
                if (Disconnected != null)
                    Disconnected(this);

            }
        }

        //记录IB本地OrderRef与Order的对应
        Dictionary<int, TradeLink.API.Order> ibOrderId2Order = new Dictionary<int, TradeLink.API.Order>();
        Dictionary<long, int> LocalOrderID2IBOrdRef = new Dictionary<long, int>();
        //记录IB本地OrderRef是否已经回报给客户端若没有回报则回报
        Dictionary<int, bool> ibOrderIdAck = new Dictionary<int, bool>();
        /// <summary>
        /// 记录Order与之对应的OrderId
        /// </summary>
        /// <param name="o"></param>
        /// <param name="id"></param>
        void BookOrderAndID(TradeLink.API.Order o,int id)
        {
            if (id == -1) return;

            if (ibOrderId2Order.ContainsKey(id))
                ibOrderId2Order[id] = o;
            else
                ibOrderId2Order.Add(id, o);

            if (LocalOrderID2IBOrdRef.ContainsKey(o.id))
                LocalOrderID2IBOrdRef[o.id] = id;
            else
                LocalOrderID2IBOrdRef.Add(o.id, id);

            if (ibOrderIdAck.ContainsKey(id))
                ibOrderIdAck[id] = false;
            else
                ibOrderIdAck.Add(id, false);
        }
        /// <summary>
        /// 通过IB本地OrderID来索引到本地的Order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TradeLink.API.Order  getOrderViaIBOrderRef(int id)
        {
            TradeLink.API.Order o = null;
            if (ibOrderId2Order.TryGetValue(id, out o))
                return o;
            return o;
        }

        TradeLink.API.Order getOrderViaLocalOrdId(long id)
        {
            int ibid = getIBOrdRefViaLocalOrdId(id);
            if (ibid > 0)
                return getOrderViaIBOrderRef(ibid);
            return null;
        }

        int getIBOrdRefViaLocalOrdId(long id)
        {
            if (LocalOrderID2IBOrdRef.ContainsKey(id))
                return LocalOrderID2IBOrdRef[id];
            else
                return -1;
        }

        bool isOrderAck(int id)
        {
            if (ibOrderIdAck.ContainsKey(id))
                return ibOrderIdAck[id];
            else
                return false;
        }

        void AckOrder(int id)
        {
            ibOrderIdAck[id] = true;
        }
        public void SendOrder(TradeLink.API.Order o)
        {
            debug("IBTrader Got Order From QSTrading Side:" + o.ToString());
            Position pos = _clearCentre.getPosition(o);//获得当前Account以及sybol所对应的持仓，用于检查委托时开仓还是平仓,同时判断仓位数量以及未成交合约数量
            int ufill_size = _clearCentre.getUnfilledSizeExceptStop(o);

            if (pos.isFlat)
            {
                debug("空仓:直接开仓");

            }
            else
            { 
                //如果有仓位 方向相同开仓 
                if ((pos.isLong && o.side) || (pos.isShort && !o.side))
                {
                    debug("有仓位 方向相同 继续开仓");

                }
                else //方向相反平仓
                {
                    int pos_size = Math.Abs(pos.Size);//目前仓位大小
                    //未成交合约数量+下单数量 < 仓位数量 才可与将限价单发送到CTP;
                    debug("总仓位大小:" + pos_size.ToString() + "  未成交合约数量:" + ufill_size.ToString());
                    //1.限价平仓,需要检查当前未成交合约数量 如果小于则下单 如果大于则报错
                    if (o.isLimit)
                    {
                        int osize = o.UnsignedSize;
                        //仓位手数 <未成交合约手数 与 当前下单和 则下单错误 提示仓位不足
                        if (pos_size < ufill_size + osize)
                        {
                            GotOrderMessage(o, "报单拒绝|平仓仓位不足");
                            return;
                        }
                    }
                    //2.市价平仓 由于市价平仓,未成交合约数量+下单数量 < 仓位数量 则下市价单,若查过总仓位数量 则撤原来的委托 然后直接市价下单
                    if (o.isMarket)
                    {
                        int osize = o.UnsignedSize;
                        //委托超过总仓位提示错误
                        if (pos_size < osize)
                        {
                            GotOrderMessage(o, "报单拒绝|平仓委托数量超过总仓位");
                            return;
                        }
                        //仓位手数 <未成交合约手数 与 当前下单和 则下单错误 提示仓位不足
                        if (pos_size < ufill_size + osize && ufill_size > 0)
                        {
                            GotOrderMessage(o, "报单警告|市价平仓仓位溢出,将撤委托并强平仓位");
                            //撤单以前的未平仓Order
                            long[] olist = _clearCentre.getPendingOrders(o);
                            //如何进行先撤单 然后在发单的解决方案
                            //一个list包含要撤单的Order当 全部撤完后 触发新单
                            debug("将委托前取消放入OrdActTransaction列别执行");
                            _orderActionTrans.AddDelBeforeInsert(olist, o);
                            //AddDelBeforeInsert(olist, o);
                            foreach (long oid in olist)
                            {
                                //取消委托
                                CancelOrder(oid);
                                //将Order放入队列后等待撤单完成后进行发单
                            }
                            return;
                        }
                    }
                }
            }
            int ordref=-1;
            if (o.isMarket)
                ordref = IBSenderMarketOrder(o.symbol, o.side, o.UnsignedSize);
            else if (o.isLimit)
                ordref = IBSenderLimitOrder(o.symbol, o.side, o.UnsignedSize, o.price);
            else if (o.isStop)
                ordref = IBSenderStopOrder(o.symbol, o.side, o.UnsignedSize, o.stopp);

            o.Broker = this.GetType().FullName;
            o.BrokerKey = ordref.ToString();
            BookOrderAndID(o, ordref);
            
        }
        public void CancelOrder(long oid)
        {
            int ordref = getIBOrdRefViaLocalOrdId(oid);
            if (ordref > 0)
                IBCancelOrder(ordref);
        }

        public void GotTick(Tick k)
        { 
        
        }
        public bool IsLive
        {
            get
            {
                return _connected;
            }
        }

        #region IB对外操作函数

        int IBSenderMarketOrder(string symbol,bool side,int size)
        {
            debug(side.ToString() + "|" + symbol.ToString() + "|" + size.ToString());
            Contract c = _ibhelper.Symbol2IBContract(symbol);
            Krs.Ats.IBNet.Order o = new Krs.Ats.IBNet.Order();
            o.Action = side ? ActionSide.Buy : ActionSide.Sell;
            o.OrderType = OrderType.Market;
            o.TotalQuantity = size;
            int ordRef = NextOrderId;
            client.PlaceOrder(ordRef, c, o);
            return ordRef;
        }

        int IBSenderLimitOrder(string symbol, bool side, int size,decimal limitprice)
        {
            debug(side.ToString() + "|" + symbol.ToString() + "|" + size.ToString());
            Contract c = _ibhelper.Symbol2IBContract(symbol);
            Krs.Ats.IBNet.Order o = new Krs.Ats.IBNet.Order();
            o.Action = side ? ActionSide.Buy : ActionSide.Sell;
            o.OrderType = OrderType.Limit;
            o.LimitPrice = limitprice;
            o.TotalQuantity = size;
            int ordRef = NextOrderId;
            client.PlaceOrder(ordRef, c, o);
            return ordRef;
        }

        //追价单有些问题 需要改进
        int IBSenderStopOrder(string symbol, bool side, int size, decimal stopprice)
        {
            debug(side.ToString() + "|" + symbol.ToString() + "|" + size.ToString());
            Contract c = _ibhelper.Symbol2IBContract(symbol);
            Krs.Ats.IBNet.Order o = new Krs.Ats.IBNet.Order();
            o.Action = side ? ActionSide.Buy : ActionSide.Sell;
            o.OrderType = OrderType.Stop;
            o.TrailStopPrice = stopprice;
            o.TotalQuantity = size;
            int ordRef = NextOrderId;
            client.PlaceOrder(ordRef, c, o);
            return ordRef;
        }

        void IBCancelOrder(int ordref)
        {
            client.CancelOrder(ordref);
        }


        
        #endregion
    }
}
