using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{

    public interface IFrequencyProcessor
    {
        // Methods
        void CallSystemNewTick(Security symbol, Tick tick);
        void ProcessBarEvents(IEnumerable<FrequencyNewBarEventArgs> eventList);
        void ProcessTickInPaperBroker(Security symbol, Tick tick);
    }

}
