using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace CTPService
{
    public partial class CTPServiceHost:FrontServer.IServiceHost
    {

        ILog logger = LogManager.GetLogger(_name);

        const string _name = "TCPServiceHost";
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        public string Name { get { return _name; } }


        FrontServer.MQServer _mqServer = null;
        public CTPServiceHost(FrontServer.MQServer mqServer)
        {
            _mqServer = mqServer;
        }

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


        ConcurrentDictionary<string, FrontServer.IConnection> connectionMap = new ConcurrentDictionary<string, FrontServer.IConnection>();

        void ctpSocketServer_SessionClosed(TLSessionBase session, SuperSocket.SocketBase.CloseReason value)
        {
            //logger.Info(string.Format("Session:{0} Closed", session.SessionID));
            OnSessionClosed(session);
            //逻辑服务器注销客户端
            _mqServer.LogicUnRegister(session.SessionID);
        }

        void ctpSocketServer_NewRequestReceived(TLSessionBase session, TLRequestInfo requestInfo)
        {
            try
            {
                logger.Info(string.Format("Session:{0} Request:{1} ", session.SessionID, requestInfo.Key));
                logger.Info("*** " + requestInfo.Body.Length.ToString());
                logger.Info(ByteUtil.ByteToHex(requestInfo.Body, ' '));
                logger.Info("***");

                CTPConnection conn = null;

                if (requestInfo.FTDType == EnumFTDType.FTDTypeNone)
                {
                    EnumFTDTagType tag = requestInfo.FTDTag;
                    switch (tag)
                    {
                        case EnumFTDTagType.FTDTagKeepAlive:
                            {
                                logger.Info(string.Format("Session:{0} HeartBeat", session.SessionID));
                                break;
                            }
                        case EnumFTDTagType.FTDTagRegister:
                            {
                                logger.Info(string.Format("Session:{0} Register", session.SessionID));

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
                                break;
                            }
                        default:
                            logger.Warn(string.Format("FTD Tag:{0} not handled", tag));
                            break;
                    }

                }

                if (requestInfo.FTDType == EnumFTDType.FTDTypeFTDC)
                {

                    if (!_connectionMap.TryGetValue(session.SessionID, out conn))
                    {
                        logger.Warn(string.Format("Client:{0} is not registed to server, ignore request", session.SessionID));
                        return;
                    }

                    EnumTransactionID transId = (EnumTransactionID)requestInfo.FTDHeader.dTransId;
                    switch (transId)
                    {
                        //用户登入
                        case EnumTransactionID.T_REQ_LOGIN:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcReqUserLoginField)
                                {
                                    Struct.V12.LCThostFtdcReqUserLoginField field = (Struct.V12.LCThostFtdcReqUserLoginField)data;
                                    LoginRequest request = RequestTemplate<LoginRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);
                                    request.LoginID = field.UserID;
                                    request.Passwd = field.Password;
                                    request.MAC = field.MacAddress;
                                    request.IPAddress = field.ClientIPAddress;
                                    request.LoginType = 1;
                                    request.ProductInfo = field.UserProductInfo;

                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} Request Login User:{1} Pass:{2}", session.SessionID, request.LoginID, request.Passwd));
                                }
                                break;
                            }
                        //查询投资者
                        case EnumTransactionID.T_QRY_USRINF:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryInvestorField)
                                {
                                    Struct.V12.LCThostFtdcQryInvestorField field = (Struct.V12.LCThostFtdcQryInvestorField)data;
                                    QryInvestorRequest request = RequestTemplate<QryInvestorRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);
                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} QryInvestorInfo", session.SessionID));
                                }
                                break;
                            }
                        default:
                            logger.Warn(string.Format("Transaction:{0} logic not handled", transId));
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Request Handler Error:" + ex.ToString());
            }
        }

        void ctpSocketServer_NewSessionConnected(TLSessionBase session)
        {
            //logger.Info(string.Format("Session:{0} Created", session.SessionID));
            OnSessionCreated(session);

        }

        public void Start()
        {
            InitServer();
            ctpSocketServer.Start();
            logger.Info(string.Format("CTPService Start at Prot:{0}", _port));
        }
    }
}
