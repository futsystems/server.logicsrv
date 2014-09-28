using System;
using System.Collections.Generic;
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
    public class AsyncServer:BaseSrvObject, ITransport
    {
        const int TimoutSecend = 5;

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
        private long handleMessage(MessageTypes type, string msg,string front, string address)
        {
            if (GotTLMessageEvent != null)
            {
                return GotTLMessageEvent(type, msg,front,address);
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

        /// <summary>
        /// AsyncServer构造函数
        /// </summary>
        /// <param name="name">服务对象标识</param>
        /// <param name="server">服务监听地址</param>
        /// <param name="port">服务端口</param>
        /// <param name="numWorkers">开启工作线程数</param>
        /// <param name="pttracker">是否启用流控</param>
        /// <param name="verb"></param>
        public AsyncServer(string name, string server, int port,int numWorkers=4,bool pttracker = true ,bool verb=false):base(name + "_AsyncSrv")
        {
            _serverip = server;//服务地址
            _port = port;//服务主端口
            _worknum = numWorkers;
            _enableThroutPutTracker = pttracker;
            //VerboseDebugging = verb;//是否输出详细日志

            zmqTP = new ZeromqThroughPut();
            //zmqTP.SendDebugEvent += new DebugDelegate(msgdebug);

        }

        Thread _srvThread;
        Thread _namesThread;
        List<Thread> workers;

        bool _srvgo = false;//路由线程运行标志
        bool _workergo = false;//worker线程运行标志
        bool _started = false;

        bool _mainthreadready = false;
        bool _serverDNSthreadready = false;
        //这里需要注意关闭 启动的细节。防止服务器崩溃
        public void Start()
        {
            if (_started)
                return;
            debug("Start Message Transport Service @" + PROGRAME);
            //启动主服务线程
            _workergo = true;
            _srvgo = true;
            _srvThread = new Thread(new ThreadStart(MessageRoute));
            _srvThread.IsBackground = true;
            _srvThread.Name = "AsyncMessageRoute@" + PROGRAME;
            _srvThread.Start();
            ThreadTracker.Register(_srvThread);


            debug("Start  Names REQ Service @" + PROGRAME);
            //启动服务响应查询线程
            _namesgo = true;
            _namesThread = new Thread(new ThreadStart(NameLookup));
            _namesThread.IsBackground = true;
            _namesThread.Name = "AsyncNamesLookup@" + PROGRAME;
            _namesThread.Start();
            ThreadTracker.Register(_namesThread);

            int _wait = 0;
            //用于等待线程中的相关服务启动完毕 这样函数返回时候服务已经启动完毕 相当于阻塞了线程
            //防止过早返回 服务没有启动造成的程序混乱
            while ((_mainthreadready != true || _serverDNSthreadready != true) && (_wait++ < 5))
            {
                debug("#:" + _wait.ToString() + "mainthread:" + _mainthreadready.ToString() + " dnsthread:" + _serverDNSthreadready.ToString() + "AsyncServer is starting.....");
                Thread.Sleep(500);
            }
            //当主线程与名成查询线程全部启动时候我们认为服务启动完毕否则我们抛出异常
            if (IsLive)
            {
                debug("Starting " + PROGRAME + " successfull");
                _started = true;
            }
            else
            {
                debug("Starting " + PROGRAME + " Failed");
                throw new QSAsyncServerError();
            }
        }

        /// <summary>
        /// 服务是否正常启动
        /// </summary>
        public bool IsLive { get { return IsMainServerAlive && IsNameServerAlive; } }
        /// <summary>
        /// 主线程是否启动
        /// </summary>
        public bool IsMainServerAlive { get { return _srvThread.IsAlive; } }
        /// <summary>
        /// 名称查询线程是否启动
        /// </summary>
        public bool IsNameServerAlive { get { return _namesThread.IsAlive; } }


        public void Stop()
        {
            if (!_started)
                return;
            //停止服务查询端口
            debug(string.Format("Stop Names REQ  Service[{0}]",PROGRAME),QSEnumDebugLevel.INFO);
            _namesgo = false;
            int namewait=0;
            while (IsNameServerAlive && namewait <10)
            {
                debug(string.Format("#{0} wait name req server stopping....",namewait),QSEnumDebugLevel.INFO);
                Thread.Sleep(2000);
                namewait++;
            }
            if (!IsNameServerAlive)
            {
                debug("Names REQ  Service Stopped successfull",QSEnumDebugLevel.INFO);
            }

            //停止工作线程
            debug(string.Format("Stop MessageRouter Service[{0}]",PROGRAME),QSEnumDebugLevel.INFO);
            debug("1.Stop WorkerThreads",QSEnumDebugLevel.INFO);
            _workergo = false;
            for(int i = 0; i < workers.Count; i++)
            {
                int workwait = 0;
                while (workers[i].IsAlive && workwait < 10)
                {
                    debug(string.Format("#{0} wait worker[{1}]  stopping....",workwait,i), QSEnumDebugLevel.INFO);
                    Thread.Sleep(1000);
                    workwait++;
                }
                if (!workers[i].IsAlive)
                    debug("worker[" + i.ToString() + "] stopped successfull",QSEnumDebugLevel.INFO);
            }

            //停止主消息路由线程
            debug("2.Stop RouteThread",QSEnumDebugLevel.INFO);
            _srvgo = false;
            int mainwait=0;
            while (IsMainServerAlive && mainwait < 10)
            {
                debug(string.Format("#{0} wait mainthread stopping....",mainwait), QSEnumDebugLevel.INFO);
                Thread.Sleep(1000);
                mainwait++;
            }
            if (!IsMainServerAlive)
            {
                debug("MainThread stopped successfull", QSEnumDebugLevel.INFO);
            }

            if((!IsMainServerAlive)&&(!IsNameServerAlive))
            {
                debug(string.Format("Stop transport of {0} successfull",PROGRAME),QSEnumDebugLevel.INFO);
            }
            else
            {
                debug(string.Format("Stop transport of {0} error",PROGRAME),QSEnumDebugLevel.ERROR);
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

        //服务端名称查询 用于客户端检测是否存在我们系统内的服务器
        private bool _namesgo;
        Providers _pn = Providers.Unknown;
        public Providers ProviderName { get { return _pn; } set { _pn = value; } }
        private void NameLookup()
        {
            byte[] buffer = new byte[0];
            int size = 0;
            using (var context = ZmqContext.Create())
            {
                using (ZmqSocket rep = context.CreateSocket(SocketType.REP))
                {
                    rep.Linger = new TimeSpan(0);
                    rep.ReceiveTimeout = new TimeSpan(0,0,1);
                    rep.Bind("tcp://" + _serverip + ":" + (Port + 1).ToString());
                    rep.ReceiveReady += (s, e) =>
                    {
                        try
                        {
                            buffer = rep.Receive(buffer,SocketFlags.DontWait,out size);
                            Message msg = Message.gotmessage(buffer);
                            //v("ServerDNS Got Message:" + msg.Type.ToString() + "|" + msg.Content.ToString() + "|" + msg.Content.Length.ToString() + " message buffer size:" + size + " recevied size:" + buffer.Length.ToString());
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
                                            rep.Send(response.Data);
                                        }
                                        catch (Exception ex)
                                        {
                                            debug("error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                                        }
                                        //int id = (int)_pn;
                                        //rep.Send(id.ToString(), Encoding.UTF8);
                                    }
                                    break;
                                ////接受前置机查询 当前置机所连接的客户端数
                                //case MessageTypes.QRYENDPOINTCONNECTED:
                                //    {
                                //        //debug(PROGRAME + "Qry connected endpoint number", QSEnumDebugLevel.INFO);
                                //        string frontid = msg.Content;
                                //        long num = handleMessage(msg.Type, msg.Content, msg.Content, msg.Content);
                                //        debug("ServerDNS Got" + msg.Content + " Connected Num:" + num.ToString());
                                //        try
                                //        {
                                //            rep.Send(Convert.ToInt32(num).ToString(), Encoding.UTF8);
                                //        }
                                //        catch (Exception ex)
                                //        {
                                //            debug("QRYENDPOINTCONNECTED error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                                //            rep.Send("10000", Encoding.UTF8);
                                //        }
                                //        break;
                                //    }
                                default:
                                    return;//其他类型的消息直接返回
                            }
                        }
                        catch (Exception ex)
                        {
                            debug("deal wektask error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                        }

                    };
                    var poller = new Poller(new List<ZmqSocket> { rep });

                    _serverDNSthreadready = true;
                    while (_namesgo)
                    {
                        _serverDNSthreadready = true;
                        try
                        {
                            poller.Poll(PollerTimeOut);
                            if (!_namesgo)
                            {
                                debug("Names thread stopped,try to close socket", QSEnumDebugLevel.INFO);
                                rep.Close();
                            }
                        }
                        catch (ZmqException e)
                        {
                            debug("names look up error" + e.ToString());
                        }

                    }
                    _serverDNSthreadready = false;
                }
            }
        }

        //服务器发送产生了一定的问题 是不是需要从worker下手发送返回消息？
        //服务端向客户端发送消息需要附带地址
        /// <summary>
        /// 向某个客户端发送消息
        /// </summary>
        /// <param name="body">消息内容(Message对应的内存块)</param>
        /// <param name="address">客户端地址/标识</param>
        /// <param name="front">对应前置地址,前置地址为空/null则客户端直接连接到本地Router</param>
        public void Send(byte[] body, string address,string front)
        {
            ZMessage zmsg = new ZMessage(body);
            //将消息地址与消息绑定，通过outputChanel发送出去,这样frontend就会根据消息地址自动的将消息发送给对应的客户端
            zmsg.Wrap(Encoding.UTF8.GetBytes(address),null);
            //如果front不为空或者null,则我们再继续添加路由地址
            if (!string.IsNullOrEmpty(front))
            {
                zmsg.Wrap(Encoding.UTF8.GetBytes(front),null);
            }
            zmsg.Send(_outputChanel);
        }

        /// <summary>
        /// 分发行情数据
        /// </summary>
        /// <param name="tick">行情数据</param>
        public void SendTick(Tick k)
        {
            string tickstr = k.symbol + "^" + TickImpl.Serialize(k);
            //debug("tickstr:" + tickstr, QSEnumDebugLevel.INFO);
            _tickpub.Send(tickstr, Encoding.UTF8);
        }
        /// <summary>
        /// 发送Tick 心跳数据
        /// 非线程安全
        /// </summary>
        public void SendTickHeartBeat()
        {
            string tickstr = "TICKHEARTBEAT";
            _tickpub.Send(tickstr, Encoding.UTF8);
        }


        //传输层前端
        ZmqSocket _outputChanel;//用于服务端主动向客户端发送消息
        ZmqSocket _tickpub;//用于转发Tick数据

        private void MessageRoute()
        {
            workers = new List<Thread>(_worknum);
            using (ZmqContext context = ZmqContext.Create())
            {   //当server端返回信息时,我们同样需要借助一定的设备完成
                using (ZmqSocket frontend = context.CreateSocket(SocketType.ROUTER), backend = context.CreateSocket(SocketType.DEALER), outchannel = context.CreateSocket(SocketType.DEALER), outClient = context.CreateSocket(SocketType.DEALER), publisher = context.CreateSocket(SocketType.PUB))//, adminpublisher = context.Socket(SocketType.PUB),adminfrontend = context.Socket(SocketType.ROUTER), adminbackend = context.Socket(SocketType.DEALER))
                {
                    #region tcp keep alive
                    //frontend.AddTcpAcceptFilter("192.168.1.1");//由防火墙来控制访问列表
                    //a.消息大小设置 管理端不设置消息大小，需要上传 配置文件
                    //if(_tlmode == QSEnumTLMode.TradingSrv)
                    //    frontend.MaxMessageSize = 300;//普通交易消息的大小在100以内。我们设置成200 过滤掉大于200的消息这里是一个bug由于消息大小设置太小,导致逻辑服务器频繁断开接入服务器连接
                    //frontend.ReceiveBufferSize = 200;
                    //frontend.SendBufferSize = 200;

                    //tcp keepalive的4个参数 connect端设置有效
                    //frontend.TcpKeepalive = ZeroMQ.TcpKeepaliveBehaviour.Enable;
                    //frontend.TcpKeepaliveCnt = 5;/* 判定断开前的KeepAlive探测次数 */
                    //frontend.TcpKeepaliveIntvl = 1000;/* 两次KeepAlive探测间的时间间隔  */
                    //frontend.TcpKeepaliveIdle = 120000;/*开始首次KeepAlive探测前的TCP空闭时间 */
                    #endregion

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
                    //starttcikthread();

                    //管理服务器只开一个线程用于处理消息 通过设置worknum实现
                    for (int workerid = 0; workerid <_worknum; workerid++)
                    {
                        workers.Add(new Thread(MessageTranslate));
                        workers[workerid].IsBackground = true;
                        workers[workerid].Name = "MessageDealWorker#" + workerid.ToString()+"@" + PROGRAME;;
                        object[] o = new object[] { context, workerid };
                        workers[workerid].Start(o);
                        ThreadTracker.Register(workers[workerid]);
                    }
                    //fronted过来的信息我们路由到backend上去
                    frontend.ReceiveReady += (s, e) =>
                    {
#if DEBUG
                        //v("frontend->backent");
#endif
                        var zmsg = new ZMessage(e.Socket);
                        zmsg.Send(backend);
                    };
                    //backend过来的信息我们路由到frontend上去
                    backend.ReceiveReady += (s, e) =>
                    {
#if DEBUG
                       // v("backend->frontend");
#endif
                        var zmsg = new ZMessage(e.Socket);
                        zmsg.Send(frontend);
                    };
                    //output接受到的消息,我们通过front发送出去,所有发送给客户端的消息通过outClient发送到output,然后再通过front路由出去
                    outchannel.ReceiveReady += (s, e) =>
                    {
#if DEBUG
                            //v("server side send the message outside");
                            //v("output->frontend");
#endif
                        var zmsg = new ZMessage(e.Socket);
#if DEBUG
                            //v("address is:" + zmsg.AddressToString());
#endif
                        zmsg.Send(frontend);
                    };
                    var poller = new Poller(new List<ZmqSocket> { frontend, backend, outchannel });
                    //让线程一直获取由socket发报过来的信息
                    _mainthreadready = true;
                    while (_srvgo)
                    {
                        try
                        {
                            poller.Poll(PollerTimeOut);
                            if (!_srvgo)
                            {
                                debug("messageroute thread stop,try to clear socket",QSEnumDebugLevel.INFO);
                                frontend.Close();
                                backend.Close();
                                publisher.Close();
                                outchannel.Close();
                                outClient.Close();
                            }
                        }
                        catch (ZmqException e)
                        {
                            debug(PROGRAME + ":MainThread[ZmqExcetion]" + e.ToString(), QSEnumDebugLevel.ERROR);
                        }
                        catch (System.Exception ex)
                        {
                            debug(PROGRAME + ":MainThread[Excetion]" + ex.ToString(), QSEnumDebugLevel.ERROR);
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
        private void MessageTranslate(object olist)
        {
            object[] list = olist as object[];
            int id = int.Parse(list[1].ToString());
            ZmqContext wctx = (ZmqContext)list[0];
            using (ZmqSocket worker = wctx.CreateSocket(SocketType.DEALER))
            {
                //将worker连接到backend用于接收由backend中继转发过来的信息
                worker.Connect("inproc://backend");
                worker.ReceiveReady += (s, e) =>
                    {
                        WorkerMessageProc(worker, id);
                    };
                var poller = new Poller(new List<ZmqSocket> { worker });

                while (_workergo)
                {
                    try
                    {
                        poller.Poll(PollerTimeOut);
                        if (!_workergo)
                        {
                            debug(string.Format("worker thread[{0}] stopped,close worker socket",id), QSEnumDebugLevel.INFO);
                            worker.Close();
                        }

                    }
                    catch (ZmqException e)
                    {
                        debug(string.Format("worker {0} look up error:",id) + e.ToString());
                    }
                }
            }
        }

        ZeromqThroughPut zmqTP;//流控管理器,用于跟踪所有客户端连接的消息流
        //Worker消息处理函数
        void WorkerMessageProc(ZmqSocket worker, int id)
        {
            try
            {
                //服务端从这里得到客户端过来的消息
                ZMessage zmsg = new ZMessage(worker);
                //1.进行消息地址解析 zmessage 中含有多个frame frame[0]是消息主体,其余frame是附加的地址信息
                //通过zmessage frame数量判断,获得对应的地址信息
                string front = string.Empty;
                string address = string.Empty;
                //debug("Frames Count:" + zmsg.FrameCount.ToString() + " body:" + UTF8Encoding.Default.GetString(zmsg.Body) + " add:" + UTF8Encoding.Default.GetString(zmsg.Address),QSEnumDebugLevel.INFO);
                //注意:进行地址有效性检查,如果有空地址 则直接返回。空地址会造成下道逻辑的错误
                //带有2层地址,客户端从接入服务器登入
                //if (TradingLib.Core.CoreGlobal.EnableAccess)//如果允许前置接入 则消息可以带有2层或者1层地址
                if(true)
                {
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
                }
                else//如果不允许前置接入,则只有1层地址的消息可以被处理
                {
                    if (zmsg.FrameCount == 2)
                    {
                        address = zmsg.AddressToString();
                        //debug("no access Got 2 frame " + "front:" + front + " address:" + address, QSEnumDebugLevel.MUST);
                        if (string.IsNullOrEmpty(address) || address.Length != 36) return;
                    }
                    else
                    {
                        return;
                    }
                }

                //2.流控 超过消息频率则直接返回不进行该消息的处理(拒绝该消息) 交易服务器才执行流控,管理服务器不执行
                //if (this._tlmode == QSEnumTLMode.TradingSrv && !zmqTP.NewZmessage(address,zmsg)) 
                //{
                //    return true;
                //}

                //3.消息处理如果解析出来的消息是有效的则丢入处理流程进行处理，如果无效则不处理
                Message msg = Message.gotmessage(zmsg.Body);
                //debug(string.Format("[AsyncServerMQ] Worker_{0} Got Message,FrameCount:{1} Front:{2} Address:{3} Type:{4} Content:{5}", id, zmsg.FrameCount,front, address, msg.Type, msg.Content),QSEnumDebugLevel.INFO);
                if(msg.isValid)
                    handleMessage(msg.Type, msg.Content,front,address);//处理消息按照消息类型进行消息路由,如果没有该消息则则会被系统过滤掉
                return;
            }
            catch (ZmqException e)
            {
                debug(PROGRAME + ":Worker[" + id.ToString() + "][ZmqExcetion]" + e.ToString(), QSEnumDebugLevel.ERROR);
                return;
            }
            //捕捉QSTradingServerError(客户端向服务端请求操作所引发的异常)
            catch (QSTradingServerError ex)
            {
                debug(PROGRAME + ":Worker[" + id.ToString() + "][QSExcetion]" + ex.Label + " " + ex.RawException.Message, QSEnumDebugLevel.ERROR);
                return;
            }
            catch (System.Exception ex)
            {
                debug(PROGRAME + ":Worker[" + id.ToString() + "][Excetion]" + ex.ToString(), QSEnumDebugLevel.ERROR);
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