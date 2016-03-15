using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.BrokerXAPI
{
    public static class StructUtils
    {
        public static string InfoStr(this XOrderField order)
        {
            return string.Format("XOrder LocalID:{1} RemoteID:{2} Total:{3} Filled:{4} Status:{5}", order.ID, order.BrokerLocalOrderID, order.BrokerRemoteOrderID, order.TotalSize, order.FilledSize, order.OrderStatus);
        }
    }
}
