using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    public class FrequencyNewBarEventArgs : EventArgs
    {
        // Methods
        public FrequencyNewBarEventArgs()
        {
            this.FrequencyEvents = new Dictionary<FreqKey, SingleBarEventArgs>();
        }

        // Properties
        public Dictionary<FreqKey, SingleBarEventArgs> FrequencyEvents { get; set; }
    }

 

}
