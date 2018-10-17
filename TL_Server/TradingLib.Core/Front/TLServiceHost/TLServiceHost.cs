using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

namespace FrontServer.TLServiceHost
{
    public partial class TLServiceHost : FrontServer.IServiceHost
    {

        ILog logger = LogManager.GetLogger(_name);

        const string _name = "TLServiceHost";
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        public string Name { get { return _name; } }


        FrontServer.MQServer _mqServer = null;
        //心跳包
        byte[] heartBeatPkt = new byte[] { 0, 6, 0, 0, 7, 4, 0, 0, 0, 0x27 };

        public TLServiceHost(FrontServer.MQServer mqServer)
        {
            _mqServer = mqServer;
        }

        TLServerBase tlSocketServer = null;
        int _port = 55622;
        int _sendBufferSize = 4069;
        int _recvBufferSize = 4069;
        void InitServer()
        {
            tlSocketServer = new TLServerBase();
            ConfigFile _configFile = ConfigFile.GetConfigFile("frontsrv_tl.cfg");
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

            cfg.SendTimeOut = 1500;
            //cfg.SyncSend = true;//同步发送 异步发送在Linux环境下会造成发送异常


            if (!tlSocketServer.Setup(cfg))
            {
                logger.Error("Setup TcpSocket Error");
            }

            logger.Info("recv buffersize:" + tlSocketServer.Config.SendBufferSize);

            tlSocketServer.NewSessionConnected += new SuperSocket.SocketBase.SessionHandler<TLSessionBase>(tlSocketServer_NewSessionConnected);
            tlSocketServer.SessionClosed += new SuperSocket.SocketBase.SessionHandler<TLSessionBase, SuperSocket.SocketBase.CloseReason>(tlSocketServer_SessionClosed);
            tlSocketServer.NewRequestReceived += new SuperSocket.SocketBase.RequestHandler<TLSessionBase, TLRequestInfo>(tlSocketServer_NewRequestReceived);
        }

        DateTime _lasttime = DateTime.Now;
        long _requestCnt = 0;
        ConcurrentDictionary<MessageTypes, int> typeCntMap = new ConcurrentDictionary<MessageTypes, int>();

