namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartCircle : ChartFilledObjectBase
    {
        public ChartCircle(ChartPoint centerPoint, ChartPoint outerPoint)
        {
            base.points.Add(centerPoint);
            base.points.Add(outerPoint);
        }

        public ChartCircle(ChartPoint centerPoint, ChartPoint outerPoint, Color color)
        {
            base.points.Add(centerPoint);
            base.points.Add(outerPoint);
            base.color = color;
        }

        public ChartCircle(ChartPoint centerPoint, ChartPoint outerPoint, Color color, int width)
        {
            base.points.Add(centerPoint);
            base.points.Add(outerPoint);
            base.color = color;
            base.width = width;
        }
    }
}

