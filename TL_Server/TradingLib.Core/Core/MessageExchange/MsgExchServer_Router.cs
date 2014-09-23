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
     * 
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
            //debug("messsage gottick:" + k.ToString(),QSEnumDebugLevel.INFO);
            //单机模式通过tlserver向客户端转发tick
            //if (_srvmode == QSEnumServerMode.StandAlone)
            tl.newTick(k);
            //1.首先迅速的让tlserver将该新Tick分发到客户端newtick 与其他信息发送是使用不同的2个ZeroMQ,因此这里不用缓存
            //若是多市场 有多个数据线程转发Tick可能会导致线程不安全,需要用中间缓存来实现单线程发送市场数据
            //再转发给其他组件注册到tradingserver的gottick事件 clearcentre
            if (GotTickEvent != null)
                GotTickEvent(k);
            //_kcache.Write(k);
            //2.异步处理tick数据包含储存Tick,生成K线数据等
            //_asynTickData.newTick(k);
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
            handler_GotOrderErrorNotify(notify, true);
        }

        
        /// <summary>
        /// 如果在brokerroute send order中发生错误 则我们需要将委托触发到清算中心进行状态更新
        /// 在一段检查产生的order error notify 不需要通知清算中心进行更新
        /// </summary>
        /// <param name="notify"></param>
        /// <param name="neednotify"></param>
        void handler_GotOrderErrorNotify(ErrorOrderNotify notify, bool neednotify = false)
        {

            _errorordercache.Write(notify);
            if (neednotify)
            {
                if (GotOrderEvent != null)
                    GotOrderEvent(notify.Order);
            }

            ErrorOrder order = new ErrorOrder(notify.Order, notify.RspInfo);
            if (GotErrorOrderEvent != null)
            {
                GotErrorOrderEvent(order);
            }
        }
        #endregion




        #region 委托回报处理
        void _br_GotOrderEvent(Order o)
        {
            handler_GotOrderEvent(o, true);
        }

        void handler_GotOrderEvent(Order o, bool needsend = true)
        {
            switch (o.Status)
            { 
                case QSEnumOrderStatus.Filled:
                    o.comment = commentFilled;
                    break;
                case QSEnumOrderStatus.PartFilled:
                    o.comment = commentPartFilled;
                    break;
                case QSEnumOrderStatus.Canceled:
                    o.comment = commentCanceled;
                    break;
                case QSEnumOrderStatus.Placed:
                    o.comment = commentPlaced;
                    break;
                case QSEnumOrderStatus.Submited:
                    o.comment = commentSubmited;
                    break;
                case QSEnumOrderStatus.Opened:
                    o.comment = commentOpened;
                    break;
                default:
                    break;
            }
            //如果需要将委托状态通知发送到客户端 则设置needsend为true
            if (needsend)
            {
                _ocache.Write(new OrderImpl(o));
            }
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
            //debug("Trade Info sectype:" + t.Security.ToString() + " currency:" + t.Currency.ToString() + " exchange:" + t.Exchange, QSEnumDebugLevel.INFO);
            _fcache.Write(new TradeImpl(t));


            //有新的成交数据系统推送当前方向的持仓数据 注 这里持仓进行了多空区分
            Position pos = _clearcentre.getPosition(t.Account, t.symbol,t.PositionSide);
            debug("New Positon Update:" + pos.ToString(), QSEnumDebugLevel.INFO);
            _posupdatecache.Write(pos);

            //获得净持仓数据
            Position netpost =_clearcentre.getPosition(t.Account,t.symbol);
            debug("netpost null:" + (netpost == null).ToString(), QSEnumDebugLevel.INFO);
            _posupdatecache.Write(netpost);          
        }

        void _br_GotFillEvent(Trade t)
        {
            //_fcache.Write(new TradeImpl(t));//注这里的成交没有结算手续费,成交部分我们需要在结算中心结算结算完手续费后再向客户端发送
            if (GotFillEvent != null)
                GotFillEvent(t);
        }
        #endregion


        #region 委托取消回报处理
        void _br_GotCancelEvent(long oid)
        {
            //_ccache.Write(oid);
            if (GotCancelEvent != null)//对外触发取消事件
                GotCancelEvent(oid);
        }
        #endregion

        #endregion
    }
}
