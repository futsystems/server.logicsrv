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

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            defaultinstance.signalTracker = new SignalTracker();
            defaultinstance.strategyTracker = new FollowStrategyCfgTracker();
            defaultinstance._followItemLogger = new AsyncFollowLoger();
            defaultinstance._followItemLogger.Start();
            Inited = false;
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

        FollowStrategyCfgTracker strategyTracker = null;
        /// <summary>
        /// 策略配置维护其
        /// </summary>
        public static FollowStrategyCfgTracker StrategyCfgTracker
        {
            get
            {
                if (defaultinstance.strategyTracker == null)
                    defaultinstance.strategyTracker = new FollowStrategyCfgTracker();
                return defaultinstance.strategyTracker;
            }
        }

        AsyncFollowLoger _followItemLogger = null;
        /// <summary>
        /// 跟单项记录器
        /// </summary>
        public static AsyncFollowLoger FollowItemLogger
        {
            get
            {
                if (defaultinstance._followItemLogger == null)
                    defaultinstance._followItemLogger = new AsyncFollowLoger();
                return defaultinstance._followItemLogger;
            }
        }


        FollowStrategyTracker _followStrategyTracker = null;
        /// <summary>
        /// 跟单实例维护其 
        /// </summary>
        public static FollowStrategyTracker FollowStrategyTracker
        {
            get
            {
                if (defaultinstance._followStrategyTracker == null)
                {
                    defaultinstance._followStrategyTracker = new FollowStrategyTracker();
                }
                return defaultinstance._followStrategyTracker;
            }
        }

        public static event Action<FollowItem> NotifyFollowItemEvent;


        /// <summary>
        /// 对外通知跟单项目的状态变化
        /// </summary>
        /// <param name="item"></param>
        public static void NotifyFollowItem(FollowItem item)
        {
            if (NotifyFollowItemEvent != null)
            {
                NotifyFollowItemEvent(item);
            }
            
        }

        static IdTracker _followIDTracker = new IdTracker();
        /// <summary>
        /// 跟单编号生成器
        /// </summary>
        public static string NextFollowKey
        {
            get
            {
                return _followIDTracker.AssignId.ToString();
            }
        }

        /// <summary>
        /// 是否初始化完毕
        /// </summary>
        public static bool Inited { get; set; }

    }
}
