using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /* 关于委托状态的跟踪与维护
     * 委托状态维护机制
     * 1.通过本地的ordertracker 采集order,trade,cancel的回报来进行状态维护
     * 通过setsize和fillsize来维护fill和partfill的状态，通过cancel来维护取消状态
     * 通过ordererror产生的order reject来维护reject的状态
     * 
     * 2.通过brokersize进行维护 在与broker通讯过程中 维护了委托的映射 本地委托到 brokerside的委托
     * 当brokerside的委托状态发生更新时，我们更新本地的委托 同时通过brokerrouter传递到系统
     * 系统内部的ordertracker只是用于记录和核对的作用
     * 
     * 
     * 
     * 
     * 
     * 风控中心用于处理客户端提交上来的委托以及其他操作的检查
     * 清算中心用于记录有路由返回过来的交易回报 生成实时的交易状态 清算中心会根据接收到的信息进行账户交易信息记录与财务计算
     * 后续事件的订阅者必须无阻塞的执行操作否则会阻塞主线程的事件回报 因此该事件处理函数必须无异常，否则会影响其他后续组件的对该事件的订阅
     * 
     * 
     * 
     * */
    public partial class MsgExchServer
    {
        /// <summary>
        /// 手工插入委托返回对应的委托编号
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public long futs_InsertOrderManual(Order o)
        {
            AssignOrderID(ref o);
            long ordid = o.id;
            _br_GotOrderEvent(o);
            return ordid;
        }

        public void futs_InsertTradeManual(Trade t)
        {
            _br_GotFillEvent(t);
        }



        #region DataFeedRouter接口
        /// <summary>
        /// 将数据路由绑定到交易服务器
        /// </summary>
        /// <param name="datafeedrouter"></param>
        public void BindDataRouter(DataFeedRouter datafeedrouter)
        {
            _datafeedRouter = datafeedrouter;
            _datafeedRouter.GotTickEvent += new TickDelegate(_datafeedRouter_GotTickEvent);

        }

        public void UnBindDataRouter(DataFeedRouter datafeedrouter)
        {
            if (datafeedrouter == _datafeedRouter)
            {
                _datafeedRouter.GotTickEvent -= new TickDelegate(_datafeedRouter_GotTickEvent);
            }
            _datafeedRouter = null;
        }

        void _datafeedRouter_GotTickEvent(Tick k)
        {
            //在DataFeedRouter中有Ringbuffer进行行情缓存
            //1.先向客户端广播Tick行情 
            tl.newTick(k);
            //2.清算中心响应Tick事件
            _clearcentre.GotTick(k);
            //2.对外触发Tick事件 用于被其他组件简体
            if (GotTickEvent != null)
                GotTickEvent(k);
        }

        #endregion


        #region BrokerRouter接口

        /// <summary>
        /// 将交易路由绑定到交易服务器
        /// 路由服务器将操作回报通过tl的操作向客户端进行发送
        /// </summary>
        /// <param name="brokerrouter"></param>
        public void BindBrokerRouter(BrokerRouter brokerrouter)
        {
            _brokerRouter = brokerrouter;
            _brokerRouter.GotCancelEvent += new LongDelegate(_br_GotCancelEvent);
            _brokerRouter.GotFillEvent += new FillDelegate(_br_GotFillEvent);
            _brokerRouter.GotOrderEvent += new OrderDelegate(_br_GotOrderEvent);
            _brokerRouter.GotErrorOrderNotifyEvent += new ErrorOrderNotifyDel(_br_GotOrderErrorNotify);//路由中心返回的委托错误均要通知到清算中心进行委托更新
        }

        public void UnBindBrokerRouter(BrokerRouter brokerrouter)
        {
            if (brokerrouter == _brokerRouter)
            {
                _brokerRouter.GotCancelEvent -= new LongDelegate(_br_GotCancelEvent);
                _brokerRouter.GotFillEvent -= new FillDelegate(_br_GotFillEvent);
                _brokerRouter.GotOrderEvent -= new OrderDelegate(_br_GotOrderEvent);
                _brokerRouter.GotErrorOrderNotifyEvent -= new ErrorOrderNotifyDel(_br_GotOrderErrorNotify);
            }
            _brokerRouter = null;
        }




        #region 委托错误回报处理
        /// <summary>
        /// 响应路由中心委托错误回报
        /// </summary>
        /// <param name="notify"></param>
        void _br_GotOrderErrorNotify(ErrorOrderNotify notify)
        {
            handler_GotOrderErrorNotify(notify);
        }

        
        /// <summary>
        /// 如果在brokerroute send order中发生错误 则我们需要将委托触发到清算中心进行状态更新
        /// 在一段检查产生的order error notify 不需要通知清算中心进行更新
        /// </summary>
        /// <param name="notify"></param>
        /// <param name="neednotify"></param>
        void handler_GotOrderErrorNotify(ErrorOrderNotify notify,bool needlog=true)
        {
            ErrorOrder order = new ErrorOrder(notify.Order, notify.RspInfo);

            //清算中心响应委托错误回报
            //如果需要记录该委托错误 则需要调用清算中心的goterrororder进行处理
            if (needlog)
            {
                _clearcentre.GotErrorOrder(order);
            }
            //放入缓存通知客户端
            _errorordercache.Write(notify);
            if (GotErrorOrderEvent != null)
            {
                GotErrorOrderEvent(order);
            }
        }

        #endregion




        #region 委托回报处理
        void _br_GotOrderEvent(Order o)
        {
            handler_GotOrderEvent(o);
        }

        void handler_GotOrderEvent(Order o)
        {
            

            switch (o.Status)
            { 
                case QSEnumOrderStatus.Filled:
                    o.comment = o.comment +" "+commentFilled;
                    break;
                case QSEnumOrderStatus.PartFilled:
                    o.comment = o.comment + " " + commentPartFilled;
                    break;
                case QSEnumOrderStatus.Canceled:
                    o.comment = o.comment + " " + commentCanceled;
                    break;
                case QSEnumOrderStatus.Placed:
                    o.comment = o.comment + " " + commentPlaced;
                    break;
                case QSEnumOrderStatus.Submited:
                    o.comment = o.comment + " " + commentSubmited;
                    break;
                case QSEnumOrderStatus.Opened:
                    o.comment = o.comment + " " + commentOpened;
                    break;
                default:
                    break;
            }
            //清算中心响应委托回报
            _clearcentre.GotOrder(o);

            //如果需要将委托状态通知发送到客户端 则设置needsend为true
            //路由中心返回委托回报时,发送给客户端的委托需要进行copy 否则后续GotOrderEvent事件如果对委托有修改,则会导致发送给客户端的委托发生变化,委托发送是在线程内延迟执行
            _ocache.Write(new OrderImpl(o));
            if (GotOrderEvent != null)
                GotOrderEvent(o);
        }
        #endregion



        #region 成交回报处理
        /// <summary>
        /// 用于向客户端发送结算完手续费的成交
        /// </summary>
        /// <param name="t"></param>
        public void newCommissionFill(Trade t)
        {
            _fcache.Write(new TradeImpl(t));
            IAccount account = _clearcentre[t.Account];

            //需要将PositionEx移动到这里 在流程内线获得持仓数据并保存到PositionEx 这样就可以避免在sending线程中造成的时间偏移 而最后position 状态形成状态跳跃
            //有新的成交数据系统推送当前方向的持仓数据 注 这里持仓进行了多空区分
            //获得净持仓数据
            Position netpost =account.GetPositionNet(t.symbol);
            _posupdatecache.Write(netpost.GenPositionEx());
            //获得持仓明细数据
            Position pos = account.GetPosition(t.symbol, t.PositionSide);
            _posupdatecache.Write(pos.GenPositionEx());
     
        }

        void _br_GotFillEvent(Trade t)
        {
            //在BrokerRouter->GotFillEvent->ClearCentre.GotFill->adjustcommission->this.GotCommissionFill调用链 形成每笔成交手续费的计算 当计算完毕后 再向客户端进行发送
            //清算中心响应成交回报
            _clearcentre.GotFill(t);//注这里的成交没有结算手续费,成交部分我们需要在结算中心结算结算完手续费后再向客户端发送
            
            //对于newCommissionFill的事件可以插入到这里 这里先执行清算中心GotFill然后 再对外处理

            //对外触发成交事件
            if (GotFillEvent != null)
                GotFillEvent(t);
        }
        #endregion


        #region 委托取消回报处理
        void _br_GotCancelEvent(long oid)
        {
            //清算中心响应取消回报
            _clearcentre.GotCancel(oid);
            //对外触发取消事件
            if (GotCancelEvent != null)
                GotCancelEvent(oid);
        }
        #endregion

        #endregion
    }
}
