namespace TradingLib.Quant.ChartObjects
{
    
    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartArrowLine : ChartObjectBase
    {
        public ChartArrowLine(ChartPoint startPoint, ChartPoint endPoint)
        {
            base.points.Add(startPoint);
            base.points.Add(endPoint);
            this.InitializeCaps();
        }

        public ChartArrowLine(ChartPoint startPoint, ChartPoint endPoint, Color lineColor)
        {
            base.points.Add(startPoint);
            base.points.Add(endPoint);
            base.color = lineColor;
            this.InitializeCaps();
        }

        public ChartArrowLine(ChartPoint startPoint, ChartPoint endPoint, Color lineColor, int width)
        {
            base.points.Add(startPoint);
            base.points.Add(endPoint);
            base.color = lineColor;
            base.width = width;
            this.InitializeCaps();
        }

        public void InitializeCaps()
        {
            base.endCap = new ChartCap(10, 10, false);
        }
    }
}

