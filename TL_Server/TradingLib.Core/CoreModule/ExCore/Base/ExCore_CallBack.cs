using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ExCore
    {

        protected virtual void NotifyTick(Tick k)
        { 
        
        }

        protected virtual void NotifyOrderActionError(OrderAction a, RspInfo e)
        { 
            
        }

        protected virtual void NotifyOrderError(Order o, RspInfo e)
        { 
        
        }

        protected virtual void NotifyOrder(Order o)
        { 
        
        }

        protected virtual void NotifyFill(Trade f)
        { 
        
        }

        protected virtual void NotifyPositionUpdate(Position pos)
        { 
        
        }
        protected virtual void NotifyCancel(long oid)
        { 
            
        }



    }
}
