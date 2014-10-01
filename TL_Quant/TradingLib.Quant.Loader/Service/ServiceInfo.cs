using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Quant;
using TradingLib.Quant.Plugin;

namespace TradingLib.Quant.Loader
{
    public class ServiceInfo : PluginInfo
    {
        // Fields
        public string AssemblyName = "";
        public bool BarDataAvailable;
        public bool BrokerFunctionsAvailable;
        public bool IsSimBroker;
        public bool SupportsMultipleInstances;
        public bool TickDataAvailable;
    }


}
