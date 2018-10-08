using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using NetMQ;


namespace TradingLib.Core
{
    /// <summary>
    /// 系统服务端的数据传组件,用于建立底层的数据交换业务
    /// AsyncServer包含行情分发与交易消息路由2大部分
    /// 行情分发采用pub-sub通讯模式,交易消息路由采用router-dealer异步模式
    /// </summary>
    public class AsyncServerNetMQ : BaseSrvObject, ITransport
    {
        public event Action<IPacket, string> NewPacketEvent;

        /// <summary>
        /// 系统默认Poller超时时间
        /// </summary>
        TimeSpan PollerTimeOut = new TimeSpan(0, 0, 1);

        /// <summary>
        /// 系统Worker线程执行消息处理超时时间
        /// </summary>
        TimeSpan WorkerTimeOut = new TimeSpan(0, 0, 2);

        bool _enableThroutPutTracker = true;
        /// <summary>
        /// 是否启用消息流控
        /// </summary>
        public bool EnableTPTracker { get { return _enableThroutPutTracker; } set { _enableThroutPutTracker = value; } }
        


        
        /// <summary>
        /// 消息处理工作线程数目
        /// </summary>
        public int NumWorkers { get { return _worknum; } set { _worknum = (value > 1 ? value : 1); } }
        private int _worknum = 1;

        /// <summary>
        /// 服务监听地址
        /// </summary>
        public string ServerIP { get { return _serverip; } set { _serverip = value; } }
        private string _serverip = string.Empty;

        /// <summary>
        /// 服务监听基准端口
        /// 比如5570为交易消息监听端口,5571为服务查询端口,5572为行情分发端口
        /// </summary>
        public int Port { get { return _port; } set { _port = value; } }
        private int _port = 5570;

        //服务端名称查询 用于客户端检测是否存在我们系统内的服务器
        Providers _pn = Providers.Unknown;
        public Providers ProviderName { get { return _pn; } set { _pn = value; } }

        /// <summary>
        /// 前置列表
        /// </summary>
        List<string> frontList = new List<string>();

        ConfigDB _cfgdb;
        bool _verbose = false;
        int _highWaterMark = 1000000;
        /// <summary>
        /// AsyncServer构造函数
        /// </summary>
        /// <param name="name">服务对象标识</param>
        /// <param name="server">服务监听地址</param>
        /// <param name="port">服务端口</param>
        /// <param name="numWorkers">开启工作线程数</param>
        /// <param name="pttracker">是否启用流控</param>
        /// <param name="verb"></param>
        public AsyncServerNetMQ(string name, string server, int port, int numWorkers = 4, bool pttracker = true, bool verb = false)
            : base(name + "_AsyncSrv")
        {
            _serverip = server;//服务地址
            _port = port;//服务主端口
            _worknum = numWorkers;
            _enableThroutPutTracker = pttracker;
            //VerboseDebugging = verb;//是否输出详细日志

            //1.加载配置文件
            _cfgdb = new ConfigDB(PROGRAME);
            if (!_cfgdb.HaveConfig("Verbose"))
            {
                _cfgdb.UpdateConfig("Verbose", QSEnumCfgType.Bool, false, "是否打印底层通讯详细信息");
            }
            _verbose = _cfgdb["Verbose"].AsBool();


            if (!_cfgdb.HaveConfig("HighWaterMark"))
            {
                _cfgdb.UpdateConfig("HighWaterMark", QSEnumCfgType.Int, 20000, "系统发送水位");
            }
            _highWaterMark = _cfgdb["HighWaterMark"].AsInt();

            

        }

        /// <summary>
        /// 详细输出日志信息
        /// </summary>
        /// <param name="msg"></param>
        void v(string msg)
        {
            if (_verbose)
            {
                logger.Debug(msg);
            }
        }
        public void Send(IPacket packet, string address)
        {
            if (_outputChanel == null)
                return;
            lock (_outputChanel)
            {
                NetMQMessage zmsg = new NetMQMessage();
                zmsg.Append(Encoding.UTF8.GetBytes(address));
                zmsg.Append(packet.Data);

                _outputChanel.SendMultipartMessage(zmsg);
            }
        }

        /// <summary>
        /// 主服务线程
        /// </summary>
        Thread _srvThread;

        /// <summary>
        /// worker线程
        /// </summary>
        List<Thread> workers;

