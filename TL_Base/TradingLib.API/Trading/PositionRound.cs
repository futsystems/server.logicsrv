using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 持仓回合记录接口
    /// 详细记录了每个交易回合的相关数据
    /// </summary>
    public interface  PositionRound
    {
        /// <summary>
        /// 帐户编号
        /// </summary>
        string Account { get; }

        /// <summary>
        /// 合约信息
        /// </summary>
        Symbol oSymbol { get; }

        /// <summary>
        /// 合约
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// 品种SecurityCode
        /// </summary>
        string Security { get; }

        /// <summary>
        /// 证券品种
        /// </summary>
        SecurityType Type { get; }

        /// <summary>
        /// 乘数
        /// </summary>
        int Multiple { get; }

        /// <summary>
        /// 入场时间
        /// </summary>
        long EntryTime { get; }

        /// <summary>
        /// 开仓手数
        /// </summary>
        int EntrySize { get; }

        /// <summary>
        /// 开仓均价
        /// </summary>
        decimal EntryPrice { get; }

        /// <summary>
        /// 开仓手续费
        /// </summary>
        decimal EntryCommission { get; }

        /// <summary>
        /// 平仓时间
        /// </summary>
        long ExitTime { get; }

        /// <summary>
        /// 平仓数量
        /// </summary>
        int ExitSize { get; }

        /// <summary>
        /// 平仓均价
        /// </summary>
        decimal ExitPrice { get; }

        /// <summary>
        /// 平仓手续费
        /// </summary>
        decimal ExitCommission { get; }

        /// <summary>
        /// 持仓期间最高价
        /// </summary>
        decimal Highest { get; }

        /// <summary>
        /// 持仓期间最低价
        /// </summary>
        decimal Lowest { get; }

        /// <summary>
        /// 平均每手盈亏点数
        /// </summary>
        decimal Points { get; }

        /// <summary>
        /// 累计盈亏点数
        /// </summary>
        decimal TotalPoints { get; }

        /// <summary>
        /// 盈利
        /// </summary>
        decimal Profit { get; }

        /// <summary>
        /// 累计手续费
        /// </summary>
        decimal Commissoin { get; }

        /// <summary>
        /// 净盈利
        /// </summary>
        decimal NetProfit { get; }

        /// <summary>
        /// 盈亏标识
        /// </summary>
        bool WL { get; }

        /// <summary>
        /// 交易数量
        /// </summary>
        int Size { get; }

        /// <summary>
        /// 持仓方向标识
        /// </summary>
        bool Side { get; }

        /// <summary>
        /// 当前持仓手数
        /// </summary>
        int HoldSize { get; }

        /// <summary>
        /// 持仓回合是否打开
        /// </summary>
        bool IsOpened { get; }

        /// <summary>
        /// 持仓回合是否关闭
        /// </summary>
        bool IsClosed {get;}

        string PRKey { get; }

        bool EqualPosition(Position pos);
    }
}
