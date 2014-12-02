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

        //public int SentSize(Order o)
        //{
        //    IAccount account = this[o.Account];
            
        //}

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
        /// 某个委托是否被维护
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public bool IsOrderTracked(long oid)
        {
            return totaltk.IsTracked(oid);
        }
        /// <summary>
        /// 所有委托
        /// </summary>
        public IEnumerable<Order> TotalOrders { get { return totaltk.TotalOrders; } }

        /// <summary>
        /// 所有持仓
        /// </summary>
        public IEnumerable<Position> TotalPositions { get { return totaltk.TotalPositions; } }

        /// <summary>
        /// 所有成交
        /// </summary>
        public IEnumerable<Trade> TotalTrades { get { return totaltk.TotalTrades; } }
        #endregion

    }
}
