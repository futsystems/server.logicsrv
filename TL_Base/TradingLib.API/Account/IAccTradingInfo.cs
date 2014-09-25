using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IAccTradingInfo
    {
        #region 该账户的交易信息
        /// <summary>
        /// 是否有任何持仓
        /// </summary>
        bool AnyPosition { get; }
        /// <summary>
        /// 获得所有持仓对象
        /// </summary>
        IEnumerable<Position> Positions { get; }
        /// <summary>
        /// 获得所有委托对象
        /// </summary>
        IEnumerable<Order> Orders { get; }
        /// <summary>
        /// 获得所有成交对象
        /// </summary>
        IEnumerable<Trade> Trades { get; }
        /// <summary>
        /// 获得所有取消
        /// </summary>
        long[] Cancels { get; }
        /// <summary>
        /// 获得所有隔夜持仓数据
        /// </summary>
        IEnumerable<Position> YdPositions { get; }
        /// <summary>
        /// 获得某个合约的持仓对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Position getPosition(string symbol,bool side);
        #endregion
    }
}
