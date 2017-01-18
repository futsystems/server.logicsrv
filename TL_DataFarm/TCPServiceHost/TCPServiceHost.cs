using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TCPServiceHost
{
    internal class MessageItem
    {
        public MessageItem(IConnection conn,IPacket packet)
        {
            this.Connection = conn;
            this.Packet = packet;
        }

        public IConnection Connection {get;set;}

        public IPacket Packet {get;set;}
    }
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

        

        bool _workergo = false;
        Thread _workerthread = null;
        RingBuffer<MessageItem> _itembuffer = new RingBuffer<MessageItem>(2000);
        static ManualResetEvent _processwaiting = new ManualResetEvent(false);
        void StartProcess()
        {
            if (_workergo) return;
            _workergo = true;
            _workerthread = new Thread(WorkProcess);
            _workerthread.IsBackground = false;
            _workerthread.Start();
        }

        void NewPacket()
        {
            if ((_workerthread != null) && (_workerthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _processwaiting.Set();
            }
        }

        void WorkProcess()
        {
            while (_workergo)
            {
                MessageItem item=null;
                while (_itembuffer.hasItems)
                {
                    try
                    {
                        item = _itembuffer.Read();
                        OnRequestEvent(item.Connection, item.Packet);
                    }
                    catch (Exception ex)
                    {
                        if (item != null)
                        {
                            logger.Error(string.Format("Conn:{0} Packet{1} {2} Process Error:{3}", item.Connection.SessionID, item.Packet.Type, item.Packet.Content, ex.ToString()));
                        }
                        else
                        {
                            logger.Error("Error:" + ex.ToString());
                        }
                    }
                }

                // clear current flag signal
                _processwaiting.Reset();
                //logger.Info("process send");
                // wait for a new signal to continue reading
                _processwaiting.WaitOne(1000);
            }
        }
        ConfigFile _cfg = null;
        //是否启用单独的Worker线程处理消息
        bool _workerendable = false;
        int _port = 5060;
        int _sendBufferSize = 65535;
        int _recvBufferSize = 65535;

        /// <summary>
        /// 初始化服务
        /// </summary>
        void InitServer()
        {

            tcpSocketServer = new TLServerBase();


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
            cfg.MaxRequestLength = 1024*10*10;
            
            //cfg.SendTimeOut = 
            //cfg.SyncSend = true;//同步发送 异步发送在Linux环境下会造成发送异常
            

            if (!tcpSocketServer.Setup(cfg))
            {
                logger.Error("Setup TcpSocket Error");
            }

            logger.Info("recv buffersize:" + tcpSocketServer.Config.SendBufferSize);

            tcpSocketServer.NewSessionConnected += new SuperSocket.SocketBase.SessionHandler<TLSessionBase>(tcpSocketServer_NewSessionConnected);
            tcpSocketServer.NewRequestReceived += new SuperSocket.SocketBase.RequestHandler<TLSessionBase, TLRequestInfo>(tcpSocketServer_NewRequestReceived);
            tcpSocketServer.SessionClosed += new SuperSocket.SocketBase.SessionHandler<TLSessionBase, SuperSocket.SocketBase.CloseReason>(tcpSocketServer_SessionClosed);
            if (_workerendable)
            {
                StartProcess();
            }
        
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

        #region SuperSocket 事件处理
        /// <summary>
        /// 连接建立事件
        /// </summary>
        /// <param name="session"></param>
        void tcpSocketServer_NewSessionConnected(TLSessionBase session)
        {
            OnSessionCreated(session);
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
        /// 用于维护已经正常Register的客户端  sessionMap用于维护底层Sockt连接
        /// </summary>
        ConcurrentDictionary<string, IConnection> _connectionMap = new ConcurrentDictionary<string, IConnection>();
        /// <summary>
        /// 请求到达事件
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        void tcpSocketServer_NewRequestReceived(TLSessionBase session, TLRequestInfo requestInfo)
        {
            try
            {
                logger.Debug(string.Format("Message type:{0} content:{1} sessoin:{2}", requestInfo.Message.Type, requestInfo.Message.Content, session.SessionID));

                string sessionId = session.SessionID;
                switch (requestInfo.Message.Type)
                {
                    //服务查询
                    case MessageTypes.SERVICEREQUEST:
                        {
                            QryServiceRequest request = RequestTemplate<QryServiceRequest>.SrvRecvRequest("", sessionId, requestInfo.Message.Content);
                            RspQryServiceResponse response = QryService(request);
                            byte[] data = response.Data;
                            session.Send(data, 0, data.Length);
                            logger.Info(string.Format("Got QryServiceRequest from:{0} request:{1} reponse:{2}", sessionId, request, response));
                            return;
                        }
                    //注册客户端
                    case MessageTypes.REGISTERCLIENT:
                        {
                            RegisterClientRequest request = RequestTemplate<RegisterClientRequest>.SrvRecvRequest("", sessionId, requestInfo.Message.Content);
                            if (request != null)
                            {
                                IConnection conn = null;
                                //连接已经建立直接返回
                                if (_connectionMap.TryGetValue(sessionId, out conn))
                                {
                                    logger.Warn(string.Format("Client:{0} already exist", session.SessionID));
                                    return;
                                }

                                //创建连接
                                conn = new TCPSocketConnection(this, session);
                                _connectionMap.TryAdd(sessionId, conn);

                                //发送回报
                                RspRegisterClientResponse response = ResponseTemplate<RspRegisterClientResponse>.SrvSendRspResponse(request);
                                response.SessionID = sessionId;
                                conn.Send(response.Data);

                                logger.Info(string.Format("Client:{0} registed to server", sessionId));
                                //向逻辑成抛出连接建立事件
                                OnConnectionCreated(conn);

                            }
                            return;
                        }
                    default:
                        {
                            IConnection conn = null;
                            if (!_connectionMap.TryGetValue(sessionId, out conn))
                            {
                                logger.Warn(string.Format("Client:{0} is not registed to server, ignore request", sessionId));
                                return;
                            }

                            
                            IPacket packet = PacketHelper.SrvRecvRequest(requestInfo.Message, "", sessionId);

                            if (_workerendable)
                            {
                                _itembuffer.Write(new MessageItem(conn, packet));
                                NewPacket();
                            }
                            else
                            {
                                OnRequestEvent(conn, packet);
                            }

                        }
                        return;

                }
            }
            catch (Exception ex)
            {
                logger.Error("Handle MessageType:{0} Content:{1} Error:{2}".Put(requestInfo.Message.Type, requestInfo.Message.Content, ex));
            }
        }
        #endregion




        #region 启动 停止
        public void Start()
        {
            _cfg = ConfigFile.GetConfigFile("TCPServiceHost.cfg");
            _port = _cfg["HistPort"].AsInt();
            _workerendable = _cfg["WorkerEnable"].AsBool();
            _sendBufferSize = _cfg["SendBufferSize"].AsInt();
            _recvBufferSize = _cfg["RecvBufferSize"].AsInt();


            logger.Info(string.Format("Start Transport Service:{0} at:{1}", _name, _port));
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
        #endregion

    }
}
