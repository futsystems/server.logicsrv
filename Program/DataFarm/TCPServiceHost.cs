using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace DataFarm
{
    public partial class TCPServiceHost:IServiceHost
    {
        ILog logger = LogManager.GetLogger(_name);

        const string  _name = "TCPServiceHost";
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        public string Name { get { return _name; } }


        TLServerBase tcpSocketServer = null;
        bool _started = false;


        /// <summary>
        /// 初始化服务
        /// </summary>
        void InitServer()
        {
            tcpSocketServer = new TLServerBase();
            if (!tcpSocketServer.Setup("127.0.0.1", 5060))
            {
                logger.Error("Setup TcpSocket Error");
            }
            tcpSocketServer.NewSessionConnected += new SuperSocket.SocketBase.SessionHandler<TLSessionBase>(tcpSocketServer_NewSessionConnected);
            tcpSocketServer.NewRequestReceived += new SuperSocket.SocketBase.RequestHandler<TLSessionBase, TLRequestInfo>(tcpSocketServer_NewRequestReceived);
            tcpSocketServer.SessionClosed += new SuperSocket.SocketBase.SessionHandler<TLSessionBase, SuperSocket.SocketBase.CloseReason>(tcpSocketServer_SessionClosed);
          
        }

        void DestoryServer()
        {
            if (tcpSocketServer != null)
            {
                tcpSocketServer.NewSessionConnected -= new SuperSocket.SocketBase.SessionHandler<TLSessionBase>(tcpSocketServer_NewSessionConnected);
                tcpSocketServer.NewRequestReceived -= new SuperSocket.SocketBase.RequestHandler<TLSessionBase, TLRequestInfo>(tcpSocketServer_NewRequestReceived);
                tcpSocketServer.SessionClosed -= new SuperSocket.SocketBase.SessionHandler<TLSessionBase, SuperSocket.SocketBase.CloseReason>(tcpSocketServer_SessionClosed);

                tcpSocketServer = null;
            }
        }

        /// <summary>
        /// 连接关闭事件
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        void tcpSocketServer_SessionClosed(TLSessionBase session, SuperSocket.SocketBase.CloseReason value)
        {
            OnSessionClosed(session);
        }

        /// <summary>
        /// 请求到达事件
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        void tcpSocketServer_NewRequestReceived(TLSessionBase session, TLRequestInfo requestInfo)
        {
            logger.Info(string.Format("Message type:{0} content:{1} sessoin:{2}", requestInfo.Message.Type, requestInfo.Message.Content, session.SessionID));
            
        }

        /// <summary>
        /// 连接建立事件
        /// </summary>
        /// <param name="session"></param>
        void tcpSocketServer_NewSessionConnected(TLSessionBase session)
        {
            OnSessionCreated(session);
        }


        public void Start()
        {
            if (_started)
            {
                logger.Warn(string.Format("ServiceHost:{0} already started", _name));
                return;
            }

            InitServer();
            if (!tcpSocketServer.Start())
            {
                logger.Error(string.Format("ServiceHost:{0} start error", _name));
            }
            _started = true;
            logger.Info(string.Format("ServiceHost:{0} start success", _name));
        }

        public void Stop()
        {
            if (!_started)
            { 
                logger.Warn(string.Format("ServiceHost:{0} not started", _name));
                return;
            }

            tcpSocketServer.Stop();

            DestoryServer();
            logger.Info(string.Format("ServiceHost:{0} stop success", _name));
            
        }
    }
}
