using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.Drawing;
    using TradingLib.API;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartCross : ChartObjectBase
    {
        public SignalChartOperation EntryOrExit { get; set; }
        public SignalChartDirection LongOrShort { get; set; }
        protected Font labelFont = new Font("Verdana", 10f, GraphicsUnit.Point);
        protected string labelText;
        protected Color textColor = Color.Green;

        EnumCross crosstype = EnumCross.None;

        public EnumCross CrossType { get { return crosstype; } }

        public ChartCross(ChartPoint point,EnumCross cross,string test)
        {
            base.points.Add(point);
            labelText = test;
            crosstype = cross;
        }

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

        public string Text
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


