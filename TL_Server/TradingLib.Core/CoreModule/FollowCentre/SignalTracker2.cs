//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using TradingLib.Common;

//namespace TradingLib.Core
//{
//    /// <summary>
//    /// 每个策略有一个单独的信号维护器用于维护信号对象
//    /// </summary>
//    public class SignalTracker
//    {
//        /// <summary>
//        /// 信号Token与信号对象的映射
//        /// </summary>
//        ConcurrentDictionary<string, ISignal> signalMap = new ConcurrentDictionary<string, ISignal>();

//        /// <summary>
//        /// 信号Token与跟单项目的映射
//        /// </summary>
//        ConcurrentDictionary<string, TradeFollowItemTracker> signalFollowItemMap = new ConcurrentDictionary<string, TradeFollowItemTracker>();

//        /// <summary>
//        /// 获得某个信号
//        /// </summary>
//        /// <param name="token"></param>
//        /// <returns></returns>
//        public ISignal GetSignal(string token)
//        {
//            if (string.IsNullOrEmpty(token))
//            {
//                return null;
//            }

//            ISignal signal = null;
//            if (signalMap.TryGetValue(token, out signal))
//            {
//                return signal;
//            }
//            return null;
//        }

//        /// <summary>
//        /// 获得某个token对应的跟单项维护器
//        /// </summary>
//        /// <param name="token"></param>
//        /// <returns></returns>
//        public TradeFollowItemTracker GetFollowItemTracker(string token)
//        {
//            TradeFollowItemTracker target = null;
//            if (signalFollowItemMap.TryGetValue(token, out target))
//            {
//                return target;
//            }
//            return null;
//        }


//        /// <summary>
//        /// 策略实例是否包含信号Signal
//        /// </summary>
//        /// <param name="signal"></param>
//        /// <returns></returns>
//        public bool HaveSignal(ISignal signal)
//        {
//            if (string.IsNullOrEmpty(signal.Token)) return false;
//            if (signalMap.Keys.Contains(signal.Token)) return true;
//            return false;
//        }

//         /// <summary>
//        /// 添加信号到策略对象
//        /// </summary>
//        public void AppendSignal(ISignal signal)
//        {
//            //1.添加信号到map
//            if (HaveSignal(signal))
//            {
//                return;
//            }
//            signalMap.TryAdd(signal.Token, signal);

//            signalFollowItemMap.TryAdd(signal.Token, new TradeFollowItemTracker());
//        }

//        /// <summary>
//        /// 从策略对象删除信号
//        /// </summary>
//        /// <param name="siginal"></param>
//        public void RemoveSignal(ISignal signal)
//        {
//            if (signal == null)
//            {
//                return;
//            }
//            //1.删除信号
//            if (!HaveSignal(signal))
//            {
//                return;
//            }
//            ISignal target = null;

//            signalMap.TryRemove(signal.Token, out target);

            
//        }
//    }
//}
