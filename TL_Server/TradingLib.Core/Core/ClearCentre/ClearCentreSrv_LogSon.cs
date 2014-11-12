using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class ClearCentre
    {
        internal void LogOrder(Order o)
        {
            _asynLoger.newOrder(o);
        }

        internal void LogOrderUpdate(Order o)
        {
            _asynLoger.updateOrder(o);
        }
        internal void LogTrade(Trade f)
        {
            _asynLoger.newTrade(f);
        }


    }
}
