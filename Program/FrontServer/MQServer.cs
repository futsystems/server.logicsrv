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
        string _logicServer = "127.0.0.1";
        bool _srvgo = false;
        TimeSpan pollerTimeOut = new TimeSpan(0, 0, 1);
        ZSocket _backend = null;
        ZContext _ctx = null;
        Thread mainThread = null;

        object _obj = new object();



        /// <summary>
        /// 启动数据储存服务
        /// </summary>
        public void StartWorker()
        {
            logger.Info("[Start Worker Service]");
            if (_sendgo) return;
            _sendgo = true;
            _sendthread = new Thread(ProcessItem);
            _sendthread.IsBackground = false;
            _sendthread.Start();
        }


        class WorkerItem
        {
            public string SessionID { get; set; }

            public IPacket Packet { get; set; }
        }

        class SendItem
        {
            public IConnection Connection { get; set; }

            public byte[] Data { get; set; }
        }

        RingBuffer<WorkerItem> itembuffer = new RingBuffer<WorkerItem>(50000);
        RingBuffer<SendItem> sendItemBuffer = new RingBuffer<SendItem>(50000);

        static ManualResetEvent _sendwaiting = new ManualResetEvent(false);
        Thread _sendthread = null;
        bool _sendgo = false;
        void NewSend()
        {
            if ((_sendthread != null) && (_sendthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _sendwaiting.Set();
            }
        }

        public void Send(IConnection conn, byte[] data)
        {
            sendItemBuffer.Write(new SendItem() { Connection = conn, Data = data });
            NewSend();
        }

        const int SLEEPDEFAULTMS = 500;
        void ProcessItem()
        {
            WorkerItem st = null;
            while (_sendgo)
            {
                try
                {
                    while (sendItemBuffer.hasItems)
                    {
                        var tmp = sendItemBuffer.Read();
                        //在某个特定情况下 会出现 待发送数据结构为null的情况 多个线程对sendbuffer的访问 形成竞争
                        if (tmp == null)
                        {
                            logger.Error("XXXX SendItem Buffer Got Null Struct");
                            continue;
                        }
                        if (tmp != null && tmp.Connection != null)
                        {
                            tmp.Connection.Send(tmp.Data);
                        }
                    }


                    while (itembuffer.hasItems)
                    {
                        st = itembuffer.Read();
                        //在某个特定情况下 会出现 待发送数据结构为null的情况 多个线程对sendbuffer的访问 形成竞争
                        if (st == null)
                        {
                            logger.Error("XXXX Worker Buffer Got Null Struct");
                            continue;
                        }

                        IConnection conn = GetConnection(st.SessionID);

                        try
                        {
                            
                            if (conn != null)
                            {
                                if (conn.IsXLProtocol)
                                {
                                    this.HandleLogicMessage(conn, st.Packet);
                                }
                                else
                                {
                                    //调用Connection对应的ServiceHost处理逻辑消息包
                                    conn.ServiceHost.HandleLogicMessage(conn, st.Packet);
                                }
                            }
                            else
                            {
                                logger.Warn(string.Format("Client:{0} do not exist", st.SessionID));
                            }

                        }
                        catch (System.TimeoutException t)
                        { 
                            //处理超时
                            if (conn != null)
                            {
                                conn.Close();
                            }
                            logger.Error(string.Format("Conn:{0} Handle Packet:{1} Error:{2} trace:{3}", st.SessionID, st.Packet.ToString(), t.ToString(), t.StackTrace));
                        }
                        catch (Exception ex)
                        {

                            logger.Error(string.Format("Conn:{0} Handle Packet:{1} Error:{2} trace:{3}", st.SessionID, st.Packet.ToString(), ex.ToString(),ex.StackTrace));

                        }
                    }
                    // clear current flag signal
                    _sendwaiting.Reset();
                    //logger.Info("process send");
                    // wait for a new signal to continue reading
                    _sendwaiting.WaitOne(SLEEPDEFAULTMS);

                }
                catch (Exception ex)
                {
                    logger.Error("SendWorker Process  error:" + ex.ToString());
                }
            }

        }



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

        //public void LogicClientHeartBeat(string sessionId)
        //{
        //    IConnection target = null;
        //    if (connectionMap.TryGetValue(sessionId, out target))
        //    {
        //        HeartBeat request = RequestTemplate<HeartBeat>.CliSendRequest(0);
        //        this.TLSend(sessionId, request);
        //    }
        //}
        /// <summary>
        /// 注册交易客户端
        /// </summary>
        /// <param name="sessionId"></param>
        public void LogicRegister(FrontServer.IConnection connection,EnumFrontType type,string versionToken)
        {
            if (!connectionMap.Keys.Contains(connection.SessionID))
            {
                RegisterClientRequest request = RequestTemplate<RegisterClientRequest>.CliSendRequest(0);
                request.FrontType = type;
                request.VersionToken = versionToken;
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

        void CloseAllConnection()
        {
            logger.Info("Close all client's connectioni");
            foreach (var conn in connectionMap.Values.ToArray())
            {
                conn.Close();
            }
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
                    _frontID = "front-" + rd.Next(1000, 9999).ToString();//前置随机变化
                    backend.SetOption(ZSocketOption.IDENTITY, Encoding.UTF8.GetBytes(_frontID));
                    backend.Linger = new TimeSpan(0);//需设置 否则底层socket无法释放 导致无法正常关闭服务
                    backend.ReceiveTimeout = new TimeSpan(0, 0, 1);
                    backend.Connect(address);

                    logger.Info(string.Format("Connect to logic server:{0}", address));
                    _backend = backend;

                    List<ZSocket> sockets = new List<ZSocket>();
                    sockets.Add(backend);

                    List<ZPollItem> pollitems = new List<ZPollItem>();
                    pollitems.Add(ZPollItem.CreateReceiver());

                    ZError error;
                    ZMessage[] incoming;
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
                                                    LogicLiveResponse response = packet as LogicLiveResponse;
                                                    if (response.Status == "Live")
                                                    {
                                                        _lastHeartBeatRecv = DateTime.Now;
                                                    }
                                                }
                                                if (packet.Type == MessageTypes.NOTIFYCLEARCLIENT)
                                                {
                                                    NotifyClearClient notify = packet as NotifyClearClient;
                                                    if(!string.IsNullOrEmpty(notify.SessionID))
                                                    {
                                                        IConnection conn = GetConnection(notify.SessionID);
                                                        if (conn != null)
                                                        {
                                                            conn.Close();
                                                            this.LogicUnRegister(notify.SessionID);

                                                        }
                                                    }
                                                }
                                                if (packet.Type == MessageTypes.NOTIFYREBOOTMQSRV)
                                                {
                                                    _srvgo = false;
                                                    logger.Info("Reboot MQServer.....");
                                                }
                                            }
                                            else //其余客户端地址为 UUID表示 转发数据到客户端
                                            {
                                                itembuffer.Write(new WorkerItem() { SessionID = clientId, Packet = packet });
                                                NewSend();
                                            }
                                        }
                                    }
                                    incoming[0].Clear();
                                }
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

            //关闭所有客户端
            CloseAllConnection();

            //输出日志与标识
            _stopped = true;
            logger.Info("MQServer MessageProcess Stoppd");
        }


    }
}
