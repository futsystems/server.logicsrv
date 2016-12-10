﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using ZeroMQ;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;
using Common.Logging;


namespace FrontServer
{
    public class MQServer
    {
        ILog logger = LogManager.GetLogger("MQServer");

        ConcurrentDictionary<string, IConnection> connectionMap = new ConcurrentDictionary<string, IConnection>();
        public MQServer()
        { 
        
        }



        /// <summary>
        /// 当前MQServer是否处于工作状态
        /// </summary>
        public bool IsLive { get { return _srvgo; } }

        bool _stopped = false;
        public bool IsStopped { get { return _stopped; } }
        public void Start()
        {
            if (_srvgo) return;
            _srvgo = true;
            logger.Info("Start MQServer");
            ConfigFile _configFile = ConfigFile.GetConfigFile("frontsrv_srv.cfg");
            _logicPort = _configFile["LogicPort"].AsInt();
            _logicServer = _configFile["LogicServer"].AsString();

            mainThread = new Thread(MessageProcess);
            mainThread.IsBackground = true;
            mainThread.Start();
        }

        public void Stop()
        {
            if (!_srvgo) return;
            logger.Info("Stop MQServer");
            _srvgo = false;
            mainThread.Join();
        }

        int _logicPort=5570;
        string _logicServer = "127.0.0.1";
        bool _srvgo = false;
        TimeSpan pollerTimeOut = new TimeSpan(0, 0, 1);
        ZSocket _backend = null;
        ZContext _ctx = null;
        Thread mainThread = null;

        object _obj = new object();



        /// <summary>
        /// 注销交易客户端
        /// </summary>
        /// <param name="sessionId"></param>
        public void LogicUnRegister(string sessionId)
        {
            IConnection target = null;
            if (connectionMap.TryRemove(sessionId, out target))
            {
                UnregisterClientRequest request = RequestTemplate<UnregisterClientRequest>.CliSendRequest(0);
                this.TLSend(sessionId, request);
            }
        }

        /// <summary>
        /// 注册交易客户端
        /// </summary>
        /// <param name="sessionId"></param>
        public void LogicRegister(FrontServer.IConnection connection)
        {
            if (!connectionMap.Keys.Contains(connection.SessionID))
            {
                RegisterClientRequest request = RequestTemplate<RegisterClientRequest>.CliSendRequest(0);
                this.TLSend(connection.SessionID, request);
                connectionMap.TryAdd(connection.SessionID, connection);
            }
        }

        /// <summary>
        /// 通过客户端UUID获得客户端连接对象
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        IConnection GetConnection(string address)
        {
            IConnection target;
            if (connectionMap.TryGetValue(address, out target))
            {
                return target;
            }
            return null;
        }

        /// <summary>
        /// 发送逻辑服务端心跳
        /// 用于确认逻辑服务器连接可用
        /// </summary>
        public void LogicHeartBeat()
        {
            logger.Debug("Send Logic HeartBeat");
            LogicLiveRequest request = RequestTemplate<LogicLiveRequest>.CliSendRequest(0);
            this.TLSend(_frontID, request);
            _lastHeartBeatSend = DateTime.Now;
        }


        public void TLSend(string address, IPacket packet)
        {
            this.TLSend(address, packet.Data);
        }
        public void TLSend(string address,byte[] data)
        {
            if (_backend != null)
            {
                lock (_obj)
                {
                    using (ZMessage zmsg = new ZMessage())
                    {
                        ZError error;
                        zmsg.Add(new ZFrame(Encoding.UTF8.GetBytes(address)));
                        zmsg.Add(new ZFrame(data));
                        //logger.Info("adds:" + CTPService.ByteUtil.ByteToHex(Encoding.UTF8.GetBytes(address)));
                        //logger.Info("data:" + CTPService.ByteUtil.ByteToHex(data));
                        if (!_backend.Send(zmsg, out error))
                        {
                            if (error == ZError.ETERM)
                            {
                                logger.Error("got ZError.ETERM,return directly");
                                return;	// Interrupted
                            }
                            throw new ZException(error);
                        }
                    }
                }
            }
        }

        public DateTime LastHeartBeatRecv { get { return _lastHeartBeatRecv; } }

        DateTime _lastHeartBeatSend = DateTime.Now;
        DateTime _lastHeartBeatRecv = DateTime.Now;

