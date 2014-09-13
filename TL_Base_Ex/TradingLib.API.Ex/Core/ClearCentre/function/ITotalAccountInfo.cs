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
        /// 获得总账户positionTracker
        /// </summary>
        Object DefaultPositionTracker { get; }

        /// <summary>
        /// 获得总账户OrderTracker
        /// </summary>
        Object DefaultOrderTracker { get; }

        /// <summary>
        /// 总账户成交列表
        /// </summary>
        List<Trade> DefaultTradeList { get; }

        /// <summary>
        /// 总账户隔夜持仓列表
        /// </summary>
        Object DefaultPositionHoldTracker { get; }

        /// <summary>
        /// 获得当前所有持仓
        /// </summary>
        Position[] PositionsHoldNow { get; }

        /// <summary>
        /// 获得上次结算所有持仓
        /// </summary>
        Position[] PositionHoldLastSettleday { get; }
    }
}
