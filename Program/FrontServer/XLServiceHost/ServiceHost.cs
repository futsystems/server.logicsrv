using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;
using Common.Logging;

namespace FrontServer.XLServiceHost
{
    public partial class XLServiceHost : FrontServer.IServiceHost
    {

        ILog logger = LogManager.GetLogger(_name);

        const string _name = "XLServiceHost";
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        public string Name { get { return _name; } }


        FrontServer.MQServer _mqServer = null;

        public XLServiceHost(FrontServer.MQServer mqServer)
        {
            _mqServer = mqServer;
        }

        XLServerBase xlSocketServer = null;
        bool _started = false;
        int _port = 55622;
        int _sendBufferSize = 4069;
        int _recvBufferSize = 4069;

        void InitServer()
        {
            xlSocketServer = new XLServerBase();
            ConfigFile _configFile = ConfigFile.GetConfigFile("frontsrv_xl.cfg");
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


            if (!xlSocketServer.Setup(cfg))
            {
                logger.Error("Setup TcpSocket Error");
            }

            logger.Info("recv buffersize:" + xlSocketServer.Config.SendBufferSize);

            xlSocketServer.NewSessionConnected += new SuperSocket.SocketBase.SessionHandler<XLSessionBase>(xlSocketServer_NewSessionConnected);
            xlSocketServer.NewRequestReceived += new SuperSocket.SocketBase.RequestHandler<XLSessionBase, XLRequestInfo>(xlSocketServer_NewRequestReceived);
            xlSocketServer.SessionClosed += new SuperSocket.SocketBase.SessionHandler<XLSessionBase, SuperSocket.SocketBase.CloseReason>(xlSocketServer_SessionClosed);

        }

        void xlSocketServer_SessionClosed(XLSessionBase session, SuperSocket.SocketBase.CloseReason value)
        {
            
        }

        void xlSocketServer_NewRequestReceived(XLSessionBase session, XLRequestInfo requestInfo)
        {
            
        }

        void xlSocketServer_NewSessionConnected(XLSessionBase session)
        {
            
        }

        public void HandleLogicMessage(FrontServer.IConnection connection, IPacket packet)
        {
        }


        public void Start()
        {
            InitServer();
            xlSocketServer.Start();
            logger.Info(string.Format("XLService Start at Prot:{0}", _port));
        }

    }
}
