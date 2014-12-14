//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;
//using TradingLib.API;

//namespace FutsMoniter
//{
//    partial class MainForm
//    {
//        /// <summary>
//        /// 注册回调函数
//        /// </summary>
//        /// <param name="module"></param>
//        /// <param name="cmd"></param>
//        /// <param name="del"></param>
//        public void RegisterCallback(string module, string cmd, JsonReplyDel del)
//        {
//            string key = module.ToUpper() + "-" + cmd.ToUpper();

//            if (!callbackmap.Keys.Contains(key))
//            {
//                callbackmap.TryAdd(key, new List<JsonReplyDel>());
//            }

//            callbackmap[key].Add(del);

//        }


//        /// <summary>
//        /// 注销回调函数
//        /// </summary>
//        /// <param name="module"></param>
//        /// <param name="cmd"></param>
//        /// <param name="del"></param>
//        public void UnRegisterCallback(string module, string cmd, JsonReplyDel del)
//        {
//            string key = module.ToUpper() + "-" + cmd.ToUpper();

//            if (!callbackmap.Keys.Contains(key))
//            {
//                callbackmap.TryAdd(key, new List<JsonReplyDel>());
//            }
//            if (callbackmap[key].Contains(del))
//            {
//                callbackmap[key].Remove(del);
//            }
//        }


//        ConcurrentDictionary<string, List<JsonReplyDel>> callbackmap = new ConcurrentDictionary<string, List<JsonReplyDel>>();
//        /// <summary>
//        /// 响应服务端的扩展回报 通过扩展模块ID 操作码 以及具体的json回报内容
//        /// </summary>
//        /// <param name="module"></param>
//        /// <param name="cmd"></param>
//        /// <param name="result"></param>
//        public void OnMGRContribResponse(string module, string cmd, string result)
//        {
//            string key = module.ToUpper() + "-" + cmd.ToUpper();
//            if (callbackmap.Keys.Contains(key))
//            {
//                foreach (JsonReplyDel del in callbackmap[key])
//                {
//                    try
//                    {
//                        del(result);
//                    }
//                    catch (Exception ex)
//                    {
//                        Globals.Debug("run callback error:" + ex.ToString());
//                    }
//                }
//            }
//            else
//            {
//                Globals.Debug("do not have any callback for " + key + " registed!");
//            }
//        }

//        /// <summary>
//        /// 响应服务端的通知回报
//        /// </summary>
//        /// <param name="module"></param>
//        /// <param name="cmd"></param>
//        /// <param name="result"></param>
//        public void OnMGRContribNotify(string module, string cmd, string result)
//        {
//            string key = module.ToUpper() + "-" + cmd.ToUpper();
//            if (callbackmap.Keys.Contains(key))
//            {
//                foreach (JsonReplyDel del in callbackmap[key])
//                {
//                    try
//                    {
//                        del(result);
//                    }
//                    catch (Exception ex)
//                    {
//                        Globals.Debug("run callback error:" + ex.ToString());
//                    }
//                }
//            }
//            else
//            {
//                Globals.Debug("do not have any callback for " + key + " registed!");
//            }
//        }

//    }
//}
