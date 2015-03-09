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
        public void ManualInsertOrder(Order o)
        {
            AssignOrderID(ref o);
            long ordid = o.id;
            //设定委托相关编号
            o.OrderSysID = o.OrderSeq.ToString();
            o.BrokerRemoteOrderID = o.OrderSysID;
            OnOrderEvent(o);
            debug("insert ordre manual .....", QSEnumDebugLevel.INFO);
            //return ordid;

        }

        public void ManualInsertTrade(Trade t)
        {
            OnFillEvent(t);
            debug("insert trade manual ....", QSEnumDebugLevel.INFO);
        }



        #region DataFeedRouter接口
        /// <summary>
        /// 将数据路由绑定到交易服务器
        /// </summary>
        /// <param name="datafeedrouter"></param>
        //public void BindDataRouter(DataFeedRouter datafeedrouter)
        //{
        //    _datafeedRouter = datafeedrouter;
        //    _datafeedRouter.GotTickEvent += new TickDelegate(_datafeedRouter_GotTickEvent);

        //}

        //public void UnBindDataRouter(DataFeedRouter datafeedrouter)
        //{
        //    if (datafeedrouter == _datafeedRouter)
        //    {
        //        _datafeedRouter.GotTickEvent -= new TickDelegate(_datafeedRouter_GotTickEvent);
        //    }
        //    _datafeedRouter = null;
        //}

        public void OnTickEvent(Tick k)
        {
            //在DataFeedRouter中有Ringbuffer进行行情缓存
            //1.先向客户端广播Tick行情 
            tl.newTick(k);
            //2.清算中心响应Tick事件
            TLCtxHelper.ModuleClearCentre.GotTick(k);
            //2.对外触发Tick事件 用于被其他组件简体
            //debug("got tick:" + TickImpl.Serialize(k), QSEnumDebugLevel.INFO);
            TLCtxHelper.EventIndicator.FireTickEvent(k);
            //if (GotTickEvent != null)
            //    GotTickEvent(k);
        }

        #endregion


        #region BrokerRouter接口

        /// <summary>
        /// 将交易路由绑定到交易服务器
        /// 路由服务器将操作回报通过tl的操作向客户端进行发送
        /// </summary>
        /// <param name="brokerrouter"></param>
        //public void BindBrokerRouter(BrokerRouter brokerrouter)
        //{
        //    //_brokerRouter = brokerrouter;
        //    _brokerRouter.GotCancelEvent += new LongDelegate(_br_GotCancelEvent);
        //    _brokerRouter.GotFillEvent += new FillDelegate(_br_GotFillEvent);
        //    _brokerRouter.GotOrderEvent += new OrderDelegate(_br_GotOrderEvent);

        //    _brokerRouter.GotOrderErrorEvent += new OrderErrorDelegate(_br_GotOrderErrorEvent);//路由中心返回的委托错误均要通知到清算中心进行委托更新
        //    _brokerRouter.GotOrderActionErrorEvent += new OrderActionErrorDelegate(_br_GotOrderActionErrorEvent);
        //}

        /// <summary>
        /// 通过委托编号 查找路由侧分解发送的委托
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public Order SentRouterOrder(long val)
        {

            return TLCtxHelper.ModuleBrokerRouter.SentRouterOrder(val);
        }



        //public void UnBindBrokerRouter(BrokerRouter brokerrouter)
        //{
        //    if (brokerrouter == _brokerRouter)
        //    {
        //        _brokerRouter.GotCancelEvent -= new LongDelegate(_br_GotCancelEvent);
        //        _brokerRouter.GotFillEvent -= new FillDelegate(_br_GotFillEvent);
        //        _brokerRouter.GotOrderEvent -= new OrderDelegate(_br_GotOrderEvent);
        //        _brokerRouter.GotOrderErrorEvent -= new OrderErrorDelegate(_br_GotOrderErrorEvent);
        //    }
        //    _brokerRouter = null;
        //}



        #region 委托操作错误回报处理

        public void OnOrderActionErrorEvent(OrderAction action, RspInfo info)
        {
            //对外通知
            NotifyOrderActionError(action, info);
        }
        #endregion


        #region 委托错误回报处理
        /// <summary>
        /// 响应路由中心委托错误回报
        /// </summary>
        /// <param name="notify"></param>
        public void OnOrderErrorEvent(Order order, RspInfo info)
        {
            handler_GotOrderErrorEvent(order,info);
        }

        
        /// <summary>
        /// 如果在brokerroute send order中发生错误 则我们需要将委托触发到清算中心进行状态更新
        /// 在一段检查产生的order error notify 不需要通知清算中心进行更新
        /// </summary>
        /// <param name="notify"></param>
        /// <param name="neednotify"></param>
        void handler_GotOrderErrorEvent(Order order,RspInfo info, bool needlog = true)
        {
            //清算中心响应委托错误回报
            //如果需要记录该委托错误 则需要调用清算中心的goterrororder进行处理
            if (needlog)
            {
                TLCtxHelper.ModuleClearCentre.GotOrderError(order, info);
            }
            //对外通知
            NotifyOrderError(order, info);
        }

        #endregion




        #region 委托回报处理
        public void OnOrderEvent(Order o)
        {
            handler_GotOrderEvent(o);
        }

        void handler_GotOrderEvent(Order o)
        {
            switch (o.Status)
            { 
                case QSEnumOrderStatus.Filled:
                    o.Comment = string.IsNullOrEmpty(o.Comment)?commentFilled:o.Comment;
                    break;
                case QSEnumOrderStatus.PartFilled:
                    o.Comment = string.IsNullOrEmpty(o.Comment)?commentPartFilled:o.Comment;
                    break;
                case QSEnumOrderStatus.Canceled:
                    o.Comment = string.IsNullOrEmpty(o.Comment)?commentCanceled:o.Comment;
                    break;
                case QSEnumOrderStatus.Placed:
                    o.Comment = string.IsNullOrEmpty(o.Comment)?commentPlaced:o.Comment;
                    break;
                case QSEnumOrderStatus.Submited:
                    o.Comment = string.IsNullOrEmpty(o.Comment)?commentSubmited:o.Comment;
                    break;
                case QSEnumOrderStatus.Opened:
                    o.Comment = string.IsNullOrEmpty(o.Comment)?commentOpened:o.Comment;
                    break;
                default:
                    break;
            }
            //清算中心响应委托回报
            TLCtxHelper.ModuleClearCentre.GotOrder(o);

            //if (o.Status == QSEnumOrderStatus.Canceled)
            //{
            //    //清算中心响应取消回报
            //    _clearcentre.GotCancel(o.id);
            //}

            //对外通知
            NotifyOrder(o);
        }
        #endregion



        #region 成交回报处理
        /// <summary>
        /// 响应成交路由返回的成交回报
        /// </summary>
        /// <param name="t"></param>
        public void OnFillEvent(Trade t)
        {
            //设定系统内成交编号
            AssignTradeID(ref t);
            //在BrokerRouter->GotFillEvent->ClearCentre.GotFill->adjustcommission->this.GotCommissionFill调用链 形成每笔成交手续费的计算 当计算完毕后 再向客户端进行发送
            //清算中心响应成交回报

            TLCtxHelper.ModuleClearCentre.GotFill(t);//注这里的成交没有结算手续费,成交部分我们需要在结算中心结算结算完手续费后再向客户端发送
            
            //对外通知
            NotifyFill(t);

            IAccount account = TLCtxHelper.ModuleAccountManager[t.Account];
            if (account != null)
            {
                //有新的成交数据后,系统自动发送对应的持仓信息
                Position pos = account.GetPosition(t.Symbol, t.PositionSide);
                if (pos != null)
                {
                    NotifyPositionUpdate(pos);
                }
            }

        }

        #endregion


        #region 委托取消回报处理
        public void OnCancelEvent(long oid)
        {
            //清算中心响应取消回报
            TLCtxHelper.ModuleClearCentre.GotCancel(oid);
            //对外触发取消事件
            //if (GotCancelEvent != null)
            //    GotCancelEvent(oid);
            
        }
        #endregion

        #endregion
    }
}
