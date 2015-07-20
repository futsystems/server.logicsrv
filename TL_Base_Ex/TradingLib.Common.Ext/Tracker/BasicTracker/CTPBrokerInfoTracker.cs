using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class CTPBrokerInfoTracker
    {
        ConcurrentDictionary<int, CTPBrokerInfo> ctpbrokerinfomap = new ConcurrentDictionary<int, CTPBrokerInfo>();

        public CTPBrokerInfoTracker()
        {
            foreach (var info in ORM.MCTPBrokerInfo.SelectCTPBrokerInfos())
            {
                ctpbrokerinfomap.TryAdd(info.ID, info);
            }
        }


        public IEnumerable<CTPBrokerInfo> CTPBrokerInfos
        {
            get
            {
                return ctpbrokerinfomap.Values;
            }
        }
    }
}
