using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    public partial class BrokerRouter
    {
        /// <summary>
        /// 将某个Broker加载到系统
        /// </summary>
        /// <param name="broker"></param>
        public void LoadBroker(IBroker broker)
        {
            //将Broker的事件触发绑定到本地函数回调
            broker.GotOrderEvent += new OrderDelegate(Broker_GotOrder);
            broker.GotCancelEvent += new LongDelegate(Broker_GotCancel);
            broker.GotFillEvent += new FillDelegate(Broker_GotFill);

            broker.GotOrderErrorEvent += new OrderErrorDelegate(Broker_GotOrderError);
            broker.GotOrderActionErrorEvent += new OrderActionErrorDelegate(Broker_GotOrderActionErrorEvent);
            //获得某个symbol的tick数据
            broker.GetSymbolTickEvent += new GetSymbolTickDel(Broker_GetSymbolTickEvent);
            //数据路由中Tick事件驱动交易通道中由Tick部分
            DataFeedRouter.GotTickEvent += new TickDelegate(broker.GotTick);
            //将清算中心绑定到交易通道
            broker.ClearCentre = new ClearCentreAdapterToBroker(_clearCentre);

            if (broker is TLBrokerBase)
            {
                TLBrokerBase brokerbase = broker as TLBrokerBase;
                brokerbase.NewBrokerOrderEvent += new OrderDelegate(LogBrokerOrderEvent);
                brokerbase.NewBrokerOrderUpdateEvent += new OrderDelegate(LogBrokerOrderUpdateEvent);
                brokerbase.NewBrokerFillEvent += new FillDelegate(LogBrokerFillEvent);
                brokerbase.NewBrokerPositionCloseDetailEvent += new Action<PositionCloseDetail>(LogBrokerPositionCloseDetailEvent);
            }
        }


        #region Broker向本地回报操作
        /// <summary>
        /// 交易接口查询某个symbol的当前最新Tick快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Tick Broker_GetSymbolTickEvent(string symbol)
        {
            try
            {
                return DataFeedRouter.GetTickSnapshot(symbol);
            }
            catch (Exception ex)
            {
                debug(PROGRAM + ":get symbol tick snapshot error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return null;
            }
        }




        /// <summary>
        /// 发送内部产生的委托错误
        /// </summary>
        /// <param name="o"></param>
        /// <param name="errortitle"></param>
        void GotOrderErrorNotify(Order o, string errortitle)
        {
            RspInfo info = RspInfoEx.Fill(errortitle);
            o.Comment = info.ErrorMessage;
            Broker_GotOrderError(o, info);
        }


        /// <summary>
        /// 当交易通道有Order错误信息时,进行处理
        /// </summary>
        void Broker_GotOrderError(Order order, RspInfo error)
        {
            if (order != null && order.isValid)
            {
                if (order.Breed == QSEnumOrderBreedType.ROUTER)
                {
                    debug("Reply ErrorOrder To Spliter:" + order.GetOrderInfo() + " ErrorTitle:" + error.ErrorMessage, QSEnumDebugLevel.INFO);
                    LogRouterOrderUpdate(order);//更新路由侧委托
                    _splittracker.GotSonOrderError(order, error);
                    return;
                }
                debug("Reply ErrorOrder To MessageExch:" + order.GetOrderInfo() + " ErrorTitle:" + error.ErrorMessage, QSEnumDebugLevel.INFO);
                _errorordernotifycache.Write(new OrderErrorPack(order, error));
            }
            else
            {
                debug("Got Invalid OrderError", QSEnumDebugLevel.ERROR);
            }
        }

        /// <summary>
        /// 获得委托操作错误
        /// </summary>
        /// <param name="action"></param>
        /// <param name="error"></param>
        void Broker_GotOrderActionErrorEvent(OrderAction action, RspInfo error)
        {
            if (action != null)
            {

            }
            else
            {
                debug("Got Invalid OrderActionError", QSEnumDebugLevel.ERROR);
            }
        }



        /// <summary>
        /// 当有成交时候回报msgexch
        /// </summary>
        void Broker_GotFill(Trade fill)
        {
            if (fill != null && fill.isValid)
            {
                if (fill.Breed == QSEnumOrderBreedType.ROUTER)
                {
                    debug("Reply Fill To Spliter:" + fill.GetTradeInfo(), QSEnumDebugLevel.INFO);
                    _splittracker.GotSonFill(fill);
                    return;
                }
                debug("Reply Fill To MessageExch:" + fill.GetTradeInfo(), QSEnumDebugLevel.INFO);
                _fillcache.Write(new TradeImpl(fill));
            }
            else
            {
                debug("Got Invalid Fill", QSEnumDebugLevel.ERROR);
            }
        }

        /// <summary>
        /// 委托正确回报时回报msgexch
        /// 这里回报需要判断是否需同通过委托拆分器处理,如果是子委托则通过委托拆分器处理
        /// </summary>

        void Broker_GotOrder(Order o)
        {
            if (o != null && o.isValid)
            {
                //这里需要判断,该委托回报是拆分过的子委托还是分帐户侧的委托 如果是拆分过的委托则需要回报给拆分器
                if (o.Breed == QSEnumOrderBreedType.ROUTER)
                {
                    debug("Reply Order To Spliter:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
                    LogRouterOrderUpdate(o);//更新路由侧委托
                    _splittracker.GotSonOrder(o);
                    return;
                }
                Order no = new OrderImpl(o);
                debug("Reply Order To MessageExch:" + no.GetOrderInfo(), QSEnumDebugLevel.INFO);
                _ordercache.Write(no);
            }
            else
            {
                debug("Got Invalid Order", QSEnumDebugLevel.ERROR);
            }
        }

        /// <summary>
        /// 撤单正确回报时回报msgexch
        /// </summary>

        void Broker_GotCancel(long oid)
        {
            debug("Reply Cancel To MessageExch:" + oid.ToString());
            _cancelcache.Write(oid);
        }

        #endregion 

       



       

    }
}
