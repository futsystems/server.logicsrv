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
            //获得某个symbol的tick数据
            broker.GetSymbolTickEvent += new GetSymbolTickDel(Broker_GetSymbolTickEvent);
            //数据路由中Tick事件驱动交易通道中由Tick部分
            DataFeedRouter.GotTickEvent += new TickDelegate(broker.GotTick);
            //将清算中心绑定到交易通道
            broker.ClearCentre = new ClearCentreAdapterToBroker(_clearCentre);

            if (broker is TLBrokerBase)
            {
                TLBrokerBase brokerbase = broker as TLBrokerBase;
                brokerbase.NewBrokerOrderEvent += new OrderDelegate(brokerbase_NewBrokerOrderEvent);
                brokerbase.NewBrokerOrderUpdateEvent += new OrderDelegate(brokerbase_NewBrokerOrderUpdateEvent);
                brokerbase.NewBrokerFillEvent += new FillDelegate(brokerbase_NewBrokerFillEvent);
                brokerbase.NewBrokerPositionCloseDetailEvent += new Action<PositionCloseDetail>(brokerbase_NewBrokerPositionCloseDetailEvent);
            }
        }


        #region 保存从成交侧返回的成交信息
        void brokerbase_NewBrokerPositionCloseDetailEvent(PositionCloseDetail obj)
        {
            _clearCentre.LogBrokerPositionCloseDetail(obj);
        }

        void brokerbase_NewBrokerFillEvent(Trade t)
        {
            _clearCentre.LogBrokerTrade(t);
        }

        void brokerbase_NewBrokerOrderUpdateEvent(Order o)
        {
            _clearCentre.LogBrokerOrderUpdate(o);
        }

        void brokerbase_NewBrokerOrderEvent(Order o)
        {
            _clearCentre.LogBrokerOrder(o);
        }
        #endregion

    }
}
