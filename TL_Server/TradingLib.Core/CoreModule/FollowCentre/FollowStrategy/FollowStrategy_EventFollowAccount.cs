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
            OrderSource source = sourceTracker[o.id];
            if (source != null)
            {
                source.FollowItem.GotOrder(o);
            }
        }

        void followAccount_GotFillEvent(Trade t)
        {
            OrderSource source = sourceTracker[t.id];
            if (source != null)
            {
                source.FollowItem.GotTrade(t);
            }
        }
    }
}
