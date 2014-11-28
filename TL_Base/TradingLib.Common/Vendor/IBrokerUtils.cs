using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class  IBrokerUtils
    {
        #region 对象过滤 返回对象不toarray避免的内存引用copy所有的计算只需要进行一次foreach循环
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Order> FilterOrders(this IBroker broker, SecurityType type)
        {
            return broker.Orders.Where(o => o.SecurityType == type);
        }

        public static IEnumerable<Order> FilterPendingOrders(this IBroker broker, SecurityType type)
        {
            return broker.Orders.Where(o => o.SecurityType == type && o.IsPending());
        }

        public static IEnumerable<Trade> FilterTrades(this IBroker broker, SecurityType type)
        {
            return broker.Trades.Where(f => f.SecurityType == type);
        }

        public static IEnumerable<Position> FilterPositions(this IBroker broker, SecurityType type)
        {
            return broker.Positions.Where(p => p.oSymbol.SecurityType == type);
        }
        #endregion

        #region 计算期货财务数据
        public static decimal CalFutMargin(this IBroker broker)
        {
            return FilterPositions(broker, SecurityType.FUT).Sum(pos => pos.CalcPositionMargin());
        }

        public static decimal CalFutMarginFrozen(this IBroker broker)
        {
            return 0;
            //return FilterPendingOrders(broker, SecurityType.FUT).Where(o => o.IsEntryPosition).Sum(e => account.CalOrderFundRequired(e, 0));
        }

        public static decimal CalFutUnRealizedPL(this IBroker broker)
        {
            return FilterPositions(broker, SecurityType.FUT).Sum(pos => pos.CalcUnRealizedPL());
        }

        public static decimal CalFutSettleUnRealizedPL(this IBroker broker)
        {
            return FilterPositions(broker, SecurityType.FUT).Sum(pos => pos.CalcSettleUnRealizedPL());
        }

        public static decimal CalFutRealizedPL(this IBroker broker)
        {
            return FilterPositions(broker, SecurityType.FUT).Sum(pos => pos.CalcRealizedPL());
        }

        public static decimal CalFutCommission(this IBroker broker)
        {
            return FilterTrades(broker, SecurityType.FUT).Sum(fill => fill.GetCommission());
        }

        public static decimal CalFutCash(this IBroker broker)
        {
            return CalFutRealizedPL(broker) + CalFutUnRealizedPL(broker) - CalFutCommission(broker);
        }

        public static decimal CalFutLiquidation(this IBroker broker)
        {
            return CalFutCash(broker);
        }

        public static decimal CalFutMoneyUsed(this IBroker broker)
        {
            return CalFutMargin(broker) + CalFutMarginFrozen(broker);
        }


        #endregion
    }
}
