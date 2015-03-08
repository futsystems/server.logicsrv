using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using FutsMoniter;

namespace TradingLib.Common
{
    /// <summary>
    /// 消息处理中继,实现ILogicHandler,用于处理底层回报上来的消息
    /// 界面层订阅这里的事件 实现数据展示
    /// </summary>
    public partial class Ctx
    {

        /// <summary>
        /// 注册回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="del"></param>
        public void RegisterCallback(string module, string cmd, Action<string,bool> del)
        {
            string key = module.ToUpper() + "-" + cmd.ToUpper();

            if (!callbackmap.Keys.Contains(key))
            {
                callbackmap.TryAdd(key, new List<Action<string,bool>>());
            }

            callbackmap[key].Add(del);

        }

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
        /// 注销回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="del"></param>
        public void UnRegisterCallback(string module, string cmd, Action<string,bool> del)
        {
            string key = module.ToUpper() + "-" + cmd.ToUpper();

            if (!callbackmap.Keys.Contains(key))
            {
                callbackmap.TryAdd(key, new List<Action<string,bool>>());
            }
            if (callbackmap[key].Contains(del))
            {
                callbackmap[key].Remove(del);
            }
        }

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
        ConcurrentDictionary<string, List<Action<string,bool>>> callbackmap = new ConcurrentDictionary<string, List<Action<string,bool>>>();
        /// <summary>
        /// 响应服务端的扩展回报 通过扩展模块ID 操作码 以及具体的json回报内容
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="result"></param>
        public void OnMGRContribResponse(string module, string cmd, string result,bool islast)
        {
            string key = module.ToUpper() + "-" + cmd.ToUpper();
            if (callbackmap.Keys.Contains(key))
            {
                foreach (Action<string,bool> del in callbackmap[key])
                {
                    try
                    {
                        del(result,islast);
                    }
                    catch (Exception ex)
                    {
                        Globals.Debug("run callback error:" + ex.ToString());
                    }
                }
            }
            else
            {
                Globals.Debug("do not have any callback for " + key + " registed!");
            }
        }

        /// <summary>
        /// 响应服务端的通知回报
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="result"></param>
        public void OnMGRContribNotify(string module, string cmd, string result)
        {
            string key = module.ToUpper() + "-" + cmd.ToUpper();
            if (callbackmap.Keys.Contains(key))
            {
                foreach (Action<string> del in notifycallbackmap[key])
                {
                    try
                    {
                        del(result);
                    }
                    catch (Exception ex)
                    {
                        Globals.Debug("run callback error:" + ex.ToString());
                    }
                }
            }
            else
            {
                Globals.Debug("do not have any callback for " + key + " registed!");
            }
        }
    }
}
