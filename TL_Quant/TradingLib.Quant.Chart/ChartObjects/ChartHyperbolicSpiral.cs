namespace TradingLib.Quant.ChartObjects
{
    
    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartHyperbolicSpiral : ChartSpiralObjectBase
    {
        public ChartHyperbolicSpiral(ChartPoint startPoint, ChartPoint endPoint) : base(startPoint, endPoint)
        {
        }

        public ChartHyperbolicSpiral(ChartPoint startPoint, ChartPoint endPoint, int sweepAngle) : base(startPoint, endPoint, sweepAngle)
        {
        }
    }
}

