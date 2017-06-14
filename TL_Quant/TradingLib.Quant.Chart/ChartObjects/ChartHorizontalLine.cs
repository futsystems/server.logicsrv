namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartHorizontalLine : ChartSingleLineBase
    {
        public ChartHorizontalLine(ChartPoint point)
        {
            base.points.Add(point);
            base.LineText = "yyyy-MM-dd";
            base.TextColor = Color.Red;
            base.TextFont = new Font("Verdana", 10f, FontStyle.Italic | FontStyle.Bold);
        }
    }
}

