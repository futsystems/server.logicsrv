using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class AgentStatistic
    {
        public string Account { get; set; }

        public decimal NowEquity { get; set; }//当前动态权益

        public decimal NowCredit { get; set; }//信用额度

        public decimal Margin { get; set; }//占用保证金

        public decimal ForzenMargin { get; set; }//冻结保证金

        public decimal RealizedPL { get; set; }//平仓盈亏

        public decimal UnRealizedPL { get; set; }//浮动盈亏

        public decimal CashIn { get; set; }

        public decimal CashOut { get; set; }

        public decimal CreditCashIn { get; set; }

        public decimal CreditCashOut { get; set; }

        public decimal CommissionCost { get; set; }

        public decimal CommissioinIncome { get; set; }


    }
}
