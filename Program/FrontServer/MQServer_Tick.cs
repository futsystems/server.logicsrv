//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using ZeroMQ;
//using TradingLib.API;
//using TradingLib.Common;
//using TradingLib.XLProtocol;
//using TradingLib.XLProtocol.V1;
//using Common.Logging;


//namespace FrontServer
//{
//    public partial class MQServer
//    {

//        /// <summary>
//        /// 实时行情注册map
//        /// 合约唯一键值 与 Connection的映射关系
//        /// </summary>
//        ConcurrentDictionary<string, ConcurrentDictionary<string, IConnection>> symKeyRegMap = new ConcurrentDictionary<string, ConcurrentDictionary<string, IConnection>>();

//        /// <summary>
//        /// 行情快照维护器具
//        /// </summary>
//        TickTracker tickTracker = new TickTracker();

//        /// <summary>
//        /// 向客户端通知行情回报
//        /// </summary>
//        /// <param name="k"></param>
//        void NotifyTick2Connections(Tick k)
//        {
//            ConcurrentDictionary<string, IConnection> target = null;
//            if (symKeyRegMap.TryGetValue(k.GetSymbolUniqueKey(), out target))
//            {
//                foreach (var conn in target.Values)
//                {
//                    TickNotify ticknotify = new TickNotify();
//                    ticknotify.Tick = k;
//                    //logger.Info("send tick");
//                    conn.Send(ticknotify.Data);
//                }
//            }
//        }

//        /// <summary>
//        /// 注销某个连接的所有行情注册
//        /// </summary>
//        /// <param name="conn"></param>
//        public void ClearSymbolRegisted(string sessionID)
//        {
//            logger.Info(string.Format("Clear symbols registed for conn:{0}", sessionID));
//            IConnection target = null;
//            foreach (var regpair in symKeyRegMap)
//            {
//                if (regpair.Value.Keys.Contains(sessionID))
//                {
//                    regpair.Value.TryRemove(sessionID, out target);
//                }
//            }
//        }

//        /// <summary>
//        /// 某个连接注册合约行情
//        /// </summary>
//        /// <param name="conn"></param>
//        /// <param name="request"></param>
//        public void OnRegisterSymbol(IConnection conn, RegisterSymbolTickRequest request)
//        {
//            if (string.IsNullOrEmpty(request.Exchange))
//            {
//                logger.Warn("Register Symbol Tick Need Exhcnange");
//                return;
//            }
//            foreach (var symbol in request.SymbolList)
//            {
//                if (string.IsNullOrEmpty(symbol)) continue;
//                string key = string.Format("{0}-{1}", request.Exchange, symbol);

//                if (!symKeyRegMap.Keys.Contains(key))
//                {
//                    symKeyRegMap.TryAdd(key, new ConcurrentDictionary<string, IConnection>());
//                }
//                ConcurrentDictionary<string, IConnection> regmap = symKeyRegMap[key];
//                if (!regmap.Keys.Contains(conn.SessionID))
//                {
//                    regmap.TryAdd(conn.SessionID, conn);
//                    //客户端订阅后发送当前市场快照
//                    Tick k = tickTracker[request.Exchange, symbol];
//                    if (k != null)
//                    {
//                        TickNotify ticknotify = new TickNotify();
//                        ticknotify.Tick = k;
//                        conn.Send(ticknotify.Data);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 某个连接注销合约行情
//        /// </summary>
//        /// <param name="conn"></param>
//        /// <param name="request"></param>
//        public void OnUngisterSymbol(IConnection conn, UnregisterSymbolTickRequest request)
//        {
//            foreach (var symbol in request.SymbolList)
//            {
//                if (string.IsNullOrEmpty(symbol)) continue;

//                //注销所有合约
//                if (symbol == "*")
//                {
//                    ClearSymbolRegisted(conn.SessionID);
//                    break;
//                }
//                if (symKeyRegMap.Keys.Contains(symbol))
//                {
//                    ConcurrentDictionary<string, IConnection> regmap = symKeyRegMap[symbol];
//                    IConnection target = null;
//                    regmap.TryRemove(conn.SessionID, out target);
//                }
//            }
//        }
//    }
//}
