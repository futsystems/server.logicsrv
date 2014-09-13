using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface Settlement
    {
        string Account { get; set; }
        int SettleDay { get; set; }
        int SettleTime { get; set; }
        decimal RealizedPL { get; set; }
        decimal UnRealizedPL { get; set; }
        decimal Commission { get; set; }
        decimal CashIn { get; set; }
        decimal CashOut { get; set; }
        decimal LastEqutiy { get; set; }
        decimal NowEquity { get; set; }
        bool Confirmed { get; set; }
    }
}
