using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using ZeroMQ;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 系统服务端的数据传组件,用于建立底层的数据交换业务
    /// AsyncServer包含行情分发与交易消息路由2大部分
    /// 行情分发采用pub-sub通讯模式,交易消息路由采用router-dealer异步模式
    /// </summary>
    public class AsyncServer : BaseSrvObject, ITransport
    {
        const int TimoutSecend = 1;

        /// <summary>
        /// 系统默认Poller超时时间
        /// </summary>
        TimeSpan PollerTimeOut = new TimeSpan(0, 0, TimoutSecend);

        /// <summary>
        /// 交易消息事件,当接收到客户端发送上来的消息时,触发该事件,从而调用消息层对消息进行解析与处理
        /// 传递参数消息类别(操作号),消息体,前置地址,客户端标识
        /// </summary>
        public event HandleTLMessageDel GotTLMessageEvent;

        bool _enableThroutPutTracker = true;
        /// <summary>
        /// 是否启用消息流控
        /// </summary>
        public bool EnableTPTracker { get { return _enableThroutPutTracker; } set { _enableThroutPutTracker = value; } }
        /// <summary>
        /// 触发消息事件,调用外层消息处理逻辑进行消息解析与处理
        /// </summary>
        /// <param name="type">消息类别</param>
        /// <param name="msg">消息体</param>
        /// <param name="front">前置地址</param>
        /// <param name="address">客户端地址/标识</param>
        /// <returns></returns>
        private long handleMessage(MessageTypes type, string msg, string front, string address)
        {
            if (GotTLMessageEvent != null)
            {
                return GotTLMessageEvent(type, msg, front, address);
            }
            return -1;
        }


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
        /// AsyncServer构造函数
        /// </summary>
        /// <param name="name">服务对象标识</param>
        /// <param name="server">服务监听地址</param>
        /// <param name="port">服务端口</param>
        /// <param name="numWorkers">开启工作线程数</param>
        /// <param name="pttracker">是否启用流控</param>
        /// <param name="verb"></param>
        public AsyncServer(string name, string server, int port, int numWorkers = 4, bool pttracker = true, bool verb = false)
            : base(name + "_AsyncSrv")
        {
            _serverip = server;//服务地址
            _port = port;//服务主端口
            _worknum = numWorkers;
            _enableThroutPutTracker = pttracker;
            //VerboseDebugging = verb;//是否输出详细日志

            zmqTP = new ZeromqThroughPut();
            //zmqTP.SendDebugEvent += new DebugDelegate(msgdebug);

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

            //这里等待线程启动完毕后再返回,线程启动完毕后方可利用该组件进行消息发送
            //_mainthreadready标志在程序进入poll段后置true
            int _wait = 0;
            while ((_mainthreadready != true) && (_wait++ < 5))
            {
                logger.Info("#:" + _wait.ToString() + "SrvThread:" + _mainthreadready.ToString() + "AsyncServer is starting.....");
                Thread.Sleep(500);
            }
            //当主线程与名成查询线程全部启动时候我们认为服务启动完毕否则我们抛出异常
            if (IsLive)
            {
                logger.Info("Starting " + PROGRAME + " successfull");
                _started = true;
            }
            else
            {
                logger.Info("Starting " + PROGRAME + " Failed");
                throw new QSAsyncServerError();
            }
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

        ~AsyncServer()
        {
            try
            {
                Stop();

            }
            catch { }
        }


        /// <summary>
        /// 向某个客户端发送消息
        /// </summary>
        /// <param name="body">消息内容(Message对应的内存块)</param>
        /// <param name="address">客户端地址/标识</param>
        /// <param name="front">对应前置地址,前置地址为空/null则客户端直接连接到本地Router</param>
        public void Send(byte[] body, string address, string front)
        {
            if (_outputChanel == null)
                return;
            lock (_outputChanel)
            {
                ZMessage zmsg = new ZMessage(body);
                //将消息地址与消息绑定，通过outputChanel发送出去,这样frontend就会根据消息地址自动的将消息发送给对应的客户端
                zmsg.Wrap(Encoding.UTF8.GetBytes(address), null);
                //如果front不为空或者null,则我们再继续添加路由地址
                if (!string.IsNullOrEmpty(front))
                {
                    zmsg.Wrap(Encoding.UTF8.GetBytes(front), null);
                }
                zmsg.Send(_outputChanel);
            }
        }

        /// <summary>
        /// 分发行情数据
        /// </summary>
        /// <param name="tick">行情数据</param>
        public void SendTick(Tick k)
        {
            string tickstr = k.Symbol + "^" + TickImpl.Serialize(k);
            SendTick(tickstr);
        }

        /// <summary>
        /// 发送Tick 心跳数据
        /// 非线程安全
        /// </summary>
        void SendTickHeartBeat()
        {
            string tickstr = "TICKHEARTBEAT";
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
                _tickpub.Send(msg, Encoding.UTF8);
            }
        }



        //传输层前端
        ZmqSocket _outputChanel;//用于服务端主动向客户端发送消息
        ZmqSocket _tickpub;//用于转发Tick数据

        private void MessageRoute()
        {
            workers = new List<Thread>(_worknum);
            using (ZmqContext context = ZmqContext.Create())
            {   
                using (ZmqSocket frontend = context.CreateSocket(SocketType.ROUTER), backend = context.CreateSocket(SocketType.DEALER), outchannel = context.CreateSocket(SocketType.DEALER), outClient = context.CreateSocket(SocketType.DEALER), publisher = context.CreateSocket(SocketType.PUB), serviceRep = context.CreateSocket(SocketType.REP))
                {
                    
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
                        object[] o = new object[] { context, workerid };
                        workers[workerid].Start(o);
                        ThreadTracker.Register(workers[workerid]);
                    }
                    //fronted过来的信息我们路由到backend上去
                    frontend.ReceiveReady += (s, e) =>
                    {
                        logger.Info("front recv message");
                        var zmsg = new ZMessage(e.Socket);
                        zmsg.Send(backend);
                    };
                    //backend过来的信息我们路由到frontend上去
                    backend.ReceiveReady += (s, e) =>
                    {
                        //logger.Info("backend recv message");
                        var zmsg = new ZMessage(e.Socket);
                        zmsg.Send(frontend);
                    };
                    //output接受到的消息,我们通过front发送出去,所有发送给客户端的消息通过outClient发送到output,然后再通过front路由出去
                    outchannel.ReceiveReady += (s, e) =>
                    {
                        //logger.Info("out channel recv message");
                        var zmsg = new ZMessage(e.Socket);
                        zmsg.Send(frontend);
                    };

                    //服务查询
                    byte[] buffer = new byte[0];
                    int size = 0;

                    serviceRep.Linger = new TimeSpan(0);
                    serviceRep.ReceiveTimeout = new TimeSpan(0, 0, 1);
                    serviceRep.Bind("tcp://" + _serverip + ":" + (Port + 1).ToString());
                    serviceRep.ReceiveReady += (s, e) =>
                    {
                        try
                        {
                            buffer = serviceRep.Receive(buffer, SocketFlags.DontWait, out size);
                            Message msg = Message.gotmessage(buffer);
                            logger.Info(string.Format("$$$ RepSrv Got Message Type:{0} Content:{1}", msg.Type, msg.Content));
                            //"*** RepSrv Got Message:" + msg.Type.ToString() + "|" + msg.Content.ToString() + "|" + msg.Content.Length.ToString() + " message buffer size:" + size + " recevied size:" + buffer.Length.ToString());
                            //如果消息无效则直接返回
                            if (!msg.isValid)
                                return;
                            switch (msg.Type)
                            {
                                case MessageTypes.BROKERNAMEREQUEST:
                                    {
                                        try
                                        {
                                            BrokerNameRequest request = RequestTemplate<BrokerNameRequest>.SrvRecvRequest("", "", msg.Content);
                                            BrokerNameResponse response = ResponseTemplate<BrokerNameResponse>.SrvSendRspResponse(request);
                                            response.Provider = Providers.QSPlatform;
                                            response.BrokerName = "Dev-2-Platform";
                                            serviceRep.Send(response.Data);
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error("error:" + ex.ToString());
                                        }
                                    }
                                    break;
                                default:
                                    return;//其他类型的消息直接返回
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error("deal wektask error:" + ex.ToString());
                        }

                    };

                    var poller = new Poller(new List<ZmqSocket> { frontend, backend, outchannel, serviceRep });
                    //让线程一直获取由socket发报过来的信息
                    _mainthreadready = true;
                    while (_srvgo)
                    {
                        try
                        {
                            poller.Poll(PollerTimeOut);//设定poll的time out可以防止该线程一直阻塞在poll，导致线程无法停止
                            if (!_srvgo)
                            {
                                logger.Info("messageroute thread stop,try to clear socket");
                                frontend.Close();
                                backend.Close();
                                publisher.Close();
                                outchannel.Close();
                                outClient.Close();
                                serviceRep.Close();
                            }
                            else//如果服务没有停止
                            {
                                DateTime now = DateTime.Now;
                                if ((now - _lasthb).TotalSeconds >= 5)
                                {
                                    this.SendTickHeartBeat();
                                    _lasthb = now;
                                }
                            }
                        }
                        catch (ZmqException e)
                        {
                            logger.Error(PROGRAME + ":MainThread[ZmqExcetion]" + e.ToString());
                        }
                        catch (System.Exception ex)
                        {
                            logger.Error(PROGRAME + ":MainThread[Excetion]" + ex.ToString());
                        }

                    }
                    _mainthreadready = false;
                }
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
            ZmqContext wctx = (ZmqContext)list[0];
            int id = int.Parse(list[1].ToString());
            using (ZmqSocket worker = wctx.CreateSocket(SocketType.DEALER))
            {
                //将worker连接到backend用于接收由backend中继转发过来的信息
                worker.Connect("inproc://backend");
                worker.ReceiveReady += (s, e) =>
                {
                    MessageProcess(worker, id);
                };
                var poller = new Poller(new List<ZmqSocket> { worker });

                while (_workergo)
                {
                    try
                    {
                        poller.Poll(PollerTimeOut);
                        if (!_workergo)
                        {
                            logger.Info(string.Format("worker thread[{0}] stopped,close worker socket", id));
                            worker.Close();
                        }

                    }
                    catch (ZmqException e)
                    {
                        logger.Error(string.Format("worker {0} proc error:", id) + e.ToString());
                    }
                }
            }
        }


        ZeromqThroughPut zmqTP;//流控管理器,用于跟踪所有客户端连接的消息流
        //Worker消息处理函数
        void MessageProcess(ZmqSocket worker, int id)
        {
            try
            {
                ZMessage zmsg = new ZMessage(worker);
                //1.进行消息地址解析 zmessage 中含有多个frame frame[0]是消息主体,其余frame是附加的地址信息
                //通过zmessage frame数量判断,获得对应的地址信息
                string front = string.Empty;
                string address = string.Empty;
                Message msg = Message.gotmessage(zmsg.Body);
                logger.Info(string.Format("*** Frame count:{0} type:{1} content:{2}", zmsg.FrameCount, msg.Type, msg.Content));

                //("Frames Count:" + zmsg.FrameCount.ToString() + " body:" + UTF8Encoding.Default.GetString(zmsg.Body) + " add:" + UTF8Encoding.Default.GetString(zmsg.Address),QSEnumDebugLevel.INFO);
                //如果消息类型是前置发送到交易逻辑服务的心跳
                if (msg.Type == MessageTypes.LOGICLIVEREQUEST)
                {
                    if (zmsg.FrameCount == 3)
                    {
                        front = zmsg.AddressToString();//获得第一层地址
                        zmsg.Unwrap();//分离第一层地址
                        address = zmsg.AddressToString();
                        LogicLiveRequest request = (LogicLiveRequest)PacketHelper.SrvRecvRequest(msg.Type, msg.Content, front, address);
                        LogicLiveResponse response = ResponseTemplate<LogicLiveResponse>.SrvSendRspResponse(request);

                        ZMessage reply = new ZMessage(response.Data);
                        //将消息地址与消息绑定，通过outputChanel发送出去,这样frontend就会根据消息地址自动的将消息发送给对应的客户端
                        reply.Wrap(Encoding.UTF8.GetBytes(address), null);
                        //如果front不为空或者null,则我们再继续添加路由地址
                        if (!string.IsNullOrEmpty(front))
                        {
                            reply.Wrap(Encoding.UTF8.GetBytes(front), null);
                        }
                        reply.Send(worker);
                    }
                    return;

                }

                //根据Frame数量进行地址检查
                if (zmsg.FrameCount == 3)
                {
                    front = zmsg.AddressToString();//获得第一层地址
                    zmsg.Unwrap();//分离第一层地址
                    address = zmsg.AddressToString();
                    //debug("Got 3 frame " + "front:" + front + " address:" + address, QSEnumDebugLevel.MUST);
                    if (string.IsNullOrEmpty(front) || string.IsNullOrEmpty(address) || address.Length != 36) return;
                }
                //带有1层地址,客户直接连接到本地的router
                else if (zmsg.FrameCount == 2)
                {
                    address = zmsg.AddressToString();
                    //debug("Got 2 frame " + "front:" + front + " address:" + address, QSEnumDebugLevel.MUST);
                    if (string.IsNullOrEmpty(address) || address.Length != 36) return;
                }
                else
                {
                    return;
                }
               

                //2.流控 超过消息频率则直接返回不进行该消息的处理(拒绝该消息) 交易服务器才执行流控,管理服务器不执行
                //if (this._tlmode == QSEnumTLMode.TradingSrv && !zmqTP.NewZmessage(address,zmsg)) 
                //{
                //    return true;
                //}

                //3.消息处理如果解析出来的消息是有效的则丢入处理流程进行处理，如果无效则不处理

                //debug(string.Format("[AsyncServerMQ] Worker_{0} Got Message,FrameCount:{1} Front:{2} Address:{3} Type:{4} Content:{5}", id, zmsg.FrameCount,front, address, msg.Type, msg.Content),QSEnumDebugLevel.INFO);
                if (msg.isValid)
                    handleMessage(msg.Type, msg.Content, front, address);//处理消息按照消息类型进行消息路由,如果没有该消息则则会被系统过滤掉
                return;
            }
            catch (ZmqException e)
            {
                logger.Error(PROGRAME + ":Worker[" + id.ToString() + "][ZmqExcetion]" + e.ToString());
                return;
            }
            //捕捉QSTradingServerError(客户端向服务端请求操作所引发的异常)
            catch (QSTradingServerError ex)
            {
                logger.Error(PROGRAME + ":Worker[" + id.ToString() + "][QSExcetion]" + ex.Label + " " + ex.RawException.Message);
                return;
            }
            catch (System.Exception ex)
            {
                logger.Error(PROGRAME + ":Worker[" + id.ToString() + "][Excetion]" + ex.ToString());
                return;
            }


        }
    }
}
/*
 *      1.0版本的消息发送与接受
 *      发送
        public void Send(byte[] body, string address)
        {
            ZMessage zmsg = new ZMessage(body);
            //将消息地址与消息绑定，通过outputChanel发送出去,这样frontend就会根据消息地址自动的将消息发送给对应的客户端
            zmsg.Wrap(Encoding.Unicode.GetBytes(address), Encoding.Unicode.GetBytes(""));
            zmsg.Send(_outputChanel);
        }
        地址加入的时候 
        frame[2] address
        frame[1] ""
        frame[0] msg
        地址与消息中间有一个empty frame
        
        接受
        //2.进行消息处理
        string address = zmsg.AddressToString();//获得zmsg的发送客户端地址,这里需要做流控
        

        2.0版本中由于引入了 路由接入。地址被严格限制。每层地址只占一个frame
        因此在消息接收时,需要严格检查地址是否是""
**/