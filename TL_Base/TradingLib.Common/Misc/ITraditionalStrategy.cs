using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /// <summary>
    /// 持仓快速止盈 止损 策略接口
    /// </summary>
    public interface ITraditionalStrategy
    {
        /// <summary>
        /// 止损参数
        /// </summary>
        StopLossArgs StopArgs { get; set; }
        /// <summary>
        /// 止盈参数
        /// </summary>
        ProfitArgs ProfitArgs { get; set; }

        /// <summary>
        /// 是否处于数据恢复过程中
        /// </summary>
        bool IsInResume { get; set; }
    }
}
