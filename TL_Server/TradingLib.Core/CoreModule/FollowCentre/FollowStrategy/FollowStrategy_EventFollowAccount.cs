using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Core
{
    
    public partial class FollowStrategy
    {
        void followAccount_GotOrderEvent(Order o)
        {

            //通过委托编号查找对应的TradeFollowItem
            TradeFollowItem item = sourceTracker[o.id];
            if (item != null)
            {
                item.GotOrder(o);
            }
        }

        void followAccount_GotFillEvent(Trade t)
        {
            TradeFollowItem item = sourceTracker[t.id];
            if (item != null)
            {
                item.GotTrade(t);
            }
        }
    }
}
