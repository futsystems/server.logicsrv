namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartTriangle : ChartObjectBase
    {
        private Color fillColor;
        private bool filled;
        private byte fillTransparency;

        public ChartTriangle(ChartPoint point1, ChartPoint point2, ChartPoint point3)
        {
            this.fillColor = Color.Beige;
            base.points.Add(point1);
            base.points.Add(point2);
            base.points.Add(point3);
        }

        public ChartTriangle(ChartPoint point1, ChartPoint point2, ChartPoint point3, Color color)
        {
            this.fillColor = Color.Beige;
            base.points.Add(point1);
            base.points.Add(point2);
            base.points.Add(point3);
            base.color = color;
        }

        [DisplayName("Fill Color"), Description("Specifies the fill color of this triangle or parallelogram."), Category("Fill")]
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

        [Category("Fill"), Description("Specifies whether or not to fill this triangle or parallelogram.")]
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

        [DisplayName("Fill Transparency"), Category("Fill"), Description("Specifies the level of transparency for this fill.  0 = completely transparent, 255 = completely filled.")]
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

