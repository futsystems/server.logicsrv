namespace TradingLib.Quant.ChartObjects
{
    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartCycleLineBase : ChartObjectBase
    {
        private ChartTextLineAlignment lineAlignment;
        private string lineText;
        private int maxLines = 40;
        private bool showText = true;
        private ChartLineTextAlignment textAlignment = ChartLineTextAlignment.Middle;
        private Color textColor = Color.Black;
        private Font textFont = new Font("Verdana", 10f, FontStyle.Italic);

        public ChartTextLineAlignment LineAlignment
        {
            get
            {
                return this.lineAlignment;
            }
            set
            {
                this.lineAlignment = value;
            }
        }

        public string LineText
        {
            get
            {
                return this.lineText;
            }
            set
            {
                this.lineText = value;
            }
        }

        public int MaxLines
        {
            get
            {
                return this.maxLines;
            }
            set
            {
                this.maxLines = value;
            }
        }

        public bool ShowText
        {
            get
            {
                return this.showText;
            }
            set
            {
                this.showText = value;
            }
        }

        public ChartLineTextAlignment TextAlignment
        {
            get
            {
                return this.textAlignment;
            }
            set
            {
                this.textAlignment = value;
            }
        }

        public Color TextColor
        {
            get
            {
                return this.textColor;
            }
            set
            {
                this.textColor = value;
            }
        }

        public Font TextFont
        {
            get
            {
                return this.textFont;
            }
            set
            {
                this.textFont = value;
            }
        }
    }
}

