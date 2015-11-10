using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace DataFarm
{
    public partial class TCPServiceHost:IServiceHost
    {
        public event Action<IServiceHost,IPacket> RequestEvent;

        void OnRequestEvent(IPacket packet)
        {
            if (RequestEvent != null)
            {
                RequestEvent(this,packet);
            }
        }
    }
}
