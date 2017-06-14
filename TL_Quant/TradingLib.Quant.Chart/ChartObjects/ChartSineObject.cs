namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartSineObject : ChartObjectBase
    {
        public ChartSineObject(ChartPoint topPoint, ChartPoint bottomPoint)
        {
            base.points.Add(topPoint);
            base.points.Add(bottomPoint);
        }

        public ChartSineObject(ChartPoint topPoint, ChartPoint bottomPoint, Color color)
        {
            base.points.Add(topPoint);
            base.points.Add(bottomPoint);
            this.SetColor(color);
        }
    }
}

