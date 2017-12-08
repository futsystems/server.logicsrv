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
        public bool IsLive { get { return _pollGo; } }

        bool _stopped = false;
        public bool IsStopped { get { return _stopped; } }

        public void Start()
        {
            if (_pollGo) return;
            _pollGo = true;
            logger.Info("Start MQServer");
            ConfigFile _configFile = ConfigFile.GetConfigFile("frontsrv_srv.cfg");
            _logicPort = _configFile["LogicPort"].AsInt();
            _logicServer = _configFile["LogicServer"].AsString();

            _pollThread = new Thread(PollProcess);
            _pollThread.IsBackground = true;
            _pollThread.Start();
        }

        public void Stop()
        {
            if (!_pollGo) return;
            logger.Info("Stop MQServer");
            _pollGo = false;
            _pollThread.Join();
        }



        int _logicPort=5570;
        string _logicServer = "127.0.0.1";
        bool _pollGo = false;
        TimeSpan pollerTimeOut = new TimeSpan(0, 0, 1);
        ZSocket _backend = null;
        Thread _pollThread = null;

        object _obj = new object();



        /// <summary>
        /// 启动数据储存服务
        /// </summary>
        public void StartWorker()
        {
            logger.Info("[Start Worker Service]");
            if (_workergo) return;
            _workergo = true;
            _workerThread = new Thread(WrokerProcess);
            _workerThread.IsBackground = false;
            _workerThread.Start();
        }




        RingBuffer<LogicMessageItem> logicMessageBuffer = new RingBuffer<LogicMessageItem>(50000);
        RingBuffer<ClientDataItem> clientDataBuffer = new RingBuffer<ClientDataItem>(50000);

        static ManualResetEvent _sendwaiting = new ManualResetEvent(false);
        Thread _workerThread = null;
        bool _workergo = false;
        void NewWorkerItem()
        {
            if ((_workerThread != null) && (_workerThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _sendwaiting.Set();
            }
        }
        const int SLEEPDEFAULTMS = 500;
        DateTime _lastWorkerTime = DateTime.Now;
        void WrokerProcess()
        {
            IConnection conn = null;
            while (_workergo)
            {
                try
                {
                    if (DateTime.Now.Subtract(_lastWorkerTime).TotalSeconds>10)
                    {
                        logger.Info("--> Worker thread live");
                        _lastWorkerTime = DateTime.Now;
                    }

                    while (clientDataBuffer.hasItems)
                    {
                        var tmp = clientDataBuffer.Read();
                        //在某个特定情况下 会出现 待发送数据结构为null的情况 多个线程对sendbuffer的访问 形成竞争
                        if (tmp == null)
                        {
                            logger.Error("[Buffer Null] ClientData Buffer Got Null Struct");
                            continue;
                        }
                        if (tmp.Connection != null && tmp.Connection.Connected)
                        {
                            conn = tmp.Connection;
                            conn.Send(tmp.Data);
                        }

                    }


                    while (logicMessageBuffer.hasItems)
                    {
                        var tmp = logicMessageBuffer.Read();
                        //在某个特定情况下 会出现 待发送数据结构为null的情况 多个线程对sendbuffer的访问 形成竞争
                        if (tmp == null)
                        {
                            logger.Error("[Buffer Null] LogicMessage Buffer Got Null Struct");
                            continue;
                        }
                        conn = GetConnection(tmp.SessionID);

                        if (conn != null && conn.ServiceHost!= null && conn.Connected)
                        {
                            //调用Connection对应的ServiceHost处理逻辑消息包
                            conn.ServiceHost.HandleLogicMessage(conn, tmp.Packet);
                        }
                        else
                        {
                            logger.Warn(string.Format("Client:{0} do not exist", tmp.SessionID));
                        }
                    }
                }
                catch (System.TimeoutException t)
                {
                    //处理超时
                    if (conn != null)
                    {
                        logger.Info("--->close connection:" + conn.SessionID);
                        conn.Close();
                    }
                    logger.Error("Worker Process  error:" + t.ToString() + t.StackTrace);
                }
                catch (Exception ex)
                {
                    logger.Error("Worker Process  error:" + ex.ToString()+ex.StackTrace);
                }

                // clear current flag signal
                _sendwaiting.Reset();
                //logger.Info("process send");
                // wait for a new signal to continue reading
                _sendwaiting.WaitOne(SLEEPDEFAULTMS);
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
                this.ForwardToBackend(sessionId, request);
            }
        }

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
                this.ForwardToBackend(connection.SessionID, request);
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
            if (string.IsNullOrEmpty(address)) return null;
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
            this.ForwardToBackend(_frontID, request);
            _lastHeartBeatSend = DateTime.Now;
        }

        /// <summary>
        /// 将数据发送到客户端
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        public void ForwardToClient(IConnection conn, byte[] data)
        {
            clientDataBuffer.Write(new ClientDataItem() { Connection = conn, Data = data });
            NewWorkerItem();
        }

        /// <summary>
        /// 将数据发送到后端服务器
        /// </summary>
        /// <param name="address"></param>
        /// <param name="packet"></param>
        public void ForwardToBackend(string address, IPacket packet)
        {
            if (_backend != null)
            {
                lock (_obj)
                {
                    try
                    {
                        using (ZMessage zmsg = new ZMessage())
                        {
                            ZError error;
                            zmsg.Add(new ZFrame(Encoding.UTF8.GetBytes(address)));
                            zmsg.Add(new ZFrame(packet.Data));
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
                    catch (Exception ex)
                    {
                        logger.Error("ForwardToBackend Error:" + ex.ToString() + ex.StackTrace);
                    }
                }
            }
        }
       

        public DateTime LastHeartBeatRecv { get { return _lastHeartBeatRecv; } }

        DateTime _lastHeartBeatSend = DateTime.Now;
        DateTime _lastHeartBeatRecv = DateTime.Now;
        DateTime _lastPollTime = DateTime.Now;
        string _frontID = string.Empty;
        Random rd = new Random();
        void PollProcess()
        {
            _lastHeartBeatRecv = DateTime.Now;
            _lastHeartBeatSend = DateTime.Now;
            _stopped = false;
            using(var ctx = new ZContext())
            {
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
                    while (_pollGo)
                    {
                        try
                        {
                            if (DateTime.Now.Subtract(_lastPollTime).TotalSeconds > 10)
                            {
                                logger.Info("--> Poll thread live");
                                _lastPollTime = DateTime.Now;
                            }

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
                                                    IConnection conn = GetConnection(notify.SessionID);
                                                    if (conn != null)
                                                    {
                                                        conn.Close();
                                                        this.LogicUnRegister(notify.SessionID);
                                                    }
                                                    
                                                }
                                                if (packet.Type == MessageTypes.NOTIFYREBOOTMQSRV)
                                                {
                                                    _pollGo = false;
                                                    logger.Info("Reboot MQServer.....");
                                                }
                                            }
                                            else //其余客户端地址为 UUID表示 转发数据到客户端
                                            {
                                                logicMessageBuffer.Write(new LogicMessageItem() { SessionID = clientId, Packet = packet });
                                                NewWorkerItem();
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
                            logger.Error("Poll Message Error:"+ex.ToString()+ ex.StackTrace);
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

    class LogicMessageItem
    {
        public string SessionID { get; set; }

        public IPacket Packet { get; set; }
    }

    class ClientDataItem
    {
        public IConnection Connection { get; set; }

        public byte[] Data { get; set; }
    }
}
