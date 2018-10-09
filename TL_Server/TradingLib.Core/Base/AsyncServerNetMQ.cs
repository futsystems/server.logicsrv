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
        /// 消息处理工作线程数目
        /// </summary>
        private int _worknum = 1;

        /// <summary>
        /// 服务监听地址
        /// </summary>
        private string _serverip = string.Empty;

        /// <summary>
        /// 服务监听基准端口
        /// 比如5570为交易消息监听端口,5571为服务查询端口,5572为行情分发端口
        /// </summary>
        private int _port = 5570;

        bool _verbose = false;

        int _highWaterMark = 1000000;

        /// <summary>
        /// 主服务线程
        /// </summary>
        Thread _srvThread;

        /// <summary>
        /// worker线程
        /// </summary>
        List<Thread> workers;

        NetMQPoller mainPoller = null;
        List<NetMQPoller> workerPollers = new List<NetMQPoller>();


        /// <summary>
        /// AsyncServer构造函数
        /// </summary>
        /// <param name="name">服务对象标识</param>
        /// <param name="server">服务监听地址</param>
        /// <param name="port">服务端口</param>
        /// <param name="numWorkers">开启工作线程数</param>
        /// <param name="pttracker">是否启用流控</param>
        /// <param name="verb"></param>
        public AsyncServerNetMQ(string name, string server, int port, int numWorkers = 4)
            : base(name + "_AsyncSrv")
        {
            _serverip = server;//服务地址
            _port = port;//服务主端口
            _worknum = numWorkers;

            //1.加载配置文件
            ConfigDB _cfgdb = new ConfigDB(PROGRAME);
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
        /// 服务是否正常启动
        /// </summary>
        public bool IsLive { get { return _srvThread.IsAlive; } }


        bool _running = false;
        public void Start()
        {
            if (_running)
                return;
            logger.Info("Start Message Transport Service @" + PROGRAME);
            //启动主服务线程
            _running = true;
            _srvThread = new Thread(new ThreadStart(MessageRoute));
            _srvThread.IsBackground = true;
            _srvThread.Name = "AsyncMessageRoute@" + PROGRAME;
            _srvThread.Start();
            ThreadTracker.Register(_srvThread);
        }

        public void Stop()
        {
            if (!_running)
                return;
            _running = false;
            //停止工作线程
            logger.Info(string.Format("Stop MessageRouter Service[{0}]", PROGRAME));
            logger.Info("1.Stop WorkerThreads");
            for (int i = 0; i < workers.Count; i++)
            {
                workerPollers[i].Stop();

                if (!workers[i].IsAlive)
                    logger.Info("worker[" + i.ToString() + "] stopped successfull");
            }

            //停止主消息路由线程
            logger.Info("2.Stop RouteThread");
            mainPoller.Stop();
            if (!IsLive)
            {
                logger.Info("MainThread stopped successfull");
            }
        }

        /// <summary>
        /// 发送数据到某个地址对应的客户端
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="address"></param>
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
                //serviceRep.Options.SendHighWatermark = _highWaterMark;
                //serviceRep.Options.ReceiveHighWatermark = _highWaterMark;


                //前端Router用于注册Client
                frontend.Bind("tcp://" + _serverip + ":" + _port.ToString());
                //后端用于向worker线程发送消息,worker再去执行
                backend.Bind("inproc://backend");
                //用于系统对外发送消息
                outchannel.Bind("inproc://output");
                //对外发送消息的对端socket
                _outputChanel = outClient;
                outClient.Connect("inproc://output");

                //tick数据转发
                publisher.Bind("tcp://*:" + (_port + 2).ToString());
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

                serviceRep.Options.Linger = new TimeSpan(1);
                serviceRep.Bind("tcp://" + _serverip + ":" + (_port + 1).ToString());

                NetMQTimer timer = new NetMQTimer(TimeSpan.FromSeconds(5));

                mainPoller = new NetMQPoller { frontend, backend, outchannel, serviceRep, timer };
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

                //定时发送心跳
                timer.Elapsed += (s,e) =>
                {
                    this.SendTickHeartBeat();
                };

                mainPoller.Run();
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
                workerPollers.Add(poller);
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
            if (cnt == 2)
            {
                string address = request.First.ConvertToString(Encoding.UTF8);
                Message msg = Message.gotmessage(request.Last.Buffer);

                //消息合法判定
                if (!msg.isValid) return;
#if DEBUG
                logger.Info(string.Format("Work {0} Recv Message Type:{1} Content:{2}  Address:{3}", id, msg.Type, msg.Content, address));
#endif

                if (string.IsNullOrEmpty(address) || address.Length != 36) return;//地址为36字符UUID

                IPacket packet = PacketHelper.SrvRecvRequest(msg, "", address);
                if (NewPacketEvent != null)
                {
                    NewPacketEvent(packet, address);
                }
            }
        }
    }
}