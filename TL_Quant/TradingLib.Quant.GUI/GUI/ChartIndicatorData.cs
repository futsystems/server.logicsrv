using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Chart
{

    public class ChartIndicatorData
    {
        // Fields
        public readonly ISeries indicator;
        public readonly IndicatorInfo info;

        // Methods
        public ChartIndicatorData(IndicatorInfo info, ISeries indicator)
        {
            this.info = info;
            this.indicator = indicator;
        }
    }


}
