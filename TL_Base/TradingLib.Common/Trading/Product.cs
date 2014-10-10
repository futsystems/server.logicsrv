using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public CurrencyType Currency { get; set; }
        public SecurityType Type { get; set; }
        public int Multiple { get; set; }
        public decimal Pricetick { get; set; }
        public decimal EntryCommission { get; set; }
        public decimal ExitCommission { get; set; }
        public decimal Margin { get; set; }
        public decimal ExtraMargin { get; set; }
        public decimal MaintanceMargin { get; set; }

    }
}
