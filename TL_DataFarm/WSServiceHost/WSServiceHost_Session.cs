using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using SuperWebSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

using TradingLib.API;
using TradingLib.DataFarm.API;
using Common.Logging;


namespace WSServiceHost
{
    public partial class WSServiceHost
    {
        ConcurrentDictionary<string, WebSocketSession> sessionMap = new ConcurrentDictionary<string, WebSocketSession>();
        ConcurrentDictionary<string, WSConnection> connectionMap = new ConcurrentDictionary<string, WSConnection>();

        void OnSessionCreated(WebSocketSession session)
        {
            if (sessionMap.Keys.Contains(session.SessionID))
            {
                logger.Error(string.Format("Session[{0}] already created", session.SessionID));
                return;
            }
            sessionMap.TryAdd(session.SessionID, session);
            var connection = new WSConnection(this, session);
            connectionMap.TryAdd(session.SessionID, connection);
            OnConnectionCreated(connection);

            logger.Info(string.Format("Session[{0}] created", session.SessionID));
        }

        void OnSessionClosed(WebSocketSession session)
        {
            if (!sessionMap.Keys.Contains(session.SessionID))
            {
                logger.Error(string.Format("Session[{0}] not exist!", session.SessionID));
                return;
            }
            WebSocketSession target = null;
            if (sessionMap.TryRemove(session.SessionID, out target))
            {
                logger.Info(string.Format("Session[{0}] closed", session.SessionID));

            }
            else
            {
                logger.Error("some error happend in close session");
            }

            //检查_connectionMap是否有对应的Connection对象 如果存在则向上抛出事件
            if (connectionMap.Keys.Contains(session.SessionID))
            {
                WSConnection conn = null;
                connectionMap.TryRemove(session.SessionID, out conn);
                //向逻辑层抛出Connection关闭事件
                OnConnectionClosed(conn);
            }
        }
    }
}
