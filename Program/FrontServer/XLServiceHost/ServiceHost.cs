﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;
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
            //logger.Info(string.Format("Session:{0} Closed", session.SessionID));
            OnSessionClosed(session);
            //逻辑服务器注销客户端
            _mqServer.LogicUnRegister(session.SessionID);
        }

        void xlSocketServer_NewRequestReceived(XLSessionBase session, XLRequestInfo requestInfo)
        {
            try
            {
                logger.Info(string.Format("PacketData Received,Type:{0} Key:{1}", requestInfo.Body.MessageType, requestInfo.Key));
                XLConnection conn = null;
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

                //检查请求域
                if (requestInfo.Body.FieldList.Count == 0)
                {
                    logger.Warn(string.Format("Client:{0} empty request,ingore", session.SessionID));
                    return;
                }


                switch (requestInfo.Body.MessageType)
                {
                    case XLMessageType.T_REQ_LOGIN:
                        {
                            var data = requestInfo.Body.FieldList[0].FieldData;
                            if (data is XLReqLoginField)
                            {
                                XLReqLoginField request = (XLReqLoginField)data;

                                XLPacketData pktData = new XLPacketData(XLMessageType.T_RSP_LOGIN);
                                ErrorField rsp = new ErrorField();
                                rsp.ErrorID = 0;
                                rsp.ErrorMsg = "正确";

                                XLRspLoginField response = new XLRspLoginField();
                                response.Name = "测试";
                                response.TradingDay = 20150101;
                                response.UserID = request.UserID;

                                pktData.AddField(rsp);
                                pktData.AddField(response);
                                byte[] ret = XLPacketData.PackToBytes(pktData, XLEnumSeqType.SeqReq, conn.NextSeqReqId, requestInfo.DataHeader.RequestID, true);
                                conn.Send(ret);

                                logger.Info(string.Format("Session:{0} >> ReqUserLogin", session.SessionID));

                                

                            }
                            else
                            {
                                logger.Warn(string.Format("Request:{0} Data Field do not macth", requestInfo.Body.MessageType));
                            }
                            break;
                        }
                    default:
                        logger.Warn(string.Format("Packet:{0} logic not handled", requestInfo.Body.MessageType));
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Request Handler Error:" + ex.ToString());
            }

        }

        void xlSocketServer_NewSessionConnected(XLSessionBase session)
        {
            OnSessionCreated(session);
            logger.Info(string.Format("Session:{0} >> Register", session.SessionID));
            XLConnection conn = null;
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
