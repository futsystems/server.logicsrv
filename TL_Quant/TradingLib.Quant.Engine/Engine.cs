using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;

namespace TradingLib.Quant.Engine
{
    public static class EngineHelper
    {
        public static PluginSettings CreateTimeFrequencySettings(BarFrequency f)
        {

            TimeFrequency frequency = new TimeFrequency(f);

            return PluginSettings.Create(frequency, typeof(FrequencyPlugin));
        }
    }
}
