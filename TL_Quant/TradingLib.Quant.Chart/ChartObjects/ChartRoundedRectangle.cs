namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartRoundedRectangle : ChartRectangle
    {
        protected int roundWidth;

        public ChartRoundedRectangle(ChartPoint leftPoint, ChartPoint rightPoint) : base(leftPoint, rightPoint)
        {
            this.roundWidth = 20;
            this.Initialize();
        }

        public ChartRoundedRectangle(ChartPoint leftPoint, ChartPoint rightPoint, Color color) : base(leftPoint, rightPoint, color)
        {
            this.roundWidth = 20;
            this.Initialize();
        }

        public ChartRoundedRectangle(ChartPoint leftPoint, ChartPoint rightPoint, Color color, int width, int roundWidth) : base(leftPoint, rightPoint, color, width)
        {
            this.roundWidth = 20;
            this.Initialize();
            this.roundWidth = roundWidth;
        }

        private void Initialize()
        {
            this.roundWidth = 20;
        }

        [Category("Rounded"), Description("Specifies the number of pixels used in the rounding of the rectangle.")]
        public int RoundWidth
        {
            get
            {
                return this.roundWidth;
            }
            set
            {
                this.roundWidth = value;
            }
        }
    }
}

