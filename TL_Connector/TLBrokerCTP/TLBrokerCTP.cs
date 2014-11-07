using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;

namespace Broker.Live
{
    public class TLBrokerCTP:TLBroker
    {


        public override void SendOrder(Order o)
        {
            base.SendOrder(o);
        }
    }
}