        string _frontID = string.Empty;
        Random rd = new Random();
        void MessageProcess()
        {
            _lastHeartBeatRecv = DateTime.Now;
            _lastHeartBeatSend = DateTime.Now;
            _stopped = false;
            using(var ctx = new ZContext())
            {
                _ctx = ctx;
                using(ZSocket backend = new ZSocket(ctx, ZSocketType.DEALER))
                {
                    string address = string.Format("tcp://{0}:{1}", _logicServer, _logicPort);
                    _frontID = "front-" + rd.Next(1000, 9999).ToString();
                    backend.SetOption(ZSocketOption.IDENTITY, Encoding.UTF8.GetBytes(_frontID));
                    backend.Connect(address);
                    
                    backend.Linger = new TimeSpan(0);//需设置 否则底层socket无法释放 导致无法正常关闭服务
                    backend.ReceiveTimeout = new TimeSpan(0, 0, 1);
                    logger.Info(string.Format("Connect to logic server:{0}", address));
                    _backend = backend;

                    ZError error;
                    ZMessage incoming;
                    ZPollItem item = ZPollItem.CreateReceiver();
                    logger.Info("MQServer MessageProcess Started");
                    while (_srvgo)
                    {
                        try
                        {
                            _backend.PollIn(item, out incoming, out error, pollerTimeOut);
                            if (incoming != null)
                            {
                                int cnt = incoming.Count;
                                if (cnt == 2)
                                {
                                    string clientId = incoming[0].ReadString(Encoding.UTF8);//读取地址
                                    Message message = Message.gotmessage(incoming.Last().Read());//读取消息
                                    logger.Debug(string.Format("LogicResponse Frames:{2} Type:{0} Content:{1} ", message.Type, message.Content, cnt));

                                    if (!string.IsNullOrEmpty(clientId))
                                    {
                                        IPacket packet = PacketHelper.CliRecvResponse(message);

                                        if (clientId == _frontID)//本地心跳
                                        {
                                            if (packet.Type == MessageTypes.LOGICLIVERESPONSE)
                                            {
                                                _lastHeartBeatRecv = DateTime.Now;
                                            }
                                        }
                                        else
                                        {
                                            IConnection conn = GetConnection(clientId);
                                            if (conn != null)
                                            {
                                                if (conn.IsXLProtocol)
                                                {
                                                    this.HandleLogicMessage(conn, packet);
                                                }
                                                else
                                                {
                                                    //调用Connection对应的ServiceHost处理逻辑消息包
                                                    conn.ServiceHost.HandleLogicMessage(conn, packet);
                                                }
                                            }
                                            else
                                            {
                                                logger.Warn(string.Format("Client:{0} do not exist", clientId));
                                            }
                                        }
                                    }
                                }
                                incoming.Clear();
                            }
                            else
                            {
                                if (error == ZError.ETERM)
                                {
                                    return;	// Interrupted
                                }
                                if (error != ZError.EAGAIN)
                                {
                                    throw new ZException(error);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            logger.Error("Poll Message Error:"+ex.ToString());
                        }
                    }
                }
            }
            _stopped = true;
            logger.Info("MQServer MessageProcess Stoppd");
        }


        void HandleLogicMessage(IConnection conn, IPacket lpkt)
        {
            switch (lpkt.Type)
            {
                case MessageTypes.LOGINRESPONSE:
                    {
                        LoginResponse response = lpkt as LoginResponse;
                        //将数据转换成CTP业务结构体
                        XLRspLoginField field = new XLRspLoginField();
                        field.TradingDay = response.TradingDay;
                        field.UserID = response.LoginID;
                        field.Name = response.NickName;

                        ErrorField rsp = ConvertRspInfo(response.RspInfo);

                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_LOGIN);
                        pkt.AddField(rsp);
                        pkt.AddField(field);


                        conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);
                        //if (response.RspInfo.ErrorID == 0)
                        //{
                        //    conn.State.Authorized = true;
                        //    conn.State.FrontID = field.FrontID;
                        //    conn.State.SessionID = field.SessionID;
                        //}
                        break;
                    }

                default:
                    logger.Warn(string.Format("Logic Packet:{0} not handled", lpkt.Type));
                    break;

            }
        }

        public void HandleXLPacketData(IConnection conn, XLPacketData pkt,int requestId)
        {
            switch (pkt.MessageType)
            {
                case XLMessageType.T_REQ_LOGIN:
                    {
                        var data = pkt.FieldList[0].FieldData;
                        if (data is XLReqLoginField)
                        {
                            XLReqLoginField field = (XLReqLoginField)data;

                            LoginRequest request = RequestTemplate<LoginRequest>.CliSendRequest(requestId);
                            request.LoginID = field.UserID;
                            request.Passwd = field.Password;
                            request.MAC = field.MacAddress;
                            request.IPAddress = field.ClientIPAddress;
                            request.LoginType = 1;
                            request.ProductInfo = field.UserProductInfo;

                            this.TLSend(conn.SessionID, request);


                            //XLPacketData pktData = new XLPacketData(XLMessageType.T_RSP_LOGIN);
                            //ErrorField rsp = new ErrorField();
                            //rsp.ErrorID = 0;
                            //rsp.ErrorMsg = "正确";

                            //XLRspLoginField response = new XLRspLoginField();
                            //response.Name = "测试";
                            //response.TradingDay = 20150101;
                            //response.UserID = request.UserID;

                            //pktData.AddField(rsp);
                            //pktData.AddField(response);
                            //byte[] ret = XLPacketData.PackToBytes(pktData, XLEnumSeqType.SeqReq, conn.NextSeqReqId, requestInfo.DataHeader.RequestID, true);
                            //conn.Send(ret);

                            logger.Info(string.Format("Session:{0} >> ReqUserLogin", conn.SessionID));



                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                default:
                    logger.Warn(string.Format("Packet:{0} logic not handled", pkt.MessageType));
                    break;
            }
        }


        ErrorField ConvertRspInfo(RspInfo info)
        {
            ErrorField field = new ErrorField();
            field.ErrorID = info.ErrorID;
            field.ErrorMsg = info.ErrorMessage;
            return field;
        }
    }
}
