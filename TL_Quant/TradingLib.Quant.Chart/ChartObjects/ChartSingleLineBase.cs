namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartSingleLineBase : ChartObjectBase
    {
        private ChartTextLineAlignment lineAlignment;
        private string lineText;
        private bool openEnd;
        private bool openStart;
        private ChartLineTextAlignment textAlignment;
        private Color textColor;
        private Font textFont;

        public ChartSingleLineBase()
        {
            this.OpenStart = true;
            this.OpenEnd = true;
            this.textAlignment = ChartLineTextAlignment.Right;
            this.lineAlignment = ChartTextLineAlignment.Middle;
        }

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

        public bool OpenEnd
        {
            get
            {
                return this.openEnd;
            }
            set
            {
                this.openEnd = value;
            }
        }

        public bool OpenStart
        {
            get
            {
                return this.openStart;
            }
            set
            {
                this.openStart = value;
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

