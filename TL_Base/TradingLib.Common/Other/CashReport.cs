using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /// <summary>
    /// 某个交易账户某日出入金统计
    /// 分为优先资金和劣后资金
    /// </summary>
    public class CashReport
    {
        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 入金
        /// </summary>
        public decimal CashIn { get; set; }

        /// <summary>
        /// 出金
        /// </summary>
        public decimal CashOut { get; set; }
    }

    /// <summary>
    /// 权益统计
    /// </summary>
    public class EquityReport
    {
        /// <summary>
        /// 交易账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 权益资金数额
        /// </summary>
        public decimal Equity { get; set; }

        /// <summary>
        /// 信用资金数额
        /// </summary>
        public decimal Credit { get; set; }
    }
}
