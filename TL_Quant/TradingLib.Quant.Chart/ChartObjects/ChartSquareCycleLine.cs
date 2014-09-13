namespace TradingLib.Quant.ChartObjects
{

    using System;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartSquareCycleLine : ChartCycleLineBase
    {
        public ChartSquareCycleLine(ChartPoint point)
        {
            base.points.Add(point);
            base.MaxLines = 20;
        }
    }
}

