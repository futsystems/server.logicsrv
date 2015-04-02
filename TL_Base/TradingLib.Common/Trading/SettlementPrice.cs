using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /// <summary>
    /// 结算价
    /// </summary>
    public class SettlementPrice
    {

        /// <summary>
        /// 结算日
        /// </summary>
        public int SettleDay { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }


        /// <summary>
        /// 结算价
        /// </summary>
        public decimal Price { get; set; }
    }
}
