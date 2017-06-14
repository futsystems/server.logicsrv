using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    public class ChartObjectEventArgs : EventArgs
    {
        // Fields
        private IChartDisplay x013829fb81d13d20;
        private IChartObject xe77b8eecae8d2268;

        // Methods
        public ChartObjectEventArgs(IChartObject chartObject, IChartDisplay chartForm)
        {
            this.xe77b8eecae8d2268 = chartObject;
            this.x013829fb81d13d20 = chartForm;
        }

        // Properties
        public IChartDisplay ChartForm
        {
            get
            {
                return this.x013829fb81d13d20;
            }
            set
            {
                this.x013829fb81d13d20 = value;
            }
        }

        public IChartObject ChartObject
        {
            get
            {
                return this.xe77b8eecae8d2268;
            }
            set
            {
                this.xe77b8eecae8d2268 = value;
            }
        }
    }


}
