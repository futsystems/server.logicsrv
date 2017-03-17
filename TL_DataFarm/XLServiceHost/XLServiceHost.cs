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

namespace XLServiceHost
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
    public partial class XLServiceHost:IServiceHost
    {
        ILog logger = LogManager.GetLogger(_name);

        const string _name = "XLServiceHost";
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        public string Name { get { return _name; } }


        XLServerBase tcpSocketServer = null;
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

            tcpSocketServer = new XLServerBase();


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

            tcpSocketServer.NewSessionConnected += new SuperSocket.SocketBase.SessionHandler<XLSessionBase>(OnNewSessionConnected);
            tcpSocketServer.NewRequestReceived += new SuperSocket.SocketBase.RequestHandler<XLSessionBase, XLRequestInfo>(OnNewRequestReceived);
            tcpSocketServer.SessionClosed += new SuperSocket.SocketBase.SessionHandler<XLSessionBase, SuperSocket.SocketBase.CloseReason>(OnSessionClosed);
            if (_workerendable)
            {
                StartProcess();
            }
        
        }

        void DestoryServer()
        {
            if (tcpSocketServer != null)
            {
                tcpSocketServer.NewSessionConnected -= new SuperSocket.SocketBase.SessionHandler<XLSessionBase>(OnNewSessionConnected);
                tcpSocketServer.NewRequestReceived -= new SuperSocket.SocketBase.RequestHandler<XLSessionBase, XLRequestInfo>(OnNewRequestReceived);
                tcpSocketServer.SessionClosed -= new SuperSocket.SocketBase.SessionHandler<XLSessionBase, SuperSocket.SocketBase.CloseReason>(OnSessionClosed);

                tcpSocketServer = null;
            }
        }

        #region SuperSocket 事件处理
        /// <summary>
        /// 连接建立事件
        /// </summary>
        /// <param name="session"></param>
        void OnNewSessionConnected(XLSessionBase session)
        {
            OnSessionCreated(session);
        }
        /// <summary>
        /// 连接关闭事件
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        void OnSessionClosed(XLSessionBase session, SuperSocket.SocketBase.CloseReason value)
        {
            OnSessionClosed(session);
        }


        /// <summary>
        /// 请求到达事件
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        void OnNewRequestReceived(XLSessionBase session, XLRequestInfo requestInfo)
        {
            try
            {
                if (requestInfo == null) return;
                logger.Info(string.Format("PacketData Received,Type:{0} Key:{1}", requestInfo.Body.MessageType, requestInfo.Key));
                XLConnection conn = null;
                //SessionID 检查连接对象
                if (!connectionMap.TryGetValue(session.SessionID, out conn))
                {
                    logger.Warn(string.Format("Client:{0} is not registed to server, ignore request", session.SessionID));
                    //关闭连接
                    session.Close();
                    return;
                }
                //conn.UpdateHeartBeat();

                //检查请求域
                if (requestInfo.Body.FieldList.Count == 0)
                {
                    logger.Warn(string.Format("Client:{0} empty request,ingore", session.SessionID));
                    return;
                }
                //通过XL协议请求处理
                this.OnXLRequestEvent(conn, requestInfo.Body, (int)requestInfo.DataHeader.RequestID);
            //    logger.Debug(string.Format("Message type:{0} content:{1} sessoin:{2}", requestInfo.Message.Type, requestInfo.Message.Content, session.SessionID));

            //    string sessionId = session.SessionID;
            //    switch (requestInfo.Message.Type)
            //    {
            //        //服务查询
            //        case MessageTypes.SERVICEREQUEST:
            //            {
            //                QryServiceRequest request = RequestTemplate<QryServiceRequest>.SrvRecvRequest("", sessionId, requestInfo.Message.Content);
            //                RspQryServiceResponse response = QryService(request);
            //                byte[] data = response.Data;
            //                session.Send(data, 0, data.Length);
            //                logger.Info(string.Format("Got QryServiceRequest from:{0} request:{1} reponse:{2}", sessionId, request, response));
            //                return;
            //            }
            //        //注册客户端
            //        case MessageTypes.REGISTERCLIENT:
            //            {
            //                RegisterClientRequest request = RequestTemplate<RegisterClientRequest>.SrvRecvRequest("", sessionId, requestInfo.Message.Content);
            //                if (request != null)
            //                {
            //                    IConnection conn = null;
            //                    //连接已经建立直接返回
            //                    if (_connectionMap.TryGetValue(sessionId, out conn))
            //                    {
            //                        logger.Warn(string.Format("Client:{0} already exist", session.SessionID));
            //                        return;
            //                    }

            //                    //创建连接
            //                    conn = new TCPSocketConnection(this, session);
            //                    _connectionMap.TryAdd(sessionId, conn);

            //                    //发送回报
            //                    RspRegisterClientResponse response = ResponseTemplate<RspRegisterClientResponse>.SrvSendRspResponse(request);
            //                    response.SessionID = sessionId;
            //                    conn.Send(response);

            //                    logger.Info(string.Format("Client:{0} registed to server", sessionId));
            //                    //向逻辑成抛出连接建立事件
            //                    OnConnectionCreated(conn);

            //                }
            //                return;
            //            }
            //        default:
            //            {
            //                IConnection conn = null;
            //                if (!_connectionMap.TryGetValue(sessionId, out conn))
            //                {
            //                    logger.Warn(string.Format("Client:{0} is not registed to server, ignore request", sessionId));
            //                    return;
            //                }

                            
            //                IPacket packet = PacketHelper.SrvRecvRequest(requestInfo.Message, "", sessionId);

            //                if (_workerendable)
            //                {
            //                    _itembuffer.Write(new MessageItem(conn, packet));
            //                    NewPacket();
            //                }
            //                else
            //                {
            //                    OnRequestEvent(conn, packet);
            //                }

            //            }
            //            return;

            //    }
            }
            catch (Exception ex)
            {
                logger.Error("Request Handler Error:" + ex.ToString());
            }
        }
        #endregion




        #region 启动 停止
        public void Start()
        {
            _cfg = ConfigFile.GetConfigFile("XLServiceHost.cfg");
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
