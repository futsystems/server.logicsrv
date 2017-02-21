using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.APIService
{
    public class GateWayConfig
    {
        public int ID { get; set; }

        public int Domain_ID { get; set; }

        public QSEnumGateWayType GateWayType { get; set; }

        public string Config { get; set; }

        public bool Avabile { get; set; }
    }
}
