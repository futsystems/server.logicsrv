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
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;


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
     
        }

        void socketServer_SessionClosed(WebSocketSession session, CloseReason value)
        {
            logger.Info(string.Format("Session:{0} closed", session.SessionID));
            OnSessionClosed(session);
        }

        void socketServer_NewSessionConnected(WebSocketSession session)
        {
            logger.Info(string.Format("Session:{0} connected", session.SessionID));
            OnSessionCreated(session);
        }

        void socketServer_NewMessageReceived(WebSocketSession session, string value)
        {
            WSConnection conn = null;
            try
            {
                logger.Info(string.Format("Session:{0} Message:{1}", session.SessionID, value));
                //SessionID 检查连接对象
                if (!connectionMap.TryGetValue(session.SessionID, out conn))
                {
                    logger.Warn(string.Format("Client:{0} is not registed to server, ignore request", session.SessionID));
                    //关闭连接
                    session.Close();
                    return;
                }

                //Json数据转换成XLPacketData 通过OnXLRequestEvent进行处理
                Newtonsoft.Json.Linq.JObject jobject = Newtonsoft.Json.Linq.JObject.Parse(value);
                XLMessageType type = jobject["MessageType"].ToObject<XLMessageType>();
                int requestID = 0;
                var pktData = XLPacketData.DeserializeJsonRequest(type, value, out requestID);
                //通过XL协议请求处理
                this.OnXLRequestEvent(conn, pktData, requestID);

            }
            catch (Exception ex)
            {
                logger.Error("Request Handler Error:" + ex.ToString());
            }
        }

     

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
