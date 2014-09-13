namespace TradingLib.Quant.ChartObjects
{
    
    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartEllipse : ChartFilledObjectBase
    {
        public ChartEllipse(ChartPoint leftPoint, ChartPoint centerPoint)
        {
            base.points.Add(leftPoint);
            base.points.Add(centerPoint);
        }

        public ChartEllipse(ChartPoint leftPoint, ChartPoint centerPoint, Color color)
        {
            base.points.Add(leftPoint);
            base.points.Add(centerPoint);
            base.color = color;
        }

        public ChartEllipse(ChartPoint leftPoint, ChartPoint centerPoint, Color color, int width)
        {
            base.points.Add(leftPoint);
            base.points.Add(centerPoint);
            base.color = color;
            base.width = width;
        }
    }
}

