using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class RunConfig
    {

        static RunConfig instance;

        static RunConfig()
        {
            instance = new RunConfig();

        }

        public static RunConfig Instance { get { return instance; } }

        private RunConfig()
        {
            this.MGRAccountStatistic = true;
            this.MGRAgentStatistic = true;

            this.Profile = new Profiler();
        }

        public  bool MGRAccountStatistic { get; set; }

        public  bool MGRAgentStatistic { get; set; }

        public  Profiler Profile { get; set; }
    }
}
