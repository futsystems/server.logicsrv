using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace XLServiceHost
{
    public partial class XLServiceHost
    {

        ConcurrentDictionary<string, XLSessionBase> sessionMap = new ConcurrentDictionary<string, XLSessionBase>();
        ConcurrentDictionary<string, XLConnection> connectionMap = new ConcurrentDictionary<string, XLConnection>();

        void OnSessionCreated(XLSessionBase session)
        {
            if (sessionMap.Keys.Contains(session.SessionID))
            {
                logger.Error(string.Format("Session[{0}] already created", session.SessionID));
                return;
            }
            sessionMap.TryAdd(session.SessionID, session);
            var connection = new XLConnection(this, session);
            connectionMap.TryAdd(session.SessionID, connection);
            OnConnectionCreated(connection);

            logger.Info(string.Format("Session[{0}] created", session.SessionID));
        }

        void OnSessionClosed(XLSessionBase session)
        {
            if (!sessionMap.Keys.Contains(session.SessionID))
            {
                logger.Error(string.Format("Session[{0}] not exist!", session.SessionID));
                return;
            }
            XLSessionBase target = null;
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
                XLConnection conn=null;
                connectionMap.TryRemove(session.SessionID, out conn);
                //向逻辑层抛出Connection关闭事件
                OnConnectionClosed(conn);
            }
        }

        //public IConnection CreateConnection(string sessionID)
        //{
        //    if (!sessionMap.Keys.Contains(sessionID))
        //    {
        //        logger.Error(string.Format("Session:{0} not exist!", sessionID));
        //        return null;
        //    }
        //    XLSessionBase target = null;
        //    if(sessionMap.TryGetValue(sessionID,out target))
        //    {
        //        IConnection connection = new XLConnection(this, target);
        //        return connection;
        //    }
        //    return null;

            
        //}

        //public void CloseConnection(string sessionID)
        //{ 

            
        //}


    }
}
