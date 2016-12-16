using System;
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
    public partial class MQServer
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
        int _tickPort = 5572;

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
                //using (ZSocket subscriber = new ZSocket(ctx, ZSocketType.SUB))
                {
                    string address = string.Format("tcp://{0}:{1}", _logicServer, _logicPort);
                    _frontID = "front-" + rd.Next(1000, 9999).ToString();
                    backend.SetOption(ZSocketOption.IDENTITY, Encoding.UTF8.GetBytes(_frontID));
                    backend.Linger = new TimeSpan(0);//需设置 否则底层socket无法释放 导致无法正常关闭服务
                    backend.ReceiveTimeout = new TimeSpan(0, 0, 1);
                    backend.Connect(address);

                    logger.Info(string.Format("Connect to logic server:{0}", address));
                    _backend = backend;

                    //string subadd = string.Format("tcp://{0}:{1}", _logicServer, _tickPort);
                    //subscriber.Connect(subadd);
                    //subscriber.Subscribe(Encoding.UTF8.GetBytes(""));

                    List<ZSocket> sockets = new List<ZSocket>();
                    sockets.Add(backend);
                    //sockets.Add(subscriber);

                    List<ZPollItem> pollitems = new List<ZPollItem>();
                    pollitems.Add(ZPollItem.CreateReceiver());
                    //pollitems.Add(ZPollItem.CreateReceiver());

                    ZError error;
                    ZMessage[] incoming;
                    //ZPollItem item = ZPollItem.CreateReceiver();
                    logger.Info("MQServer MessageProcess Started");
                    while (_srvgo)
                    {
                        try
                        {
                            if (sockets.PollIn(pollitems, out incoming, out error, pollerTimeOut))
                            {
                                //Backend
                                if (incoming[0] != null)
                                {
                                    int cnt = incoming[0].Count;
                                    if (cnt == 2)
                                    {
                                        string clientId = incoming[0][0].ReadString(Encoding.UTF8);//读取地址
                                        Message message = Message.gotmessage(incoming[0].Last().Read());//读取消息
                                        //logger.Debug(string.Format("LogicResponse Frames:{2} Type:{0} Content:{1} ", message.Type, message.Content, cnt));
                                        logger.Debug(string.Format("LogicResponse Type:{0} Session:{1}", message.Type, clientId));
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
                                    incoming[0].Clear();
                                }
                                //TickSub
                                //if (incoming[1] != null)
                                //{
                                //    string tickstr = incoming[1].First().ReadString(Encoding.UTF8);
                                //    Tick k = TickImpl.Deserialize2(tickstr);
                                //    if (k != null && k.UpdateType != "H")
                                //    { 
                                //        //处理行情逻辑
                                //        tickTracker.UpdateTick(k);

                                //        if (k.UpdateType == "X" || k.UpdateType == "Q" || k.UpdateType == "F" || k.UpdateType == "S")
                                //        {
                                //            //转发实时行情
                                //            Tick snapshot = tickTracker[k.Exchange, k.Symbol];
                                //            //logger.Info("notifytick");
                                //            NotifyTick2Connections(snapshot);
                                //        }
                                        
                                //    }
                                //    incoming[1].Clear();
                                //    //logger.Info(tickstr);
                                //}
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


    }
}
