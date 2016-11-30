using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace CTPService
{
    public partial class CTPServiceHost
    {

        ConcurrentDictionary<string, TLSessionBase> sessionMap = new ConcurrentDictionary<string, TLSessionBase>();
        /// <summary>
        /// 用于维护已经正常Register的客户端  sessionMap用于维护底层Sockt连接
        /// </summary>
        ConcurrentDictionary<string, IConnection> _connectionMap = new ConcurrentDictionary<string, IConnection>();

        void OnSessionCreated(TLSessionBase session)
        {
            if (sessionMap.Keys.Contains(session.SessionID))
            {
                logger.Error(string.Format("Session[{0}] already created", session.SessionID));
                return;
            }
            sessionMap.TryAdd(session.SessionID, session);
            logger.Info(string.Format("Session[{0}] created", session.SessionID));
        }

        void OnSessionClosed(TLSessionBase session)
        {
            if (!sessionMap.Keys.Contains(session.SessionID))
            {
                logger.Error(string.Format("Session[{0}] not exist!", session.SessionID));
                return;
            }
            TLSessionBase target = null;
            if (sessionMap.TryRemove(session.SessionID, out target))
            {
                logger.Info(string.Format("Session[{0}] closed", session.SessionID));

            }
            else
            {
                logger.Error("some error happend in close session");
            }

            //检查_connectionMap是否有对应的Connection对象 如果存在则向上抛出事件
            if (_connectionMap.Keys.Contains(session.SessionID))
            {
                IConnection conn = null;
                _connectionMap.TryRemove(session.SessionID, out conn);
                //向逻辑层抛出Connection关闭事件
                //OnConnectionClosed(conn);
            }
        }

        public IConnection CreateConnection(string sessionID)
        {
            if (!sessionMap.Keys.Contains(sessionID))
            {
                logger.Error(string.Format("Session:{0} not exist!", sessionID));
                return null;
            }
            TLSessionBase target = null;
            if (sessionMap.TryGetValue(sessionID, out target))
            {
                IConnection connection = new CTPConnection(target);
                return connection;
            }
            return null;


        }

        public void CloseConnection(string sessionID)
        {

        }
    }
}
