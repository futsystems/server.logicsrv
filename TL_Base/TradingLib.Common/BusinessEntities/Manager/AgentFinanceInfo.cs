using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class AgentFinanceInfo
    {
        public string Account { get; set; }

        public CurrencyType Currency { get; set; }

        public decimal RealizedPL { get; set; }

        public decimal UnRealizedPL { get; set; }

        public decimal CommissioinIncome { get; set; }

        public decimal CommissionCost { get; set; }

        public decimal Margin { get; set; }


        public decimal MarginFrozen { get; set; }

        public decimal LastEquity { get; set; }

        public decimal CashIn { get; set; }

        public decimal CashOut { get; set; }

        public decimal NowEquity { get; set; }

        public decimal StaticEquity { get; set; }

        public decimal SubStaticEquity { get; set; }

        public decimal LastCredit { get; set; }

        public decimal CreditCashIn { get; set; }

        public decimal CreditCashOut { get; set; }

        public decimal NowCredit { get; set; }

        public decimal Profit { get; set; }

    }
}
