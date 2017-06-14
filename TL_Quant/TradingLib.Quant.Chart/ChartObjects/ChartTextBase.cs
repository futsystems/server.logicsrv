using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace TradingLib.Quant.ChartObjects
{
    [Serializable]
    public class ChartTextBase : ChartObjectBase
    {
        // Fields
        protected Font labelFont = new Font("Verdana", 10f, GraphicsUnit.Point);
        protected string labelText;
        protected Color textColor = Color.Black;

        // Properties
        public Font LabelFont
        {
            get
            {
                return this.labelFont;
            }
            set
            {
                this.labelFont = value;
            }
        }

        public string LabelText
        {
            get
            {
                return this.labelText;
            }
            set
            {
                this.labelText = value;
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
    }


}
