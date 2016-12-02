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
                logger.Debug(string.Format("Session:{0} Request:{1} ", session.SessionID, requestInfo.Key));
                //logger.Info("*** " + requestInfo.Body.Length.ToString());
                //logger.Info(ByteUtil.ByteToHex(requestInfo.Body, ' '));
                //logger.Info("***");

                CTPConnection conn = null;

                if (requestInfo.FTDType == EnumFTDType.FTDTypeNone)
                {
                    EnumFTDTagType tag = requestInfo.FTDTag;
                    switch (tag)
                    {
                        case EnumFTDTagType.FTDTagKeepAlive:
                            {
                                logger.Info(string.Format("Session:{0} >> HeartBeat", session.SessionID));
                                if (_connectionMap.TryGetValue(session.SessionID, out conn))
                                {
                                    //更新connection最近心跳时间
                                    conn.UpdateHeartBeat();

                                }
                                break;
                            }
                        case EnumFTDTagType.FTDTagRegister:
                            {
                                logger.Info(string.Format("Session:{0} >> Register", session.SessionID));

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
                                break;
                            }
                        default:
                            logger.Warn(string.Format("FTD Tag:{0} not handled", tag));
                            break;
                    }
                    return;
                }

                if (requestInfo.FTDType == EnumFTDType.FTDTypeFTDC)
                {

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

                    if (requestInfo.FTDFields.Count == 0)
                    {
                        logger.Warn(string.Format("Client:{0} empty request,ingore", session.SessionID));
                        return;
                    }


                    EnumTransactionID transId = (EnumTransactionID)requestInfo.FTDHeader.dTransId;
                    switch (transId)
                    {
                        //用户登入 ReqUserLogin
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
                                    conn.State.MACAddress = field.MacAddress;
                                    conn.State.CTPVersion = requestInfo.FTDHeader.bVersion.ToString();
                                    conn.State.BrokerID = field.BrokerID;
                                    conn.State.LoginID = field.UserID;
                                    conn.State.ProductInfo = field.UserProductInfo;

                                    logger.Info(string.Format("Session:{0} >> ReqUserLogin BrokerID:{1} User:{2} CTPVer:{3} ProductInfo:{4}", session.SessionID, field.BrokerID, field.UserID, requestInfo.FTDHeader.bVersion, field.UserProductInfo));
                                }
                                break;
                            }
                        //查询投资者 ReqQryInvestor
                        case EnumTransactionID.T_QRY_USRINF:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryInvestorField)
                                {
                                    Struct.V12.LCThostFtdcQryInvestorField field = (Struct.V12.LCThostFtdcQryInvestorField)data;
                                    QryInvestorRequest request = RequestTemplate<QryInvestorRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);
                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqQryInvestor", session.SessionID));
                                }
                                break;
                            }
                        //查询结算确认 ReqQrySettlementInfoConfirm
                        case EnumTransactionID.T_QRY_SETCONFIRM:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQrySettlementInfoConfirmField)
                                {
                                    Struct.V12.LCThostFtdcQrySettlementInfoConfirmField field = (Struct.V12.LCThostFtdcQrySettlementInfoConfirmField)data;
                                    QrySettleInfoConfirmRequest request = RequestTemplate<QrySettleInfoConfirmRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);
                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqQrySettlementInfoConfirm", session.SessionID));
                                }
                                break;
                            }
                        //*请求查询保证金监管系统经纪公司资金账户密钥 ReqQryCFMMCTradingAccountKey
                        case EnumTransactionID.T_QRY_CFMMCKEY:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryCFMMCTradingAccountKeyField)
                                {
                                    Struct.V12.LCThostFtdcQryCFMMCTradingAccountKeyField field = (Struct.V12.LCThostFtdcQryCFMMCTradingAccountKeyField)data;

                                    logger.Info(string.Format("Session:{0} >> ReqQryCFMMCTradingAccountKey", session.SessionID));

                                    Struct.V12.LCThostFtdcCFMMCTradingAccountKeyField response = new Struct.V12.LCThostFtdcCFMMCTradingAccountKeyField();
                                    response.BrokerID = "888888";
                                    response.AccountID = "00000";
                                    response.CurrentKey = "";
                                    response.KeyID = 1;
                                    response.ParticipantID = "";

                                    byte[] rdata = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcCFMMCTradingAccountKeyField>(ref response, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_CFMMCKEY, (int)requestInfo.FTDHeader.dReqId, conn.NextSeqId);
                                    int encPktLen = 0;
                                    byte[] encData = Struct.V12.StructHelperV12.EncPkt(rdata, out encPktLen);

                                    conn.Send(encData, encPktLen);
                                }
                                break;
                            }
                        //请求查询监控中心用户令牌 ReqQueryCFMMCTradingAccountToken
                        case EnumTransactionID.T_QRY_TDTOK:
                            {
                                //T_RSP_TDTOK 返回包含LCThostFtdcRspInfoField + LCThostFtdcQueryCFMMCTradingAccountTokenField 2个数据域
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQueryCFMMCTradingAccountTokenField)
                                {
                                    Struct.V12.LCThostFtdcQueryCFMMCTradingAccountTokenField field = (Struct.V12.LCThostFtdcQueryCFMMCTradingAccountTokenField)data;

                                    logger.Info(string.Format("Session:{0} >> ReqQueryCFMMCTradingAccountToken", session.SessionID));

                                    Struct.V12.LCThostFtdcRspInfoField rsp = new Struct.V12.LCThostFtdcRspInfoField();
                                    rsp.ErrorID = 0;
                                    rsp.ErrorMsg = "正确";


                                    byte[] rdata = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcQueryCFMMCTradingAccountTokenField>(ref rsp, ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_TDTOK, (int)requestInfo.FTDHeader.dReqId, conn.NextSeqId);
                                    int encPktLen = 0;
                                    byte[] encData = Struct.V12.StructHelperV12.EncPkt(rdata, out encPktLen);

                                    conn.Send(encData, encPktLen);
                                    //CTP柜台 此后还会进行T_RTN_TDTOK

                                }
                                break;
                            }
                        //请求查询客户通知 ReqQryNotice
                        case EnumTransactionID.T_QRY_NOTICE:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryNoticeField)
                                {
                                    Struct.V12.LCThostFtdcQryNoticeField field = (Struct.V12.LCThostFtdcQryNoticeField)data;
                                    QryNoticeRequest request = RequestTemplate<QryNoticeRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);
                                    
                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqQryNotice", session.SessionID));
                                }
                                break;
                            }
                        //请求查询交易通知 ReqQryTradingNotice
                        case EnumTransactionID.T_QRY_TDNOTICE:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryTradingNoticeField)
                                {
                                    Struct.V12.LCThostFtdcQryTradingNoticeField field = (Struct.V12.LCThostFtdcQryTradingNoticeField)data;
                                    logger.Info(string.Format("Session:{0} >> ReqQryTradingNotice", session.SessionID));

                                    Struct.V12.LCThostFtdcTradingNoticeField response = new Struct.V12.LCThostFtdcTradingNoticeField();
                                    //response.FieldContent = "市场有风险，投资需谨慎";
                                    //打包数据
                                    byte[] rdata = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcTradingNoticeField>(ref response, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_NOTICE, (int)requestInfo.FTDHeader.dReqId, conn.NextSeqId);
                                    int encPktLen = 0;
                                    byte[] encData = Struct.V12.StructHelperV12.EncPkt(rdata, out encPktLen);

                                    conn.Send(encData, encPktLen);

                                }
                                break;
                            }
                        //请求查询投资者结算结果 ReqQrySettlementInfo
                        case EnumTransactionID.T_QRY_SMI:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQrySettlementInfoField)
                                {
                                    Struct.V12.LCThostFtdcQrySettlementInfoField field = (Struct.V12.LCThostFtdcQrySettlementInfoField)data;
                                    XQrySettleInfoRequest request = RequestTemplate<XQrySettleInfoRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);

                                    request.Tradingday = string.IsNullOrEmpty(field.TradingDay) ? 0 : int.Parse(field.TradingDay);


                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqQrySettlementInfo", session.SessionID));

                                }
                                break;
                            }
                        //投资者结算结果确认 ReqSettlementInfoConfirm
                        case EnumTransactionID.T_REQ_SETCONFIRM:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcSettlementInfoConfirmField)
                                {
                                    Struct.V12.LCThostFtdcSettlementInfoConfirmField field = (Struct.V12.LCThostFtdcSettlementInfoConfirmField)data;
                                    ConfirmSettlementRequest request = RequestTemplate<ConfirmSettlementRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);
                                    //request.Account = field.InvestorID;

                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqSettlementInfoConfirm", session.SessionID));

                                }
                                break;
                                
                            }
                        //请求查询合约 ReqQryInstrument
                        case EnumTransactionID.T_QRY_INST:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryInstrumentField)
                                {
                                    Struct.V12.LCThostFtdcQryInstrumentField field = (Struct.V12.LCThostFtdcQryInstrumentField)data;

                                    QrySymbolRequest request = RequestTemplate<QrySymbolRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);
                                    request.SecurityType = SecurityType.FUT;//查询期货合约


                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqQryInstrument", session.SessionID));

                                }
                                break;
                            }
                        //请求查询报单 ReqQryOrder
                        case EnumTransactionID.T_QRY_ORDER:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryOrderField)
                                {
                                    Struct.V12.LCThostFtdcQryOrderField field = (Struct.V12.LCThostFtdcQryOrderField)data;

                                    QryOrderRequest request = RequestTemplate<QryOrderRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);
                                    //request.Symbol = request.Symbol;
                                    


                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqQryOrder", session.SessionID));

                                }
                                break;
                            }
                        //查询成交 ReqQryTrade
                        case EnumTransactionID.T_QRY_TRADE:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryTradeField)
                                {
                                    Struct.V12.LCThostFtdcQryTradeField field = (Struct.V12.LCThostFtdcQryTradeField)data;

                                    QryTradeRequest request = RequestTemplate<QryTradeRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);
                                    //request.Symbol = field.InstrumentID;


                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqQryTrade", session.SessionID));

                                }
                                break;
                            }
                        //请求查询投资者持仓 ReqQryInvestorPosition
                        case EnumTransactionID.T_QRY_INVPOS:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryInvestorPositionField)
                                {
                                    Struct.V12.LCThostFtdcQryInvestorPositionField field = (Struct.V12.LCThostFtdcQryInvestorPositionField)data;

                                    QryPositionRequest request = RequestTemplate<QryPositionRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);


                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqQryInvestorPosition", session.SessionID));

                                }
                                break;
                            }
                            //请求查询资金账户 ReqQryTradingAccount
                        case EnumTransactionID.T_QRY_TDACC:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryTradingAccountField)
                                {
                                    Struct.V12.LCThostFtdcQryTradingAccountField field = (Struct.V12.LCThostFtdcQryTradingAccountField)data;

                                    QryAccountInfoRequest request = RequestTemplate<QryAccountInfoRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);


                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqQryTradingAccount", session.SessionID));

                                }
                                break;
                            }
                            //请求查询签约银行 ReqQryContractBank
                        case EnumTransactionID.T_QRY_CONTBK:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryContractBankField)
                                {
                                    Struct.V12.LCThostFtdcQryContractBankField field = (Struct.V12.LCThostFtdcQryContractBankField)data;

                                    QryContractBankRequest request = RequestTemplate<QryContractBankRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);

                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqQryContractBank", session.SessionID));

                                }
                                break;
                            }
                            //请求查询银期签约关系 ReqQryAccountregister
                        case EnumTransactionID.T_QRY_ACCREG:
                            {
                                var data = requestInfo.FTDFields[0].FTDCData;
                                if (data is Struct.V12.LCThostFtdcQryAccountregisterField)
                                {
                                    Struct.V12.LCThostFtdcQryAccountregisterField field = (Struct.V12.LCThostFtdcQryAccountregisterField)data;

                                    QryRegisterBankAccountRequest request = RequestTemplate<QryRegisterBankAccountRequest>.CliSendRequest((int)requestInfo.FTDHeader.dReqId);

                                    _mqServer.TLSend(session.SessionID, request);
                                    logger.Info(string.Format("Session:{0} >> ReqQryAccountregister", session.SessionID));

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
