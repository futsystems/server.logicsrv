namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartRectanglePrice : ChartRoundedRectangle
    {
        public ChartRectanglePrice(ChartPoint leftPoint, ChartPoint rightPoint) : base(leftPoint, rightPoint)
        {
            this.Initialize();
        }

        public ChartRectanglePrice(ChartPoint leftPoint, ChartPoint rightPoint, Color color) : base(leftPoint, rightPoint, color)
        {
            this.Initialize();
        }

        public ChartRectanglePrice(ChartPoint leftPoint, ChartPoint rightPoint, Color color, int width, int roundWidth) : base(leftPoint, rightPoint, color)
        {
            this.Initialize();
            base.roundWidth = roundWidth;
        }

        private void Initialize()
        {
            base.roundWidth = 0;
            base.FillColor = Color.DarkKhaki;
            base.Filled = true;
            base.FillTransparency = 40;
        }
    }
}

