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
        public string Account { get; set; }
        public int SettleDay { get; set; }
        public int SettleTime { get; set; }

        public decimal RealizedPL { get; set; }
        public decimal UnRealizedPL { get; set; }
        public decimal Commission { get; set; }
        public decimal CashIn { get; set; }
        public decimal CashOut { get; set; }
        public decimal LastEqutiy { get; set; }
        public decimal NowEquity { get; set; }
        public bool Confirmed { get; set; }
    }


}
