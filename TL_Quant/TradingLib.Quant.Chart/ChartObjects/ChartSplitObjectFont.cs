namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartSplitObjectFont : ChartSplitObjectBase
    {
        private string lineText;
        private bool openEnd;
        private bool openStart;
        private ChartLineTextAlignment textAlignment;
        private Font textFont;

        public ChartSplitObjectFont(ChartPoint point1, ChartPoint point2) : base(point1, point2)
        {
            this.Initialize();
        }

        public ChartSplitObjectFont(ChartPoint point1, ChartPoint point2, string text) : base(point1, point2)
        {
            this.Initialize();
            this.lineText = text;
        }

        private void Initialize()
        {
            this.textAlignment = ChartLineTextAlignment.Left;
            this.openStart = false;
            this.openEnd = false;
            this.textFont = new Font("Verdana", 10f);
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

