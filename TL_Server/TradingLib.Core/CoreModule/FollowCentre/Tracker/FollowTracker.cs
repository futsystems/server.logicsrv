using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public class FollowTracker
    {
        static FollowTracker defaultinstance;

        static FollowTracker()
        {
            defaultinstance = new FollowTracker();
        }

        private FollowTracker()
        { 
        
        }

        SignalTracker signalTracker = null;
        /// <summary>
        /// 信号维护器
        /// </summary>
        public static SignalTracker SignalTracker
        {
            get
            {
                if (defaultinstance.signalTracker == null)
                    defaultinstance.signalTracker = new SignalTracker();
                return defaultinstance.signalTracker;
            }
        }
    }
}
