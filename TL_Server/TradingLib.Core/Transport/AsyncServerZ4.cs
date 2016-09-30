using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using ZeroMQ;


namespace TradingLib.Core
{
    /// <summary>
    /// 系统服务端的数据传组件,用于建立底层的数据交换业务
    /// AsyncServer包含行情分发与交易消息路由2大部分
    /// 行情分发采用pub-sub通讯模式,交易消息路由采用router-dealer异步模式
    /// </summary>
    public class AsyncServerZ4 : BaseSrvObject, ITransport
    {
        /// <summary>
        /// 系统默认Poller超时时间
        /// </summary>
        TimeSpan PollerTimeOut = new TimeSpan(0, 0, 1);

        /// <summary>
        /// 系统Worker线程执行消息处理超时时间
        /// </summary>
        TimeSpan WorkerTimeOut = new TimeSpan(0, 0, 2);
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
        public AsyncServerZ4(string name, string server, int port, int numWorkers = 4, bool pttracker = true, bool verb = false)
            : base(name + "_AsyncSrv")
        {
            _serverip = server;//服务地址
            _port = port;//服务主端口
            _worknum = numWorkers;
            _enableThroutPutTracker = pttracker;
            //VerboseDebugging = verb;//是否输出详细日志

            //zmqTP = new ZeromqThroughPut();
            //zmqTP.SendDebugEvent += new DebugDelegate(msgdebug);

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
            ctx.Shutdown();
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

        ~AsyncServerZ4()
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
                using (ZMessage zmsg = new ZMessage())
                {
                    ZError error;

                    if (!string.IsNullOrEmpty(front))
                    {
                        zmsg.Add(new ZFrame(Encoding.UTF8.GetBytes(front)));
                    }
                    zmsg.Add(new ZFrame(Encoding.UTF8.GetBytes(address)));
                    zmsg.Add(new ZFrame(body));
                    if (!_outputChanel.Send(zmsg, out error))
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

        /// <summary>
        /// 分发行情数据
        /// </summary>
        /// <param name="tick">行情数据</param>
        public void SendTick(Tick k)
        {
            string tickstr = k.Symbol + "^" + TickImpl.Serialize2(k);
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
                using (ZMessage zmsg = new ZMessage())
                {
                    ZError error;
                    zmsg.Add(new ZFrame(msg));

                    if (!_tickpub.Send(zmsg, out error))
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



        //传输层前端
        ZSocket _outputChanel;//用于服务端主动向客户端发送消息
        ZSocket _tickpub;//用于转发Tick数据
        ZContext ctx = null;
        private void MessageRoute()
        {
            workers = new List<Thread>(_worknum);
            using (ctx = new ZContext())
            {
                using (ZSocket frontend = new ZSocket(ctx, ZSocketType.ROUTER))//RouterSocket 逻辑服务端口
                using (ZSocket backend = new ZSocket(ctx, ZSocketType.DEALER))
                using (ZSocket outchannel = new ZSocket(ctx, ZSocketType.DEALER))
                using (ZSocket outClient = new ZSocket(ctx, ZSocketType.DEALER))
                using (ZSocket publisher = new ZSocket(ctx, ZSocketType.PUB))//PubSocket 行情发布端口
                using (ZSocket serviceRep = new ZSocket(ctx, ZSocketType.REP))//RepSocket 服务查询端口
                {
                    frontend.SendHighWatermark = _highWaterMark;
                    frontend.ReceiveHighWatermark = _highWaterMark;
                    backend.SendHighWatermark = _highWaterMark;
                    backend.ReceiveHighWatermark = _highWaterMark;
                    outchannel.SendHighWatermark = _highWaterMark;
                    outchannel.ReceiveHighWatermark = _highWaterMark;
                    outClient.SendHighWatermark = _highWaterMark;
                    outClient.ReceiveHighWatermark = _highWaterMark;
                    serviceRep.SendHighWatermark = _highWaterMark;
                    serviceRep.ReceiveHighWatermark = _highWaterMark;


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
                        object[] o = new object[] { ctx, workerid };
                        workers[workerid].Start(o);
                        ThreadTracker.Register(workers[workerid]);
                    }

                    serviceRep.Linger = new TimeSpan(0);
                    serviceRep.ReceiveTimeout = new TimeSpan(0, 0, 1);
                    serviceRep.Bind("tcp://" + _serverip + ":" + (Port + 1).ToString());

                    List<ZSocket> sockets = new List<ZSocket>();
                    sockets.Add(frontend);
                    sockets.Add(backend);
                    sockets.Add(outchannel);
                    sockets.Add(serviceRep);

                    List<ZPollItem> pollitems = new List<ZPollItem>();
                    pollitems.Add(ZPollItem.CreateReceiver());
                    pollitems.Add(ZPollItem.CreateReceiver());
                    pollitems.Add(ZPollItem.CreateReceiver());
                    pollitems.Add(ZPollItem.CreateReceiver());

                    // Switch messages between sockets
                    ZError error;
                    ZMessage[] incoming;

                    //让线程一直获取由socket发报过来的信息
                    _mainthreadready = true;
                    while (_srvgo)
                    {
                        try
                        {
                            if (sockets.PollIn(pollitems, out incoming, out error, PollerTimeOut))
                            {
                                if (incoming[0] != null)
                                {
                                    //v("front recv msg");
                                    backend.Send(incoming[0]);
                                }
                                if (incoming[1] != null)
                                {
                                    //v("backend recv msg");
                                    frontend.Send(incoming[1]);
                                }
                                if (incoming[2] != null)
                                {
                                    //v("out channel recv msg");
                                    frontend.Send(incoming[2]);
                                }

                                if (incoming[3] != null)
                                {
                                    RepProc(serviceRep, incoming[3]);
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
                            DateTime now = DateTime.Now;
                            if ((now - _lasthb).TotalSeconds >= 5)
                            {
                                this.SendTickHeartBeat();
                                _lasthb = now;
                            }

                        }
                        catch (ZException ex)
                        {
                            logger.Error(PROGRAME + ":MainThread[ZmqExcetion]" + ex.ToString());
                            return;
                        }
                        catch (System.Exception ex)
                        {
                            logger.Error(PROGRAME + ":MainThread[Excetion]" + ex.ToString());
                            return;
                        }

                    }
                    _mainthreadready = false;
                }
            }
        }

        void RepProc(ZSocket rep, ZMessage req)
        {
            byte[] buffer = req.First().Read();
            req.Clear();

            Message msg = Message.gotmessage(buffer);

            logger.Info(string.Format("$$$ RepSrv Got Message Type:{0} Content:{1}", msg.Type, msg.Content));
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

                        using (ZMessage zmsg = new ZMessage())
                        {
                            ZError error;
                            zmsg.Add(new ZFrame(response.Data));
                            if (!rep.Send(zmsg, out error))
                            {
                                if (error == ZError.ETERM)
                                {
                                    logger.Error("got ZError.ETERM,return directly");
                                    return;	// Interrupted
                                }
                                throw new ZException(error);
                            }
                        }
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
            ZContext wctx = (ZContext)list[0];
            int id = int.Parse(list[1].ToString());
            using (ZSocket worker = new ZSocket(wctx, ZSocketType.DEALER))
            {
                //将worker连接到backend用于接收由backend中继转发过来的信息
                worker.Connect("inproc://backend");
                ZError error;
                ZMessage request;
                var poller = ZPollItem.CreateReceiver();

                while (_workergo)
                {
                    try
                    {
                        //v(string.Format("Worker {0} wait message", id));
                        if (null == (request = worker.ReceiveMessage(out error)))
                        {
                            if (error == ZError.ETERM)
                            {
                                logger.Error("Work ZmqSocket TERM");
                                return;	// Interrupted
                            }
                            throw new ZException(error);
                        }
                        else
                        {
                            //v(string.Format("Worker {0} recv message", id));
                            WorkTaskProc(worker, request, id);
                            //v(string.Format("Worker {0} finish", id));
                        }

                    }
                    catch (ZException ex)
                    {
                        logger.Error(string.Format("worker {0} proc zmq error:{1}", id, ex.ToString()));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(string.Format("worker {0} proc error:{1}", id, ex.ToString()));
                    }

                }
            }
        }

        void WorkTaskProc(ZSocket worker, ZMessage request, int id)
        {
            int cnt = request.Count;
            if (cnt == 2 || cnt == 3)
            {
                //1.进行消息地址解析 zmessage 中含有多个frame frame[0]是消息主体,其余frame是附加的地址信息
                string front = cnt == 3 ? request[0].ReadString(Encoding.UTF8) : string.Empty;
                string address = cnt == 3 ? request[1].ReadString(Encoding.UTF8) : request[0].ReadString(Encoding.UTF8);
                Message msg = Message.gotmessage(request.Last().Read());
                request.Clear();

                //消息合法判定
                if (!msg.isValid) return;

                //logger.Info(string.Format("Work {0} Recv Message Type:{1} Content:{2} Front:{3} Address:{4}", id, msg.Type, msg.Content, front, address));

                //处理前置的逻辑连接心跳
                if (cnt == 3 && msg.Type == MessageTypes.LOGICLIVEREQUEST)
                {
                    LogicLiveRequest req = (LogicLiveRequest)PacketHelper.SrvRecvRequest(msg.Type, msg.Content, front, address);
                    LogicLiveResponse rep = ResponseTemplate<LogicLiveResponse>.SrvSendRspResponse(req);

                    using (ZMessage zmsg = new ZMessage())
                    {
                        ZError error;
                        zmsg.Add(new ZFrame(front));
                        zmsg.Add(new ZFrame(address));
                        zmsg.Add(new ZFrame(rep.Data));
                        if (!worker.Send(zmsg, out error))
                        {
                            if (error == ZError.ETERM)
                            {
                                logger.Error("got ZError.ETERM,return directly");
                                return;	// Interrupted
                            }
                            throw new ZException(error);
                        }
                    }
                    return;
                }
                else//处理客户端消息 客户端消息需要检查地址信息
                {
                    if (string.IsNullOrEmpty(address) || address.Length != 36) return;//地址为36字符UUID
                    if (cnt == 3 && string.IsNullOrEmpty(front)) return;//如果通过前置接入 则front不为空
                }

                //2.流控 超过消息频率则直接返回不进行该消息的处理(拒绝该消息) 交易服务器才执行流控,管理服务器不执行
                //if (this._tlmode == QSEnumTLMode.TradingSrv && !zmqTP.NewZmessage(address,zmsg)) 
                //{
                //    return true;
                //}

                //Timeout timeout = new Timeout();
                //timeout.MessageHandler = () =>
                //{
                    //3.消息处理如果解析出来的消息是有效的则丢入处理流程进行处理，如果无效则不处理
                handleMessage(msg.Type, msg.Content, front, address);
                ////};
                ////bool re = timeout.DoWithTimeout(WorkerTimeOut);
                ////if (re)
                //{
                ////    logger.Warn(string.Format("Wroker:{0}  Handle Message TimeOut, type:{1} content:{2}",id, msg.Type, msg.Content));
                //}

            }
        }
    }

    public class Timeout
    {
        private ManualResetEvent mTimeoutObject;
        //标记变量
        private bool mBoTimeout;
        public Action MessageHandler;
        public Timeout(AsyncServerZ4 srv)
        {
            //  初始状态为 停止
            this.mTimeoutObject =new ManualResetEvent(true);
        }
        ///<summary>
        /// 指定超时时间 异步执行某个方法
        ///</summary>
        ///<returns>执行 是否超时</returns>
        public bool DoWithTimeout(TimeSpan timeSpan)
        {
            if (this.MessageHandler == null)
            {
                return false;
            }
            this.mTimeoutObject.Reset();
            this.mBoTimeout =true; //标记
            this.MessageHandler.BeginInvoke(DoAsyncCallBack, null);
            // 等待 信号Set
            if (!this.mTimeoutObject.WaitOne(timeSpan, false))
            {
                this.mBoTimeout =true;
            }
            return this.mBoTimeout;
        }
        ///<summary>
        /// 异步委托 回调函数
        ///</summary>
        ///<param name="result"></param>
        private void DoAsyncCallBack(IAsyncResult result)
        {
            try
            {
                this.MessageHandler.EndInvoke(result);
                // 指示方法的执行未超时
                this.mBoTimeout =false; 
            }
            catch (Exception ex)
            {
                
                this.mBoTimeout =true;
            }
            finally
            {
                this.mTimeoutObject.Set();
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