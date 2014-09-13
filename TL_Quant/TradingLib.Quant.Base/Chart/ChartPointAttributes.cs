using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    public class ChartPointAttributes
    {
        // Fields
        private Color backgroundColor;
        private Bar endBar;
        private Color foregroundColor;
        private Bar startBar;
        private string text;
        private Font textFont;

        // Properties
        public Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                this.backgroundColor = value;
            }
        }

        public Bar EndBar
        {
            get
            {
                return this.endBar;
            }
            set
            {
                this.endBar = value;
            }
        }

        public Color ForegroundColor
        {
            get
            {
                return this.foregroundColor;
            }
            set
            {
                this.foregroundColor = value;
            }
        }

        public Bar StartBar
        {
            get
            {
                return this.startBar;
            }
            set
            {
                this.startBar = value;
            }
        }

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
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
