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
        //路由侧只需要记录委托关系,不需要记录成交 用于恢复交易状态，路由侧只需要恢复委托 保存中间分解路径
        void LogRouterOrder(Order o)
        {
            TLCtxHelper.ModuleDataRepository.NewOrder(o);
        }

        void LogRouterOrderUpdate(Order o)
        {
            TLCtxHelper.ModuleDataRepository.UpdateOrder(o);
        }

        void LogBrokerPositionCloseDetailEvent(PositionCloseDetail obj)
        {
            TLCtxHelper.ModuleDataRepository.NewPositionCloseDetail(obj);
        }

        void LogBrokerFillEvent(Trade t)
        {
            TLCtxHelper.ModuleDataRepository.NewTrade(t);
        }

        void LogBrokerOrderUpdateEvent(Order o)
        {
            TLCtxHelper.ModuleDataRepository.UpdateOrder(o);
        }

        void LogBrokerOrderEvent(Order o)
        {
            TLCtxHelper.ModuleDataRepository.NewOrder(o);
        }
    }
}
