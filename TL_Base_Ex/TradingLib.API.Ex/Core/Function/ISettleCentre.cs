using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{

    public interface ISettleCentre
    {
        /// <summary>
        /// 结算模式，结算模式决定了清算中心数据加载方式
        /// </summary>
        QSEnumSettleMode SettleMode { get; }

        /// <summary>
        /// 上次结算日
        /// </summary>
        int LastSettleday { get; }

        /// <summary>
        /// 当前交易日
        /// </summary>
        int Tradingday { get; }

        /// <summary>
        /// 结算时间
        /// </summary>
        int SettleTime { get; }

        /// <summary>
        /// 重置时间
        /// </summary>
        int ResetTime { get; }

        /// <summary>
        /// 下一个结算时间
        /// </summary>
        long NextSettleTime { get; }
    }
}
