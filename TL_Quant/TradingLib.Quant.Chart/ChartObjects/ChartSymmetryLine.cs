namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartSymmetryLine : ChartCycleLineBase
    {
        public ChartSymmetryLine(ChartPoint point1, ChartPoint point2)
        {
            base.points.Add(point1);
            base.points.Add(point2);
            base.MaxLines = 20;
        }
    }
}

