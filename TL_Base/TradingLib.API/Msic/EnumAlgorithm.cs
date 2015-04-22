using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 浮动盈亏算法
    /// </summary>
    public enum QSEnumAlgorithm
    {
        /// <summary>
        /// 浮盈浮亏都计
        /// </summary>
        AG_All = 1,

        /// <summary>
        /// 只计算亏损
        /// </summary>
        AG_OnlyLost = 1,

        /// <summary>
        /// 只计算盈利
        /// </summary>
        AG_OnlyGain = 2,

        /// <summary>
        /// 都不计算
        /// </summary>
        AG_None = 3,
        

    }
}