        void tlSocketServer_NewRequestReceived(TLSessionBase session, TLRequestInfo requestInfo)
        {
            
            try
            {
                if (GlobalConfig.ProfileEnable) RunConfig.Instance.Profile.EnterSection("TLSocket NewRequest");

                _requestCnt++;
                if (!typeCntMap.ContainsKey(requestInfo.Message.Type))
                {
                    typeCntMap.TryAdd(requestInfo.Message.Type, 0);
                }
                typeCntMap[requestInfo.Message.Type] = (typeCntMap[requestInfo.Message.Type] + 1);

                if (DateTime.Now.Subtract(_lasttime).Minutes >= 1)
                {
                    logger.Debug(string.Format("last minute TL request cnt:{0}", _requestCnt));
                    foreach (var item in typeCntMap)
                    {
                        logger.Debug(string.Format("     type :{0} cnt:{1}", item.Key, item.Value));
                        typeCntMap[item.Key] = 0;
                    }
                    _requestCnt = 0;
                    _lasttime = DateTime.Now;

                }
                if (session == null)
                {
                    logger.Error("[Session Null] return directly");
                    return;
                }
                logger.Debug(string.Format("Message type:{0} content:{1} sessoin:{2}", requestInfo.Message.Type, requestInfo.Message.Content, session.SessionID));

                string sessionId = session.SessionID;
                switch (requestInfo.Message.Type)
                {
                    //服务查询 查询服务之后 客户端Socket连接会断开
                    case MessageTypes.SERVICEREQUEST:
                        {
                            QryServiceRequest request = RequestTemplate<QryServiceRequest>.SrvRecvRequest("", sessionId, requestInfo.Message.Content);
                            RspQryServiceResponse response = ResponseTemplate<RspQryServiceResponse>.SrvSendRspResponse(request);
                            response.OnService = (_mqServer == null || !_mqServer.IsLive) ? false : true;
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
                                TLConnection conn = null;
                                //连接已经建立直接返回
                                if (_connectionMap.TryGetValue(sessionId, out conn))
                                {
                                    logger.Warn(string.Format("Client:{0} already exist", session.SessionID));
                                    return;
                                }

                                //创建连接 如果sessionmap没有记录session则conn为null
                                conn = CreateConnection(session.SessionID);
                                //conn为空判定
                                if (conn == null)
                                {
                                    logger.Error(string.Format("Session:{0} Register,but connection created fail", session.SessionID));
                                    logger.Info("--->close connection [0]:" + session.SessionID);
                                    session.Close();
                                    OnSessionClosed(session);
                                    return;
                                }

                                _connectionMap.TryAdd(session.SessionID, conn);
                                //客户端发送初始化数据包后执行逻辑服务器客户端注册操作
                                _mqServer.LogicRegister(conn, EnumFrontType.TLSocket, request.VersionToken);
                                logger.Info("connection registed");
                                //发送回报
                                RspRegisterClientResponse response = ResponseTemplate<RspRegisterClientResponse>.SrvSendRspResponse(request);
                                response.SessionID = sessionId;

                                //创建connection之后 数据发送需要统一由mqserver中的线程进行发送
                                _mqServer.ForwardToClient(conn, response.Data);

                                logger.Info(string.Format("Session:{0} Registed Remote EndPoint:{1}", conn.SessionID, conn.State.IPAddress));
                            }
                            return;
                        }
                    default:
                        {
                            //通过SessionID查找对应的TLConnection
                            TLConnection conn = null;
                            if (!_connectionMap.TryGetValue(sessionId, out conn))
                            {
                                logger.Warn(string.Format("Client:{0} is not registed to server, ignore request", sessionId));

                                logger.Info("--->close connection [1]:" + session.SessionID);
                                session.Close();
                                OnSessionClosed(session);
                                //逻辑服务器注销客户端
                                _mqServer.LogicUnRegister(session.SessionID);
                                return;
                            }
                            //conn为空判定
                            if (conn == null)
                            {
                                logger.Info("--->close connection [2]:" + session.SessionID);
                                session.Close();
                                OnSessionClosed(session);
                                //逻辑服务器注销客户端
                                _mqServer.LogicUnRegister(session.SessionID);
                                return;
                            }

                            conn.UpdateHeartBeat();
                            IPacket packet = PacketHelper.SrvRecvRequest(requestInfo.Message, "", sessionId);

                            //客户端心跳回报
                            if (packet.Type == MessageTypes.HEARTBEATREQUEST)
                            {
                                HeartBeatResponse response = ResponseTemplate<HeartBeatResponse>.SrvSendRspResponse(packet as HeartBeatRequest);
                                _mqServer.ForwardToClient(conn, response.Data);
                                return;
                            }

                            //客户端登入 记录IP信息
                            if (packet.Type == MessageTypes.LOGINREQUEST)
                            {
                                LoginRequest request = packet as LoginRequest;
                                request.IPAddress = conn.IState.IPAddress;
                            }

                            _mqServer.ForwardToBackend(conn.SessionID, packet);

                        }
                        return;

                }
            }
            catch (Exception ex)
            {
                logger.Error("Handle Front MessageType:{0} Content:{1} Error:{2} Stack:{3}".Put(requestInfo.Message.Type, requestInfo.Message.Content, ex, ex.StackTrace));
            }
            finally
            {
                if (GlobalConfig.ProfileEnable) RunConfig.Instance.Profile.LeaveSection();
            }
        }

        void tlSocketServer_SessionClosed(TLSessionBase session, SuperSocket.SocketBase.CloseReason value)
        {
            OnSessionClosed(session);
            //逻辑服务器注销客户端
            _mqServer.LogicUnRegister(session.SessionID);
            
        }

        void tlSocketServer_NewSessionConnected(TLSessionBase session)
        {
            if (_mqServer == null || !_mqServer.IsLive) session.Close();//mqServer为空或者非运行状态 则不接收外部连接建立
            OnSessionCreated(session);
        }


       

        public void Start()
        {
            InitServer();
            tlSocketServer.Start();
            logger.Info(string.Format("TLService Start at Prot:{0}", _port));
        }

        public void Stop()
        {
            if (tlSocketServer != null)
            {
                tlSocketServer.Stop();
            }
            logger.Info("TLService Stopped");
        }

    }

}
