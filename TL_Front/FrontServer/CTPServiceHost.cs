using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace CTPService
{
    public class CTPServiceHost
    {

        ILog logger = LogManager.GetLogger(_name);

        const string _name = "TCPServiceHost";
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        public string Name { get { return _name; } }


        TLServerBase ctpSocketServer = null;
        bool _started = false;
        int _port = 55622;
        int _sendBufferSize = 4069;
        int _recvBufferSize = 4069;
        void InitServer()
        {
            ctpSocketServer = new TLServerBase();


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


            if (!ctpSocketServer.Setup(cfg))
            {
                logger.Error("Setup TcpSocket Error");
            }

            logger.Info("recv buffersize:" + ctpSocketServer.Config.SendBufferSize);

            ctpSocketServer.NewSessionConnected += new SuperSocket.SocketBase.SessionHandler<TLSessionBase>(ctpSocketServer_NewSessionConnected);
            ctpSocketServer.NewRequestReceived += new SuperSocket.SocketBase.RequestHandler<TLSessionBase, TLRequestInfo>(ctpSocketServer_NewRequestReceived);
            ctpSocketServer.SessionClosed += new SuperSocket.SocketBase.SessionHandler<TLSessionBase, SuperSocket.SocketBase.CloseReason>(ctpSocketServer_SessionClosed);
            
        }

        void ctpSocketServer_SessionClosed(TLSessionBase session, SuperSocket.SocketBase.CloseReason value)
        {
            logger.Info(string.Format("Session:{0} Closed", session.SessionID));
        }

        void ctpSocketServer_NewRequestReceived(TLSessionBase session, TLRequestInfo requestInfo)
        {
            logger.Info(string.Format("Session:{0} Request:{1} ", session.SessionID, ""));
        }

        void ctpSocketServer_NewSessionConnected(TLSessionBase session)
        {
            logger.Info(string.Format("Session:{0} Created", session.SessionID));
        }


        public void Start()
        {
            InitServer();
            ctpSocketServer.Start();
            logger.Info(string.Format("CTPService Start at Prot:{0}", _port));
        }
    }
}
