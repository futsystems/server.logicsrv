using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TCPServiceHost
{
    public partial class TCPServiceHost:IServiceHost
    {

        ConcurrentDictionary<string, TLSessionBase> sessionMap = new ConcurrentDictionary<string, TLSessionBase>();

        void OnSessionCreated(TLSessionBase session)
        {
            if (sessionMap.Keys.Contains(session.SessionID))
            {
                logger.Error(string.Format("Session:{0} already created", session.SessionID));
                return;
            }
            sessionMap.TryAdd(session.SessionID, session);
            logger.Info(string.Format("Session:{0} created success", session.SessionID));
        }

        void OnSessionClosed(TLSessionBase session)
        {
            if (!sessionMap.Keys.Contains(session.SessionID))
            {
                logger.Error(string.Format("Session:{0} not exist!", session.SessionID));
                return;
            }
            TLSessionBase target = null;
            if (sessionMap.TryRemove(session.SessionID, out target))
            {
                logger.Info(string.Format("Session:{0} closed success", session.SessionID));
            }
            else
            {
                logger.Error("some error happend in close session");
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
            if(sessionMap.TryGetValue(sessionID,out target))
            {
                IConnection connection = new TCPSocketConnection(this, target);
                return connection;
            }
            return null;

            
        }

        public void CloseConnection(string sessionID)
        { 
            
        }


    }
}
