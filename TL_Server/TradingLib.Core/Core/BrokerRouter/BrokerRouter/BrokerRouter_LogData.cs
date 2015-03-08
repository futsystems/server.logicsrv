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
        #region 保存路由分拆委托和成交侧交易信息
        //路由侧只需要记录委托关系,不需要记录成交 用于恢复交易状态，路由侧只需要恢复委托 保存中间分解路径
        void LogRouterOrder(Order o)
        {
            _clearCentre.LogRouterOrder(o);
        }
        void LogRouterOrderUpdate(Order o)
        {
            _clearCentre.LogRouterOrderUpdate(o);
        }



        void LogBrokerPositionCloseDetailEvent(PositionCloseDetail obj)
        {
            _clearCentre.LogBrokerPositionCloseDetail(obj);
        }

        void LogBrokerFillEvent(Trade t)
        {
            _clearCentre.LogBrokerTrade(t);
        }

        void LogBrokerOrderUpdateEvent(Order o)
        {
            _clearCentre.LogBrokerOrderUpdate(o);
        }

        void LogBrokerOrderEvent(Order o)
        {
            _clearCentre.LogBrokerOrder(o);
        }
        #endregion
    }
}
