using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    public class BrokerAccountInfoEventArgs : EventArgs
    {

        /// <summary>
        /// Broker标识
        /// </summary>
        public string BrokerToken { get; set; }


        public BrokerAccountInfo AccountInfo { get; set; }
    }
}
