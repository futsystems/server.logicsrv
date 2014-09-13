using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class BrokerAccountState
    {
        // Methods
        public BrokerAccountState()
        {
            this.Positions = new List<Position>();
            this.PendingOrders = new List<Order>();
        }

        // Properties
        public bool BrokerOverride { get; set; }

        public List<Order> PendingOrders { get; set; }

        public List<Position> Positions { get; set; }
    }

 

}
