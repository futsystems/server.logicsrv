using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class SingleBarEventArgs : EventArgs
    {
        // Methods
        public SingleBarEventArgs(Security symbol, Bar bar, DateTime barEndTime, bool ticksWereSent)
        {
            if ((barEndTime == DateTime.MinValue) || (barEndTime == DateTime.MaxValue))
            {
                throw new ArgumentException("Invalid barEndTime value: " + barEndTime);
            }
            if (bar == null)
            {
                throw new ArgumentNullException("bar");
            }
            this.Symbol = symbol;
            this.Bar = bar;
            this.BarEndTime = barEndTime;
            this.TicksWereSent = ticksWereSent;
        }

        public override string ToString()
        {
            return (this.Symbol.ToString() + " " + this.Bar.BarStartTime.ToString() + " " + this.BarEndTime.ToString());
        }

        // Properties
        public Bar Bar { get; private set; }

        public DateTime BarEndTime { get; private set; }

        public DateTime BarStartTime
        {
            get
            {
                return this.Bar.BarStartTime;
            }
        }

        public Security Symbol { get; private set; }

        public bool TicksWereSent { get; private set; }
    }


}
