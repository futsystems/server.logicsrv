using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class SymbolSetup : SecurityFreq
    {
        // Fields
        public BarConstructionType BarConstruction;
        public string BrokerService;
        public string HistService;
        public string RealtimeService;
        public DateTime DownloadStartDate;
        public bool SaveLiveBars;
        public bool SaveLiveTicks;

        // Methods
        public SymbolSetup(Security symbol, BarFrequency frequency)
            : base(symbol, frequency)
        {
            this.HistService = "";
            this.RealtimeService = "";
            this.BrokerService = "";
            this.DownloadStartDate = DateTime.MinValue;
        }

        public SymbolSetup Clone()
        {
            SymbolSetup setup = (SymbolSetup)base.MemberwiseClone();
            setup.Security = new SecurityImpl(this.Security);
            return setup;
        }

        public override bool Equals(object obj)
        {
            if (obj is SymbolSetup)
            {
                if (!base.Equals(obj))
                {
                    return false;
                }
                SymbolSetup setup = (SymbolSetup)obj;
                if ((((this.HistService == setup.HistService) && (this.RealtimeService == setup.RealtimeService)) && ((this.BrokerService == setup.BrokerService) && (this.SaveLiveTicks == setup.SaveLiveTicks))) && (((this.SaveLiveBars == setup.SaveLiveBars) && (this.DownloadStartDate == setup.DownloadStartDate))))
                {
                    return true;
                }
            }
            return false;
        }

        /*
        /// <summary>
        /// 获得对应的FrequencyPlugin
        /// </summary>
        /// <returns></returns>
        public FrequencyPlugin GetFrequencyPlugin()
        {
            return new TimeFrequency { BarLength = TimeSpan.FromMinutes((double)base.Frequency) };
        }
        **/
        public override int GetHashCode()
        {
            return (((base.GetHashCode() ^ this.HistService.GetHashCode()) ^ this.RealtimeService.GetHashCode()) ^ this.BrokerService.GetHashCode());
        }
    }


}
