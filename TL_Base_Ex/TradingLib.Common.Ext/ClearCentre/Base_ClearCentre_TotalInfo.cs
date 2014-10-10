using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class ClearCentreBase
    {

        #region 【ITotalAccountInfo】 获得整体的交易信息

        public OrderTracker OrderTracker { get { return totaltk.OrderTracker; } }
        ///// <summary>
        ///// 通过OrderId获得该Order
        ///// </summary>
        ///// <param name="oid"></param>
        ///// <returns></returns>
        public Order SentOrder(long oid)
        {
            return totaltk.SentOrder(oid);
        }

        /// <summary>
        /// 所有委托
        /// </summary>
        public IEnumerable<Order> TotalOrders { get { return totaltk.OrderTracker; } }

        /// <summary>
        /// 所有持仓
        /// </summary>
        public IEnumerable<Position> TotalPositions { get { return totaltk.PositionTracker; } }

        /// <summary>
        /// 所有成交
        /// </summary>
        public IEnumerable<Trade> TotalTrades { get { return totaltk.TradeTracker; } }

        /// <summary>
        /// 所有隔夜持仓
        /// </summary>
        public IEnumerable<Position> TotalYdPositions { get { return totaltk.PositionHoldTracker; } }
        #endregion

    }
}
