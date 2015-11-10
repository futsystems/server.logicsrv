using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm;

namespace TradingLib.DataFarm.Common
{

    /// <summary>
    /// 客户端连接通道管理
    /// </summary>
    public partial class DataServer
    {
        ConcurrentDictionary<string, DataFarm.API.IConnection> connectionMap = new ConcurrentDictionary<string, API.IConnection>();

        /// <summary>
        /// 客户端认证通过后创建Connection
        /// </summary>
        /// <param name="host"></param>
        /// <param name="sessionID"></param>
        DataFarm.API.IConnection CreateConnection(DataFarm.API.IServiceHost host, string sessionID)
        {
            DataFarm.API.IConnection connection = host.CreateConnection(sessionID);
            if (!connectionMap.Keys.Contains(sessionID))
            {
                connectionMap.TryAdd(sessionID, connection);
                return connection;
            }
            else
            { 
                throw new Exception(string.Format("Session:{0} already exit",sessionID));
            }
        }

        /// <summary>
        ///关闭Connection连接 
        /// </summary>
        void CloseConnection() 
        { 
        
        }

        /// <summary>
        /// 查找某个SessionID对应的Connection对象
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        DataFarm.API.IConnection GetConnection(string sessionID)
        {
            DataFarm.API.IConnection target = null;
            if (connectionMap.TryGetValue(sessionID, out target))
            {
                return target;
            }
            return null;
        }


    }
}
