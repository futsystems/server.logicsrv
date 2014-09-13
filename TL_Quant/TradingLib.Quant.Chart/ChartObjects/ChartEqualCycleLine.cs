namespace TradingLib.Quant.ChartObjects
{
    
    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartEqualCycleLine : ChartCycleLineBase
    {
        public ChartEqualCycleLine(ChartPoint point1, ChartPoint point2)
        {
            base.points.Add(point1);
            base.points.Add(point2);
            base.LineText = "{0:f2} {1:p1}";
            base.MaxLines = 20;
        }
    }
}

