using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using TradingLib.Quant.Base;


namespace TradingLib.Quant.ChartObjects
{
    [Serializable]
    public class ChartLabel : ChartTextBase
    {
        // Fields
        private ChartLabelAlignment labelAlignment;
        private byte labelTransparency;
        private int roundWidth;
        private int shadowWidth;
        private int stickLength;
        private Color textBackgroundColor;

        // Methods
        public ChartLabel(ChartPoint point, string text)
        {
            this.textBackgroundColor = Color.Yellow;
            this.labelTransparency = 0xff;
            this.roundWidth = 2;
            this.shadowWidth = 2;
            this.stickLength = 6;
            base.points.Add(point);
            base.labelText = text;
        }

        public ChartLabel(ChartPoint point, string text, Color textColor)
        {
            this.textBackgroundColor = Color.Yellow;
            this.labelTransparency = 0xff;
            this.roundWidth = 2;
            this.shadowWidth = 2;
            this.stickLength = 6;
            base.points.Add(point);
            base.labelText = text;
            base.textColor = textColor;
            
        }

        public ChartLabel(ChartPoint point, string text, Color textColor, Color backgroundColor)
        {
            this.textBackgroundColor = Color.Yellow;
            this.labelTransparency = 0xff;
            this.roundWidth = 2;
            this.shadowWidth = 2;
            this.stickLength = 6;
            base.points.Add(point);
            base.labelText = text;
            base.textColor = textColor;
            this.textBackgroundColor = backgroundColor;
        }

        // Properties
        public ChartLabelAlignment LabelAlignment
        {
            get
            {
                return this.labelAlignment;
            }
            set
            {
                this.labelAlignment = value;
            }
        }

        public byte LabelTransparency
        {
            get
            {
                return this.labelTransparency;
            }
            set
            {
                this.labelTransparency = value;
            }
        }

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

        public int ShadowWidth
        {
            get
            {
                return this.shadowWidth;
            }
            set
            {
                this.shadowWidth = value;
            }
        }

        public int StickLength
        {
            get
            {
                return this.stickLength;
            }
            set
            {
                this.stickLength = value;
            }
        }

        public Color TextBackgroundColor
        {
            get
            {
                return this.textBackgroundColor;
            }
            set
            {
                this.textBackgroundColor = value;
            }
        }
    }

 

}
