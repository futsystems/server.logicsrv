namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartRectangle : ChartObjectBase
    {
        private Color fillColor;
        private bool filled;
        private byte fillTransparency;

        public ChartRectangle(ChartPoint leftPoint, ChartPoint rightPoint)
        {
            this.fillColor = Color.Beige;
            base.points.Add(leftPoint);
            base.points.Add(rightPoint);
        }

        public ChartRectangle(ChartPoint leftPoint, ChartPoint rightPoint, Color color)
        {
            this.fillColor = Color.Beige;
            base.points.Add(leftPoint);
            base.points.Add(rightPoint);
            base.color = color;
        }

        public ChartRectangle(ChartPoint leftPoint, ChartPoint rightPoint, Color color, int width)
        {
            this.fillColor = Color.Beige;
            base.points.Add(leftPoint);
            base.points.Add(rightPoint);
            base.color = color;
            base.width = width;
        }

        [Category("Fill"), Description("Specifies the fill color of this rectangle."), DisplayName("Fill Color")]
        public Color FillColor
        {
            get
            {
                return this.fillColor;
            }
            set
            {
                this.fillColor = value;
            }
        }

        [Category("Fill"), Description("Specifies whether or not to fill this rectangle.")]
        public bool Filled
        {
            get
            {
                return this.filled;
            }
            set
            {
                this.filled = value;
            }
        }

        [Category("Fill"), DisplayName("Fill Transparency"), Description("Specifies the level of transparency for this fill.  0 = completely transparent, 255 = completely filled.")]
        public byte FillTransparency
        {
            get
            {
                return this.fillTransparency;
            }
            set
            {
                this.fillTransparency = value;
            }
        }
    }
}

