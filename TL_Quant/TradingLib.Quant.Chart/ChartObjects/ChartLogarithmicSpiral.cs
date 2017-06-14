namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartLogarithmicSpiral : ChartSpiralObjectBase
    {
        public ChartLogarithmicSpiral(ChartPoint startPoint, ChartPoint endPoint) : base(startPoint, endPoint)
        {
        }

        public ChartLogarithmicSpiral(ChartPoint startPoint, ChartPoint endPoint, int sweepAngle) : base(startPoint, endPoint, sweepAngle)
        {
        }
    }
}

