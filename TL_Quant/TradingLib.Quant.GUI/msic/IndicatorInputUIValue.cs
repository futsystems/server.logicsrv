using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.GUI
{
    public class IndicatorInputUIValue
    {
        // Fields
        public Control Control;
        public SeriesInputValue Input;

        // Methods
        public IndicatorInputUIValue(Control control, SeriesInputValue input)
        {
            this.Control = control;
            this.Input = input;
        }
    }


}
