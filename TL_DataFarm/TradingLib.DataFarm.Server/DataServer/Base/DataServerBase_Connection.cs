using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 维护客户端连接
    /// </summary>
    public partial class DataServerBase
    {
        ConcurrentDictionary<string, IConnection> connectionMap = new ConcurrentDictionary<string, IConnection>();

        /// <summary>
        /// IServiceHost接受客户端注册创建连接对象
        /// 逻辑层记录并维护连接对象
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        void OnConnectionCreated(IServiceHost host, IConnection conn)
        {
            AddConnection(conn);
        }

        /// <summary>
        /// IServiceHost接受客户端注销
        /// 逻辑层注销客户端
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        void OnConnectionClosed(IServiceHost host, IConnection conn)
        {
            RemoveConnection(conn);

            //清理连接注册的行情合约
            ClearSymbolRegisted(conn);
        }

        /// <summary>
        /// 删除连接
        /// </summary>
        /// <param name="conn"></param>
        void RemoveConnection(IConnection conn)
        {
            IConnection target = null;
            if (connectionMap.Keys.Contains(conn.SessionID))
            {
                connectionMap.TryRemove(conn.SessionID, out target);
                //logger.Info(string.Format("Connection:{0} closed", conn.SessionID));
            }
            else
            {
                logger.Warn(string.Format("Connection:{0} do not exit", conn.SessionID));
            }
        }

        /// <summary>
        /// 添加连接
        /// </summary>
        /// <param name="conn"></param>
        void AddConnection(IConnection conn)
        {
            if (!connectionMap.Keys.Contains(conn.SessionID))
            {
                connectionMap.TryAdd(conn.SessionID, conn);
                //logger.Info(string.Format("Connection:{0} created", conn.SessionID));
            }
            else
            {
                logger.Warn(string.Format("Connection:{0} already exit", conn.SessionID));
            }
        }


        /// <summary>
        /// 关闭某个连接
        /// </summary>
        /// <param name="conn"></param>
        public virtual void CloseConnection(IConnection conn)
        {
            RemoveConnection(conn);
        }

        /// <summary>
        /// 通过SessionID查找某个IConnection对象
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public virtual IConnection GetConnection(string sessionID)
        {
            IConnection target = null;
            if (connectionMap.TryGetValue(sessionID, out target))
            {
                return target;
            }
            return null;
        }
    }
}
