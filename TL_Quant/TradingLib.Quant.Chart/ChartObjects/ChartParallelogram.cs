namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartParallelogram : ChartTriangle
    {
        public ChartParallelogram(ChartPoint point1, ChartPoint point2, ChartPoint point3) : base(point1, point2, point3)
        {
        }

        public ChartParallelogram(ChartPoint point1, ChartPoint point2, ChartPoint point3, Color color) : base(point1, point2, point3, color)
        {
        }
    }
}

