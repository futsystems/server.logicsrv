using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.GUI
{
    public class IndicatorUIValue
    {
        // Fields
        private ConstructorArgument constructarg;
        private Control uicontrol;

        // Methods
        public IndicatorUIValue(Control control, ConstructorArgument arg)
        {
            this.uicontrol = control;
            this.constructarg = arg;
        }

        // Properties
        public ConstructorArgument Argument
        {
            get
            {
                return this.constructarg;
            }
            set
            {
                this.constructarg = value;
            }
        }

        public Control Control
        {
            get
            {
                return this.uicontrol;
            }
            set
            {
                this.uicontrol = value;
            }
        }
    }


}