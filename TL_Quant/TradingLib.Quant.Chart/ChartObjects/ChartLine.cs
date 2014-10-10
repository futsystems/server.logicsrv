namespace TradingLib.Quant.ChartObjects
{
    
    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;


    [Serializable]
    public class ChartLine : ChartObjectBase
    {
        public ChartLine(ChartPoint startPoint, ChartPoint endPoint)
        {
            base.points.Add(startPoint);
            base.points.Add(endPoint);
        }

        public ChartLine(ChartPoint startPoint, ChartPoint endPoint, Color lineColor)
        {
            base.points.Add(startPoint);
            base.points.Add(endPoint);
            base.color = lineColor;
        }

        public ChartLine(ChartPoint startPoint, ChartPoint endPoint, Color lineColor, int width)
        {
            base.points.Add(startPoint);
            base.points.Add(endPoint);
            base.color = lineColor;
            base.width = width;
        }
    }
}

