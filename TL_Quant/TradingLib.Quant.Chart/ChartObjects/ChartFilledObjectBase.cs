namespace TradingLib.Quant.ChartObjects
{
    using System;
    using System.ComponentModel;
    using System.Drawing;


    [Serializable]
    public class ChartFilledObjectBase : ChartObjectBase
    {
        private Color fillColor = Color.Beige;
        private bool filled;
        private byte fillTransparency;

        [DisplayName("Fill Color"), Description("Specifies the fill color of this rectangle."), Category("Fill")]
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

        [Description("Specifies whether or not to fill this shape."), Category("Fill")]
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

        [Category("Fill"), Description("Specifies the level of transparency for this fill.  0 = completely transparent, 255 = completely filled."), DisplayName("Fill Transparency")]
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

