namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartParabolicSpiral : ChartSpiralObjectBase
    {
        public ChartParabolicSpiral(ChartPoint startPoint, ChartPoint endPoint) : base(startPoint, endPoint)
        {
        }

        public ChartParabolicSpiral(ChartPoint startPoint, ChartPoint endPoint, int sweepAngle) : base(startPoint, endPoint, sweepAngle)
        {
        }
    }
}

