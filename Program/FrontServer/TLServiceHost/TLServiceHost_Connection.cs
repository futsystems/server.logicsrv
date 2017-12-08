using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace FrontServer.TLServiceHost
{
    public partial class TLServiceHost
    {

        ConcurrentDictionary<string, TLSessionBase> sessionMap = new ConcurrentDictionary<string, TLSessionBase>();
        /// <summary>
        /// 用于维护已经正常Register的客户端  sessionMap用于维护底层Sockt连接
        /// </summary>
        ConcurrentDictionary<string, TLConnection> _connectionMap = new ConcurrentDictionary<string, TLConnection>();

        void OnSessionCreated(TLSessionBase session)
        {
            if (sessionMap.Keys.Contains(session.SessionID))
            {
                logger.Error(string.Format("Session:{0} already created", session.SessionID));
                return;
            }
            sessionMap.TryAdd(session.SessionID, session);
            logger.Info(string.Format("Session:{0} >> Created", session.SessionID));
        }

        void OnSessionClosed(TLSessionBase session)
        {
            //if (!sessionMap.Keys.Contains(session.SessionID))
            //{
            //    logger.Error(string.Format("Session:{0} not exist!", session.SessionID));
            //    return;
            //}
            TLSessionBase target = null;
            if (sessionMap.TryRemove(session.SessionID, out target))
            {
                logger.Info(string.Format("Session:{0} >> Closed", session.SessionID));

            }
            else
            {
                logger.Error(string.Format("Session:{0} not exist!", session.SessionID));
            }

            //检查_connectionMap是否有对应的Connection对象 如果存在则向上抛出事件
            if (_connectionMap.Keys.Contains(session.SessionID))
            {
                TLConnection conn = null;
                _connectionMap.TryRemove(session.SessionID, out conn);
                //向逻辑层抛出Connection关闭事件
                //OnConnectionClosed(conn);
            }
        }

        public TLConnection CreateConnection(string sessionID)
        {
            //todo 简化
            //if (!sessionMap.Keys.Contains(sessionID))
            //{
            //    logger.Error(string.Format("Session:{0} not exist!", sessionID));
            //    return null;
            //}
            TLSessionBase target = null;
            if (sessionMap.TryGetValue(sessionID, out target))
            {
                logger.Info(string.Format("TLConnection:{0} created", sessionID));
                TLConnection connection = new TLConnection(this, target);
                return connection;
            }
            return null;


        }

        public TLConnection GetConnection(string sessionID)
        {
            TLConnection target = null;
            if (_connectionMap.TryGetValue(sessionID, out target))
            {
                return target;
            }
            return null;
        }

        public void CloseConnection(string sessionID)
        {

        }
    }
}
