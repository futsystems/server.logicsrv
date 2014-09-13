using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /*
    /// <summary>
    /// 账户集的统计接口
    /// </summary>
    public interface IGeneralStatistic
    {
        /// <summary>
        /// 账户个数
        /// </summary>
        int NumTotalAccount { get; }
        /// <summary>
        /// 委托数目
        /// </summary>
        int NumOrders { get; }
        /// <summary>
        /// 成交数目
        /// </summary>
        int NumTrades { get; }
        /// <summary>
        /// 持长数目
        /// </summary>
        int NumPositions { get; }
        /// <summary>
        /// 累计当前权益
        /// </summary>
        decimal SumEquity { get; }
        /// <summary>
        /// 累计购买力
        /// </summary>
        decimal SumBuyPower { get; }
        /// <summary>
        /// 累计平仓权益
        /// </summary>
        decimal SumRealizedPL { get; }
        /// <summary>
        /// 累计未平仓权益
        /// </summary>
        decimal SumUnRealizedPL { get; }
        /// <summary>
        /// 累计手续费
        /// </summary>
        decimal SumCommission { get; }
        /// <summary>
        /// 累计净利润
        /// </summary>
        decimal SumNetProfit { get; }
        /// <summary>
        /// 累计保证金占用
        /// </summary>
        decimal SumMargin { get; }
        /// <summary>
        /// 累计冻结保证金
        /// </summary>
        decimal SumFrozenMargin { get; }

    }

    /// <summary>
    /// 获得一组配资帐号的整体统计
    /// </summary>
    public interface IFinStatistic : IGeneralStatistic
    {
        /// <summary>
        /// 所有账户的累计配资额度
        /// </summary>
        decimal SumFinAmmount { get; }
        /// <summary>
        /// 所有账户的配资费用
        /// </summary>
        decimal SumFinFee { get; }
    }

    /// <summary>
    /// 合约仓位多空分布
    /// </summary>
    public interface ISymbolPositionStatistic
    { 

        string Symbol{get;}
        int LongAccountNum { get; }
        int ShortAccountNum { get; }
        int LongPositionSize { get; }
        decimal LongPositionPrice { get; }
        int ShortPositionSize { get; }
        decimal ShortPositionPrice { get; }
        /// <summary>
        /// 清空所有数据
        /// </summary>
        void Reset();
        void GotPosition(Position p);
    }**/
}
