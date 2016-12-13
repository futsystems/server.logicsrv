using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;
using Common.Logging;
using SuperWebSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace FrontServer.WSServiceHost
{
    public partial class WSServiceHost
    {
        ConcurrentDictionary<string, WebSocketSession> sessionMap = new ConcurrentDictionary<string, WebSocketSession>();
        /// <summary>
        /// 用于维护已经正常Register的客户端  sessionMap用于维护底层Sockt连接
        /// </summary>
        ConcurrentDictionary<string, WSConnection> _connectionMap = new ConcurrentDictionary<string, WSConnection>();

        void OnSessionCreated(WebSocketSession session)
        {
            if (sessionMap.Keys.Contains(session.SessionID))
            {
                logger.Error(string.Format("Session:{0} already created", session.SessionID));
                return;
            }
            sessionMap.TryAdd(session.SessionID, session);
            logger.Info(string.Format("Session:{0} >> Created", session.SessionID));
        }

        void OnSessionClosed(WebSocketSession session)
        {
            if (!sessionMap.Keys.Contains(session.SessionID))
            {
                logger.Error(string.Format("Session:{0} not exist!", session.SessionID));
                return;
            }
            WebSocketSession target = null;
            if (sessionMap.TryRemove(session.SessionID, out target))
            {
                logger.Info(string.Format("Session:{0} >> Closed", session.SessionID));

            }
            else
            {
                logger.Error("some error happend in close session");
            }

            //检查_connectionMap是否有对应的Connection对象 如果存在则向上抛出事件
            if (_connectionMap.Keys.Contains(session.SessionID))
            {
                WSConnection conn = null;
                _connectionMap.TryRemove(session.SessionID, out conn);
                //向逻辑层抛出Connection关闭事件
                //OnConnectionClosed(conn);
            }
        }

        public WSConnection CreateConnection(string sessionID)
        {
            if (!sessionMap.Keys.Contains(sessionID))
            {
                logger.Error(string.Format("Session:{0} not exist!", sessionID));
                return null;
            }
            WebSocketSession target = null;
            if (sessionMap.TryGetValue(sessionID, out target))
            {
                WSConnection connection = new WSConnection(this, target);
                return connection;
            }
            return null;


        }

        public WSConnection GetConnection(string sessionID)
        {
            WSConnection target = null;
            if (_connectionMap.TryGetValue(sessionID, out target))
            {
                return target;
            }
            return null;
        }
      



    }
}
