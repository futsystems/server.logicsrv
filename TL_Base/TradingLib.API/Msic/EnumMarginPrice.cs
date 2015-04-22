﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public enum QSEnumMarginPrice
    {
        /// <summary>
        /// 昨结算价
        /// </summary>
        PreSettlementPrice = 1,

        /// <summary>
        /// 最新价格
        /// </summary>
        TradePrice = 2,

        /// <summary>
        /// 成交均价
        /// </summary>
        AveragePrice = 3,

        /// <summary>
        /// 开仓价格
        /// </summary>
        OpenPrice = 4,
    }
}