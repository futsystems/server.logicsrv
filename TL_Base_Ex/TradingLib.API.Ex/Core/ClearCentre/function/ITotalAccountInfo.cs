using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 获得柜台系统总的交易信息
    /// </summary>
    public interface ITotalAccountInfo
    {
        /// <summary>
        /// 通过order id找到对应的Order
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        Order SentOrder(long oid);

        /// <summary>
        /// 所有委托
        /// </summary>
        IEnumerable<Order> TotalOrders { get;}

        /// <summary>
        /// 所有持仓
        /// </summary>
        IEnumerable<Position> TotalPositions { get;}

        /// <summary>
        /// 所有成交
        /// </summary>
        IEnumerable<Trade> TotalTrades { get;}

        /// <summary>
        /// 所有隔夜持仓
        /// </summary>
        //IEnumerable<Position> TotalYdPositions { get;}
    }
}
