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
        bool _started = false;
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

            //cfg.SendTimeOut = 
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

        void tlSocketServer_NewRequestReceived(TLSessionBase session, TLRequestInfo requestInfo)
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
                            RspQryServiceResponse response = ResponseTemplate<RspQryServiceResponse>.SrvSendRspResponse(request);
                            response.OnService = true;
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

                                //创建连接
                                conn = CreateConnection(session.SessionID);

                                _connectionMap.TryAdd(session.SessionID, conn);
                                //客户端发送初始化数据包后执行逻辑服务器客户端注册操作
                                _mqServer.LogicRegister(conn);
                                

                                //发送回报
                                RspRegisterClientResponse response = ResponseTemplate<RspRegisterClientResponse>.SrvSendRspResponse(request);
                                response.SessionID = sessionId;
                                conn.Send(response.Data);

                                logger.Info(string.Format("Session:{0} Registed Remote EndPoint:{1}", conn.SessionID, conn.State.IPAddress));
                                //logger.Info(string.Format("Client:{0} registed to server", sessionId));

                            }
                            return;
                        }
                    default:
                        {
                            TLConnection conn = null;
                            if (!_connectionMap.TryGetValue(sessionId, out conn))
                            {
                                logger.Warn(string.Format("Client:{0} is not registed to server, ignore request", sessionId));
                                return;
                            }

                            IPacket packet = PacketHelper.SrvRecvRequest(requestInfo.Message, "", sessionId);
                            _mqServer.TLSend(conn.SessionID, packet);

                        }
                        return;

                }
            }
            catch (Exception ex)
            {
                logger.Error("Handle MessageType:{0} Content:{1} Error:{2}".Put(requestInfo.Message.Type, requestInfo.Message.Content, ex));
            }
        }

        void tlSocketServer_SessionClosed(TLSessionBase session, SuperSocket.SocketBase.CloseReason value)
        {
            //logger.Info(string.Format("Session:{0} Closed", session.SessionID));
            OnSessionClosed(session);
            //逻辑服务器注销客户端
            _mqServer.LogicUnRegister(session.SessionID);
        }

        void tlSocketServer_NewSessionConnected(TLSessionBase session)
        {
            OnSessionCreated(session);
        }


        public void HandleLogicMessage(FrontServer.IConnection connection, IPacket packet)
        {
            try
            {
                string hex = string.Empty;
                TLConnection conn = GetConnection(connection.SessionID);
                if (conn == null)
                {
                    logger.Warn(string.Format("Session:{0} TLConnection do not exist", connection.SessionID));
                    return;
                }
                //将逻辑服务器发送过来的数据转发到对应的连接上去
                conn.Send(packet.Data);

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Handler Logic Packet:{0} Error:{1}", packet.ToString(), ex.ToString()));
            }
        }

        public void Start()
        {
            InitServer();
            tlSocketServer.Start();
            logger.Info(string.Format("TLService Start at Prot:{0}", _port));
        }

    }

}
