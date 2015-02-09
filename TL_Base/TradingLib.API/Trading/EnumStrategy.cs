using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{

    public enum QSEnumAvabileFundStrategy
    { 
        /// <summary>
        /// 
        /// </summary>
        UnPLInclude,

        /// <summary>
        /// 
        /// </summary>
        UnPLExclude,
    }

    /// <summary>
    /// 持仓保证金算法中策略
    /// </summary>
    public enum QSEnumMarginStrategy
    {
        /// <summary>
        /// 最新价
        /// 持仓保证金会按最新价格变动而发生变化
        /// </summary>
        [Description("最新价")]
        LastPrice,

        /// <summary>
        /// 持仓成本
        /// 当日开仓按开仓价格，隔夜仓结算后按昨结算来计算持仓保证金
        /// 保证金金额在当日不发生变化
        /// </summary>
        [Description("持仓成本")]
        PositionCost,
    }
}
