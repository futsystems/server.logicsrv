using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperWebSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;
using Common.Logging;

namespace FrontServer.WSServiceHost
{
    public partial class WSServiceHost : FrontServer.IServiceHost
    {

        ILog logger = LogManager.GetLogger(_name);

        const string _name = "WSServiceHost";
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        public string Name { get { return _name; } }


        FrontServer.MQServer _mqServer = null;

        public WSServiceHost(FrontServer.MQServer mqServer)
        {
            _mqServer = mqServer;
        }

        WebSocketServer wsSocketServer = null;
        bool _started = false;
        int _port = 55622;
        int _sendBufferSize = 4069;
        int _recvBufferSize = 4069;

        void InitServer()
        {
            wsSocketServer = new WebSocketServer();
            ConfigFile _configFile = ConfigFile.GetConfigFile("frontsrv_ws.cfg");
            _port = _configFile["Port"].AsInt();

            SuperSocket.SocketBase.Config.ServerConfig cfg = new SuperSocket.SocketBase.Config.ServerConfig();
            cfg.Port = _port;
            cfg.SendBufferSize = _sendBufferSize;
            cfg.ReceiveBufferSize = _recvBufferSize;
            cfg.Ip = "0.0.0.0";

            cfg.ClearIdleSession = true;
            cfg.IdleSessionTimeOut = 300;
            cfg.ClearIdleSessionInterval = 120;
            cfg.MaxConnectionNumber = 1024;
            cfg.Mode = SuperSocket.SocketBase.SocketMode.Tcp;
            cfg.LogAllSocketException = true;
            cfg.LogBasicSessionActivity = true;
            cfg.MaxRequestLength = 1024 * 10 * 10;

            //cfg.SendTimeOut = 
            //cfg.SyncSend = true;//同步发送 异步发送在Linux环境下会造成发送异常


            if (!wsSocketServer.Setup(_port))
            {
                logger.Error("Setup WebSocket Error");
            }

            logger.Info("recv buffersize:" + wsSocketServer.Config.SendBufferSize);

            wsSocketServer.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(wsSocketServer_SessionClosed);
            wsSocketServer.NewSessionConnected += new SessionHandler<WebSocketSession>(wsSocketServer_NewSessionConnected);
            
            wsSocketServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(wsSocketServer_NewMessageReceived);
        }

        void wsSocketServer_NewMessageReceived(WebSocketSession session, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value)) return;
                logger.Info(string.Format("Json Received:{0}",value));
                WSConnection conn = null;
                //SessionID 检查连接对象
                if (!_connectionMap.TryGetValue(session.SessionID, out conn))
                {
                    logger.Warn(string.Format("Client:{0} is not registed to server, ignore request", session.SessionID));
                    //关闭连接
                    session.Close();
                    //逻辑服务器注销客户端
                    _mqServer.LogicUnRegister(session.SessionID);
                    return;
                }
                conn.UpdateHeartBeat();

                //Json数据转换成XLPacketData并进行处理
                //_mqServer.HandleXLPacketData(conn, requestInfo.Body, (int)requestInfo.DataHeader.RequestID);


            }
            catch (Exception ex)
            {
                logger.Error("Request Handler Error:" + ex.ToString());
            }
        }

        void wsSocketServer_NewSessionConnected(WebSocketSession session)
        {
            logger.Info(string.Format("Session:{0} connected", session.SessionID));

            OnSessionCreated(session);
            logger.Info(string.Format("Session:{0} >> Register", session.SessionID));
            WSConnection conn = null;
            //连接已经建立直接返回
            if (_connectionMap.TryGetValue(session.SessionID, out conn))
            {
                logger.Warn(string.Format("Client:{0} already exist", session.SessionID));
                return;
            }

            //创建连接
            conn = CreateConnection(session.SessionID);
            _connectionMap.TryAdd(session.SessionID, conn);
            //客户端发送初始化数据包后执行逻辑服务器客户端注册操作
            _mqServer.LogicRegister(conn);
            logger.Info(string.Format("Session:{0} Registed Remote EndPoint:{1}", conn.SessionID, conn.State.IPAddress));
        }

        void wsSocketServer_SessionClosed(WebSocketSession session, CloseReason value)
        {
            logger.Info(string.Format("Session:{0} Closed", session.SessionID));
            OnSessionClosed(session);
            //逻辑服务器注销客户端
            _mqServer.LogicUnRegister(session.SessionID);
        }

     

        public void HandleLogicMessage(FrontServer.IConnection connection, IPacket packet)
        {

        }


        public void Start()
        {
            InitServer();
            wsSocketServer.Start();
            logger.Info(string.Format("WSService Start at Prot:{0}", _port));
        }

    }
}
