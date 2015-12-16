using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface SettlementPrice
    {
        /// <summary>
        /// 合约
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// 交易所
        /// </summary>
        string Exchange { get; set; }


        /// <summary>
        /// 结算价格
        /// </summary>
        decimal Settlement { get; set; }
    }
}
