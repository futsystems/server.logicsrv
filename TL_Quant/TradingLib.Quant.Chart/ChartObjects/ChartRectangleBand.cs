namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartRectangleBand : ChartRectangle
    {
        public ChartRectangleBand(ChartPoint leftPoint, ChartPoint rightPoint) : base(leftPoint, rightPoint)
        {
            this.Initialize();
        }

        public ChartRectangleBand(ChartPoint leftPoint, ChartPoint rightPoint, Color color) : base(leftPoint, rightPoint, color)
        {
            this.Initialize();
        }

        public ChartRectangleBand(ChartPoint leftPoint, ChartPoint rightPoint, Color color, int width) : base(leftPoint, rightPoint, color, width)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            base.Filled = true;
            base.FillColor = Color.DarkKhaki;
            base.FillTransparency = 40;
        }
    }
}