        bool _srvgo = false;//路由线程运行标志
        bool _workergo = false;//worker线程运行标志
        bool _started = false;

        bool _mainthreadready = false;

        public void Start()
        {
            if (_started)
                return;
            logger.Info("Start Message Transport Service @" + PROGRAME);
            //启动主服务线程
            _workergo = true;
            _srvgo = true;
            _srvThread = new Thread(new ThreadStart(MessageRoute));
            _srvThread.IsBackground = true;
            _srvThread.Name = "AsyncMessageRoute@" + PROGRAME;
            _srvThread.Start();
            ThreadTracker.Register(_srvThread);
        }

        /// <summary>
        /// 服务是否正常启动
        /// </summary>
        public bool IsLive { get { return _srvThread.IsAlive; } }


        public void Stop()
        {
            if (!_started)
                return;

            //停止工作线程
            logger.Info(string.Format("Stop MessageRouter Service[{0}]", PROGRAME));
            logger.Info("1.Stop WorkerThreads");
            //ctx.Shutdown();
            _workergo = false;
            for (int i = 0; i < workers.Count; i++)
            {
                int workwait = 0;
                while (workers[i].IsAlive && workwait < 10)
                {
                    logger.Info(string.Format("#{0} wait worker[{1}]  stopping....", workwait, i));
                    Thread.Sleep(1000);
                    workwait++;
                }
                if (!workers[i].IsAlive)
                    logger.Info("worker[" + i.ToString() + "] stopped successfull");
            }

            //停止主消息路由线程
            logger.Info("2.Stop RouteThread");
            _srvgo = false;
            int mainwait = 0;
            while (IsLive && mainwait < 10)
            {
                logger.Info(string.Format("#{0} wait mainthread stopping....", mainwait));
                Thread.Sleep(1000);
                mainwait++;
            }
            if (!IsLive)
            {
                logger.Info("MainThread stopped successfull");
            }
        }

        /// <summary>
        /// 分发行情数据
        /// </summary>
        /// <param name="tick">行情数据</param>
        public void Publish(Tick k)
        {
            string tickstr = TickImpl.Serialize2(k);
            SendTick(tickstr);
        }

        /// <summary>
        /// 发送Tick 心跳数据
        /// 非线程安全
        /// </summary>
        void SendTickHeartBeat()
        {
            string tickstr = "H,";
            SendTick(tickstr);
        }

        DateTime _lasthb = DateTime.Now;
        /// <summary>
        /// 通过tickpub socket 对外转发数据
        /// </summary>
        /// <param name="msg"></param>
        void SendTick(string msg)
        {
            if (_tickpub == null)
                return;
            lock (_tickpub)
            {
                _tickpub.SendFrame(msg,false);
            }
        }

        public void DropClient(string clientId)
        { 
        
        }

