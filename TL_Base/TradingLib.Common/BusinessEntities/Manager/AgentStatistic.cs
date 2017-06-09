using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class AgentStatistic
    {
        public string Account { get; set; }


        public decimal LastEquity { get; set; }

        public decimal LastCredit { get; set; }

        public decimal NowEquity { get; set; }//当前动态权益

        public decimal NowCredit { get; set; }//信用额度

        public decimal CommissionCost { get; set; }

        public decimal CommissioinIncome { get; set; }

        public decimal StaticEquity { get; set; }

        public decimal SubStaticEquity { get; set; }

        public decimal FlatEquity { get; set; }


        public decimal CustMargin { get; set; }//占用保证金

        public decimal CustForzenMargin { get; set; }//冻结保证金

        public decimal CustRealizedPL { get; set; }//平仓盈亏

        public decimal CustUnRealizedPL { get; set; }//浮动盈亏

        public decimal CustCashIn { get; set; }

        public decimal CustCashOut { get; set; }


    }
}
