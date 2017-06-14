using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    [Flags]
    public enum ServiceConnectOptions
    {
        Broker = 4,
        HistoricalData = 1,
        LiveData = 2,
        None = 0
    }

 

 

}
