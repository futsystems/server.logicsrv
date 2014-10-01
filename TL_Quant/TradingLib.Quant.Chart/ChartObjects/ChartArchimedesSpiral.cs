using TradingLib.Quant.Chart;

namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartArchimedesSpiral : ChartSpiralObjectBase
    {
        public ChartArchimedesSpiral(ChartPoint startPoint, ChartPoint endPoint) : base(startPoint, endPoint)
        {
        }

        public ChartArchimedesSpiral(ChartPoint startPoint, ChartPoint endPoint, int sweepAngle) : base(startPoint, endPoint, sweepAngle)
        {
        }
    }
}

