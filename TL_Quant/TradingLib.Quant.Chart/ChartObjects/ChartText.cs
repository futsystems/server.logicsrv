namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartText : ChartTextBase
    {
        public ChartText(ChartPoint point, string text)
        {
            base.points.Add(point);
            base.labelText = text;
            base.LabelFont = new Font("Verdana", 40f);
            base.Alpha = 100;
            base.Color = Color.Black;
        }

        public ChartText(ChartPoint point, string text, Color textColor)
        {
            base.points.Add(point);
            base.labelText = text;
            base.textColor = textColor;
            base.LabelFont = new Font("Verdana", 40f);
            base.Alpha = 100;
        }
    }
}

