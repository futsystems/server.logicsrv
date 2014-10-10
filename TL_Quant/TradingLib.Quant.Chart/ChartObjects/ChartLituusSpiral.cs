namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartLituusSpiral : ChartSpiralObjectBase
    {
        public ChartLituusSpiral(ChartPoint startPoint, ChartPoint endPoint) : base(startPoint, endPoint)
        {
        }

        public ChartLituusSpiral(ChartPoint startPoint, ChartPoint endPoint, int sweepAngle) : base(startPoint, endPoint, sweepAngle)
        {
        }
    }
}

