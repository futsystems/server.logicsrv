using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Common.Json;

using Common.Logging;


namespace TradingLib.MDClient
{
    /// <summary>
    /// 扩展类命令事件
    /// </summary>
    public class EventContrib
    {

        ILog logger = LogManager.GetLogger("EventContrib");

        public EventContrib()
        {
            
        }




        public event Action<string> DemoEvent;

        /// <summary>
        /// 演示事件
        /// </summary>
        /// <param name="msg"></param>
        public void FireDemo(string msg)
        {
            if (DemoEvent != null)
                DemoEvent(msg);
        }
        /// <summary>
        /// 注册Request回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="del"></param>
        public void RegisterCallback(string module, string cmd, Action<string,bool> del)
        {
            string key = module.ToUpper() + "-" + cmd.ToUpper();

            if (!callbackmap.Keys.Contains(key))
            {
                callbackmap.TryAdd(key, new List<Action<string, bool>>());
            }
            callbackmap[key].Add(del);

        }

        /// <summary>
        /// 注册Notify回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="del"></param>
        public void RegisterNotifyCallback(string module, string cmd, Action<string> del)
        {
            string key = module.ToUpper() + "-" + cmd.ToUpper();

            if (!notifycallbackmap.Keys.Contains(key))
            {
                notifycallbackmap.TryAdd(key, new List<Action<string>>());
            }

            notifycallbackmap[key].Add(del);

        }



        /// <summary>
        /// 注销Request回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="del"></param>
        public void UnRegisterCallback(string module, string cmd, Action<string, bool> del)
        {
            string key = module.ToUpper() + "-" + cmd.ToUpper();

            if (!callbackmap.Keys.Contains(key))
            {
                callbackmap.TryAdd(key, new List<Action<string, bool>>());
            }
            if (callbackmap[key].Contains(del))
            {
                callbackmap[key].Remove(del);
            }
        }

        /// <summary>
        /// 注销Notify回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="del"></param>
        public void UnRegisterNotifyCallback(string module, string cmd, Action<string> del)
        {
            string key = module.ToUpper() + "-" + cmd.ToUpper();

            if (!notifycallbackmap.Keys.Contains(key))
            {
                notifycallbackmap.TryAdd(key, new List<Action<string>>());
            }
            if (notifycallbackmap[key].Contains(del))
            {
                notifycallbackmap[key].Remove(del);
            }
        }


        ConcurrentDictionary<string, List<Action<string>>> notifycallbackmap = new ConcurrentDictionary<string, List<Action<string>>>();
        ConcurrentDictionary<string, List<Action<string, bool>>> callbackmap = new ConcurrentDictionary<string, List<Action<string, bool>>>();
        /// <summary>
        /// 响应服务端的扩展回报 通过扩展模块ID 操作码 以及具体的json回报内容
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="result"></param>
        internal void OnMGRContribResponse(RspMGRContribResponse response)
        {
            logger.Info("ModuleID:{0} CMDStr:{1} Result:{2} IsLast:{3}".Put(response.ModuleID, response.CMDStr, response.Result, response.IsLast));
            string key = response.ModuleID.ToUpper() + "-" + response.CMDStr.ToUpper();
            if (callbackmap.Keys.Contains(key))
            {
                foreach (Action<string, bool> del in callbackmap[key])
                {
                    try
                    {
                        del(response.Result, response.IsLast);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("ContribResponse Callback Error", ex);
                    }
                }
            }
            else
            {
                logger.Warn("do not have any callback for " + key + " registed!");
            }
        }

        /// <summary>
        /// 响应服务端的通知回报
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="result"></param>
        internal void OnMGRContribNotifyResponse(NotifyMGRContribNotify notify)
        {
            logger.Info("ModuleID:{0} CMDStr:{1} Result:{2}".Put(notify.ModuleID, notify.CMDStr, notify.Result));
            string key = notify.ModuleID.ToUpper() + "-" + notify.CMDStr.ToUpper();
            if (notifycallbackmap.Keys.Contains(key))
            {
                foreach (Action<string> del in notifycallbackmap[key])
                {
                    try
                    {
                        del(notify.Result);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("ContribNotifyResponse Callback Error", ex);
                    }
                }
            }
            else
            {
                logger.Warn("do not have any callback for " + key + " registed!");
            }
        }
    }
}