        //传输层前端
        NetMQ.Sockets.DealerSocket _outputChanel;//用于服务端主动向客户端发送消息
        NetMQ.Sockets.PublisherSocket _tickpub;//用于转发Tick数据
        private void MessageRoute()
        {
            workers = new List<Thread>(_worknum);

            using (NetMQ.Sockets.RouterSocket frontend = new NetMQ.Sockets.RouterSocket())//RouterSocket 逻辑服务端口
            using (NetMQ.Sockets.DealerSocket backend = new NetMQ.Sockets.DealerSocket())
            using (NetMQ.Sockets.DealerSocket outchannel = new NetMQ.Sockets.DealerSocket())
            using (NetMQ.Sockets.DealerSocket outClient = new NetMQ.Sockets.DealerSocket())
            using (NetMQ.Sockets.PublisherSocket publisher = new NetMQ.Sockets.PublisherSocket())//PubSocket 行情发布端口
            using (NetMQ.Sockets.ResponseSocket serviceRep = new NetMQ.Sockets.ResponseSocket())//RepSocket 服务查询端口
            {
                frontend.Options.SendHighWatermark = _highWaterMark;
                frontend.Options.ReceiveHighWatermark = _highWaterMark;
                backend.Options.SendHighWatermark = _highWaterMark;
                backend.Options.ReceiveHighWatermark = _highWaterMark;
                outchannel.Options.SendHighWatermark = _highWaterMark;
                outchannel.Options.ReceiveHighWatermark = _highWaterMark;
                outClient.Options.SendHighWatermark = _highWaterMark;
                outClient.Options.ReceiveHighWatermark = _highWaterMark;
                serviceRep.Options.SendHighWatermark = _highWaterMark;
                serviceRep.Options.ReceiveHighWatermark = _highWaterMark;


                //前端Router用于注册Client
                frontend.Bind("tcp://" + _serverip + ":" + Port.ToString());
                //后端用于向worker线程发送消息,worker再去执行
                backend.Bind("inproc://backend");
                //用于系统对外发送消息
                outchannel.Bind("inproc://output");
                //对外发送消息的对端socket
                _outputChanel = outClient;
                outClient.Connect("inproc://output");

                //tick数据转发
                publisher.Bind("tcp://*:" + (Port + 2).ToString());
                _tickpub = publisher;

                //管理服务器只开一个线程用于处理消息 通过设置worknum实现
                for (int workerid = 0; workerid < _worknum; workerid++)
                {
                    workers.Add(new Thread(MessageWorkerProc));
                    workers[workerid].IsBackground = true;
                    workers[workerid].Name = "MessageDealWorker#" + workerid.ToString() + "@" + PROGRAME; ;
                    object[] o = new object[] { workerid };
                    workers[workerid].Start(o);
                    ThreadTracker.Register(workers[workerid]);
                }

                serviceRep.Options.Linger = new TimeSpan(0);
                serviceRep.Bind("tcp://" + _serverip + ":" + (Port + 1).ToString());

                NetMQ.NetMQPoller poller = new NetMQPoller{frontend,backend,outchannel,serviceRep};
                frontend.ReceiveReady +=(s,a)=>
                {
                    try
                    {
                        backend.SendMultipartMessage(a.Socket.ReceiveMultipartMessage());
                    }
                    catch (NetMQException ex)
                    {
                        logger.Error(PROGRAME + ":MainThread[ZmqExcetion]" + ex.ToString());
                        return;
                    }
                    catch (System.Exception ex)
                    {
                        logger.Error(PROGRAME + ":MainThread[Excetion]" + ex.ToString());
                        return;
                    }
                };
                backend.ReceiveReady += (s, a) =>
                {
                    try
                    {
                        frontend.SendMultipartMessage(a.Socket.ReceiveMultipartMessage());
                    }
                    catch (NetMQException ex)
                    {
                        logger.Error(PROGRAME + ":MainThread[ZmqExcetion]" + ex.ToString());
                        return;
                    }
                    catch (System.Exception ex)
                    {
                        logger.Error(PROGRAME + ":MainThread[Excetion]" + ex.ToString());
                        return;
                    }
                };

                outchannel.ReceiveReady += (s, a) =>
                {
                    try
                    {
                        frontend.SendMultipartMessage(a.Socket.ReceiveMultipartMessage());
                    }
                    catch (NetMQException ex)
                    {
                        logger.Error(PROGRAME + ":MainThread[ZmqExcetion]" + ex.ToString());
                        return;
                    }
                    catch (System.Exception ex)
                    {
                        logger.Error(PROGRAME + ":MainThread[Excetion]" + ex.ToString());
                        return;
                    }
                };
                serviceRep.ReceiveReady+= (s, a) =>
                {
                    try
                    {
                        RepProc(serviceRep, a.Socket.ReceiveMultipartMessage());
                    }
                    catch (NetMQException ex)
                    {
                        logger.Error(PROGRAME + ":MainThread[ZmqExcetion]" + ex.ToString());
                        return;
                    }
                    catch (System.Exception ex)
                    {
                        logger.Error(PROGRAME + ":MainThread[Excetion]" + ex.ToString());
                        return;
                    }
                };

                poller.RunAsync();
                //让线程一直获取由socket发报过来的信息
                _mainthreadready = true;
                while (_srvgo)
                {
                    
                    DateTime now = DateTime.Now;
                    if ((now - _lasthb).TotalSeconds >= 5)//每5秒钟发送行情心跳
                    {
                        this.SendTickHeartBeat();
                        _lasthb = now;
                    }
                    Thread.Sleep(500);
                }
                _mainthreadready = false;
            }
            
        }


