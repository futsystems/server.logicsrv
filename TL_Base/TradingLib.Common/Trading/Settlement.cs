using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 结算记录
    /// </summary>
    public class SettlementImpl:Settlement
    {
        public SettlementImpl()
        {
            this.Account = string.Empty;
            this.SettleDay = 0;
            this.SettleTime = 0;
            this.RealizedPL = 0M;
            this.UnRealizedPL = 0M;
            this.Commission = 0M;
            this.CashIn = 0M;
            this.CashOut = 0M;
            this.LastEquity = 0M;
            this.NowEquity = 0M;
            this.CreditCashIn = 0M;
            this.CreditCashOut = 0M;
            this.LastCredit = 0M;
            this.NowCredit = 0;
            this.Confirmed = false;
        }

        public string Account { get; set; }
        public int SettleDay { get; set; }
        public int SettleTime { get; set; }

        public decimal RealizedPL { get; set; }
        public decimal UnRealizedPL { get; set; }
        public decimal Commission { get; set; }
        public decimal CashIn { get; set; }
        public decimal CashOut { get; set; }
        public decimal LastEquity { get; set; }
        public decimal NowEquity { get; set; }
        public decimal CreditCashIn { get; set; }
        public decimal CreditCashOut { get; set; }
        public decimal LastCredit { get; set; }
        public decimal NowCredit { get; set; }
        public bool Confirmed { get; set; }
    }


}
