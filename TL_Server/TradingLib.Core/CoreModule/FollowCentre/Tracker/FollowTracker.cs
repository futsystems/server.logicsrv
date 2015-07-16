using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 跟单组件 全局访问对象
    /// </summary>
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