        void RepProc(NetMQ.Sockets.ResponseSocket rep, NetMQMessage req)
        {
            Message msg = Message.gotmessage(req.First().Buffer);
            logger.Info(string.Format("RepSrv Got Message Type:{0} Content:{1}", msg.Type, msg.Content));
            //如果消息无效则直接返回
            if (!msg.isValid) return;
            switch (msg.Type)
            {
                case MessageTypes.BROKERNAMEREQUEST:
                    {
                        BrokerNameRequest request = RequestTemplate<BrokerNameRequest>.SrvRecvRequest("", "", msg.Content);
                        BrokerNameResponse response = ResponseTemplate<BrokerNameResponse>.SrvSendRspResponse(request);
                        response.Provider = Providers.QSPlatform;
                        response.BrokerName = "Dev-2-Platform";

                        NetMQMessage zmsg = new NetMQMessage();
                        zmsg.Append(new NetMQFrame(response.Data));
                        rep.SendMultipartMessage(zmsg);

                        return;
                    }
                default:
                    return;//其他类型的消息直接返回
            }

        }
        /// <summary>
        /// 传输层消息翻译与分发,在backend中通过启动多个work来提高系统并发能力
        /// 在worker后端的操作需要保证线程安全
        /// </summary>
        /// <param name="olist"></param>
        private void MessageWorkerProc(object olist)
        {
            object[] list = olist as object[];
            int id = int.Parse(list[0].ToString());
            using (NetMQ.Sockets.DealerSocket worker =new NetMQ.Sockets.DealerSocket())
            {
                //将worker连接到backend用于接收由backend中继转发过来的信息
                worker.Connect("inproc://backend");
                NetMQ.NetMQPoller poller = new NetMQPoller { worker };
                worker.ReceiveReady += (s, a) =>
                {
                    try
                    {
                        var zmsg = a.Socket.ReceiveMultipartMessage();
                        WorkTaskProc(worker, zmsg, id);
                    }
                    catch (NetMQException ex)
                    {
                        logger.Error(string.Format("worker {0} proc zmq error:{1}", id, ex.ToString()));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(string.Format("worker {0} proc error:{1}", id, ex.ToString()));
                    }
                };
                poller.Run();
            }
        }

        

        void WorkTaskProc(NetMQ.Sockets.DealerSocket worker, NetMQMessage request, int id)
        {
            int cnt = request.FrameCount;
            if (cnt == 2 || cnt == 3)
            {
                //1.进行消息地址解析 zmessage 中含有多个frame frame[0]是消息主体,其余frame是附加的地址信息
                string front = cnt == 3 ? request[0].ConvertToString(Encoding.UTF8) : string.Empty;
                string address = cnt == 3 ? request[1].ConvertToString(Encoding.UTF8) : request[0].ConvertToString(Encoding.UTF8);
                Message msg = Message.gotmessage(request.Last().Buffer);
                //request.Clear();

                //消息合法判定
                if (!msg.isValid) return;
#if DEBUG
                logger.Info(string.Format("Work {0} Recv Message Type:{1} Content:{2} Front:{3} Address:{4}", id, msg.Type, msg.Content, front, address));
#endif
                //处理前置的逻辑连接心跳
                if (cnt == 3 && msg.Type == MessageTypes.LOGICLIVEREQUEST)
                {
                    //将新的前置地址加入到列表
                    if (!frontList.Contains(front)) frontList.Add(front);

                    LogicLiveRequest req = (LogicLiveRequest)PacketHelper.SrvRecvRequest(msg, front, address);
                    LogicLiveResponse rep = ResponseTemplate<LogicLiveResponse>.SrvSendRspResponse(req);

                    NetMQMessage zmsg = new NetMQMessage();
                    zmsg.Append(new NetMQFrame(front));
                    zmsg.Append(new NetMQFrame(address));
                    zmsg.Append(new NetMQFrame(rep.Data));
                    worker.SendMultipartMessage(zmsg);

                    return;
                }
                else//处理客户端消息 客户端消息需要检查地址信息
                {
                    if (string.IsNullOrEmpty(address) || address.Length != 36) return;//地址为36字符UUID
                    if (cnt == 3 && string.IsNullOrEmpty(front)) return;//如果通过前置接入 则front不为空
                }

                //3.消息处理如果解析出来的消息是有效的则丢入处理流程进行处理，如果无效则不处理
                IPacket packet = PacketHelper.SrvRecvRequest(msg, front, address);
                if (NewPacketEvent != null)
                {
                    NewPacketEvent(packet, address);
                }
            }
        }
    }
}