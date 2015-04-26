using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{
    public enum QSEnumMarginPrice
    {
        /// <summary>
        /// 昨结算价
        /// </summary>
        [Description("昨日结算价")]
        PreSettlementPrice=1,

        /// <summary>
        /// 最新价格
        /// </summary>
        [Description("最新价")]
        TradePrice=2,

        /// <summary>
        /// 成交均价
        /// </summary>
        [Description("成交均价")]
        AveragePrice=3,

        /// <summary>
        /// 开仓价格
        /// </summary>
        [Description("开仓均价")]
        OpenPrice=4,
    }
}
