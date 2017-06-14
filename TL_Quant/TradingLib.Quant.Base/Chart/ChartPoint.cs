using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public struct ChartPoint
    {
        private DateTime dateX;
        private double valueY;
        private bool empty;
        public DateTime DateX
        {
            get
            {
                return this.dateX;
            }
            set
            {
                this.dateX = value;
            }
        }

        public double ValueY
        {
            get
            {
                return this.valueY;
            }
            set
            {
                this.valueY = value;
            }
        }
        public bool Empty
        {
            get
            {
                return this.empty;
            }
            set
            {
                this.empty = value;
            }
        }
        public ChartPoint(DateTime dateX, double valueY)
        {
            this.empty = false;
            this.dateX = dateX;
            this.valueY = valueY;
        }

        public ChartPoint(bool empty)
        {
            this.empty = true;
            this.dateX = DateTime.MinValue;
            this.valueY = double.NaN;
        }
    }

}
