using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;
using SuperWebSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace WSServiceHost
{
    public partial class WSServiceHost : IServiceHost
    {

        ILog logger = LogManager.GetLogger(_name);

        const string _name = "WSServiceHost";
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        public string Name { get { return _name; } }


        ConfigFile _cfg = null;
        //是否启用单独的Worker线程处理消息
        bool _workerendable = false;
        int _port = 8080;

        WebSocketServer socketServer = null;

        void InitServer()
        {
            socketServer = new WebSocketServer();

            if (!socketServer.Setup(_port))
            {
                logger.Error("Setup WebSockServer Error");
                return;
            }

            socketServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(socketServer_NewMessageReceived);
            socketServer.NewSessionConnected += new SessionHandler<WebSocketSession>(socketServer_NewSessionConnected);
            socketServer.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(socketServer_SessionClosed);
            //socketServer.NewDataReceived += new SessionHandler<TLWebSocketSessionBase, byte[]>(socketServer_NewDataReceived);
            //socketServer.NewMessageReceived += new SessionHandler<TLWebSocketSessionBase, string>(socketServer_NewMessageReceived);
            //socketServer.NewSessionConnected += new SessionHandler<TLWebSocketSessionBase>(socketServer_NewSessionConnected);
            //socketServer.SessionClosed += new SessionHandler<TLWebSocketSessionBase, CloseReason>(socketServer_SessionClosed);
        }

        void socketServer_SessionClosed(WebSocketSession session, CloseReason value)
        {
            logger.Info(string.Format("Session:{0} closed", session.SessionID));
        }

        void socketServer_NewSessionConnected(WebSocketSession session)
        {
            logger.Info(string.Format("Session:{0} connected", session.SessionID));
        }

        void socketServer_NewMessageReceived(WebSocketSession session, string value)
        {
            logger.Info(string.Format("Session:{0} Message:{1}", session.SessionID, value));
        }

        //void socketServer_SessionClosed(TLWebSocketSessionBase session, CloseReason value)
        //{
        //    logger.Info(string.Format("Session:{0} closed", session.SessionID));
        //}

        //void socketServer_NewSessionConnected(TLWebSocketSessionBase session)
        //{
        //    logger.Info(string.Format("Session:{0} connected", session.SessionID));
        //}

        //void socketServer_NewMessageReceived(TLWebSocketSessionBase session, string value)
        //{
        //    logger.Info(string.Format("Session:{0} Message:{1}", session.SessionID, value));
        //}

        //void socketServer_NewDataReceived(TLWebSocketSessionBase session, byte[] value)
        //{
        //    //throw new NotImplementedException();
        //}

        public void Start()
        {
            logger.Info(string.Format("Start WebSocket Server at Port:{0}", _port));
            InitServer();

            if(!socketServer.Start())
            {
                logger.Error("Start WebSocket Error");
            }
            logger.Info("Start WebSocket Success");
        }

        public void Stop()
        { 
            
        }
    }
}
