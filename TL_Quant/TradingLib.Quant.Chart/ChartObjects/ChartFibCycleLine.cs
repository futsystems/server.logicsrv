namespace TradingLib.Quant.ChartObjects
{
    
    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartFibCycleLine : ChartCycleLineBase
    {
        public ChartFibCycleLine(ChartPoint point)
        {
            base.points.Add(point);
            base.MaxLines = 20;
        }
    }
}

