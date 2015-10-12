using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class AccountSettlement
    {
        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 昨日权益
        /// </summary>
        public decimal LastEquity { get; set; }

        /// <summary>
        /// 昨日信用额度
        /// </summary>
        public decimal LastCredit { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public decimal CloseProfitByDate { get; set; }

        /// <summary>
        /// 盯市持仓盈亏
        /// </summary>
        public decimal PositionProfitByDate { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// 入金
        /// </summary>
        public decimal CashIn { get; set; }

        /// <summary>
        /// 出金
        /// </summary>
        public decimal CashOut { get; set; }

        /// <summary>
        /// 信用入金
        /// </summary>
        public decimal CreditCashIn { get; set; }

        /// <summary>
        /// 信用出金
        /// </summary>
        public decimal CreditCashOut { get; set; }


        /// <summary>
        /// 结算权益
        /// </summary>
        public decimal EquitySettled { get; set; }

        /// <summary>
        /// 结算信用额度
        /// </summary>
        public decimal CreditSettled { get; set; }
    }
}
