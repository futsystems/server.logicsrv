using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.ChartObjects
{

    using System;
    using System.Drawing;
    using TradingLib.Quant.Base;

    [Serializable]
    public class ChartSignal : ChartObjectBase
    {
        public SignalChartOperation EntryOrExit { get; set; }
        public SignalChartDirection LongOrShort { get; set; }
        protected Font labelFont = new Font("Verdana", 10f, GraphicsUnit.Point);
        protected string labelText;
        protected Color textColor = Color.Green;


        public ChartSignal(ChartPoint point,SignalChartDirection dircetion,SignalChartOperation operation,string price)
        {
            base.points.Add(point);
            EntryOrExit = operation;
            LongOrShort = dircetion;
            labelText = price;
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

        public string PriceText
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


