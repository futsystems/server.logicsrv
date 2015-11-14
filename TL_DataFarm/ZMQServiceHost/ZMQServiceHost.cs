using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using Common.Logging;
using ZeroMQ;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.DataFarm.Common;


namespace ZMQServiceHost
{
    public class ZMQServiceHost:IServiceHost
    {

        ILog logger = LogManager.GetLogger(_name);

        const string _name = "ZMQServiceHost";
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        public string Name { get { return _name; } }


        const int TimoutSecend = 1;
        /// <summary>
        /// 系统默认Poller超时时间
        /// </summary>
        TimeSpan pollerTimeOut = new TimeSpan(0, 0, TimoutSecend);

        /// <summary>
        /// 消息处理工作线程数目
        /// </summary>
        public int NumWorkers { get { return _worknum; } set { _worknum = (value > 1 ? value : 1); } }
        private int _worknum = 1;


        int _port = 9590;

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


        public ZMQServiceHost()
        { 
            
        }
        public void Start()
        {
            if (_started)
                return;
            logger.Info("Start Transport Service @" + _name);
            //启动主服务线程
            _workergo = true;
            _srvgo = true;
            _srvThread = new Thread(new ThreadStart(MessageRoute));
            _srvThread.IsBackground = true;
            _srvThread.Name = "AsyncMessageRoute@" + _name;
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
                logger.Info("Start success");
                _started = true;
            }
            else
            {
                logger.Info("Start failed");
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
            logger.Info(string.Format("Stop MessageRouter Service[{0}]", _name));
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



        //传输层前端
        ZmqSocket _outputChanel;//用于服务端主动向客户端发送消息
        private void MessageRoute()
        {
            workers = new List<Thread>(_worknum);
            using (ZmqContext context = ZmqContext.Create())
            {   
                //当server端返回信息时,我们同样需要借助一定的设备完成
                using (ZmqSocket frontend = context.CreateSocket(SocketType.ROUTER),backend = context.CreateSocket(SocketType.DEALER),outchannel = context.CreateSocket(SocketType.DEALER), outClient = context.CreateSocket(SocketType.DEALER))
                {
                    //前端Router用于注册Client
                    frontend.Bind("tcp://*:" + _port.ToString());
                    //后端用于向worker线程发送消息,worker再去执行
                    backend.Bind("inproc://backend");
                    //用于系统对外发送消息
                    outchannel.Bind("inproc://output");
                    //对外发送消息的对端socket
                    _outputChanel = outClient;
                    outClient.Connect("inproc://output");
                    logger.Info("MD ServiceHost Listen at:" + _port.ToString());
                    for (int workerid = 0; workerid < _worknum; workerid++)
                    {
                        workers.Add(new Thread(MessageWorkerProc));
                        workers[workerid].IsBackground = true;
                        workers[workerid].Name = "MessageDealWorker#" + workerid.ToString() + "@" + _name; ;
                        object[] o = new object[] { context, workerid };
                        workers[workerid].Start(o);
                    }
                    //fronted过来的信息我们路由到backend上去
                    frontend.ReceiveReady += (s, e) =>
                    {
                        var zmsg = new ZMessage(e.Socket);
                        zmsg.Send(backend);
                    };
                    //backend过来的信息我们路由到frontend上去
                    backend.ReceiveReady += (s, e) =>
                    {
                        var zmsg = new ZMessage(e.Socket);
                        zmsg.Send(frontend);
                    };
                    //output接受到的消息,我们通过front发送出去,所有发送给客户端的消息通过outClient发送到output,然后再通过front路由出去
                    outchannel.ReceiveReady += (s, e) =>
                    {
                        var zmsg = new ZMessage(e.Socket);
                        zmsg.Send(frontend);
                    };


                    var poller = new Poller(new List<ZmqSocket> { frontend, backend, outchannel});
                    //让线程一直获取由socket发报过来的信息
                    _mainthreadready = true;
                    while (_srvgo)
                    {
                        try
                        {
                            poller.Poll(pollerTimeOut);//设定poll的time out可以防止该线程一直阻塞在poll，导致线程无法停止
                            if (!_srvgo)
                            {
                                logger.Info("messageroute thread stop,try to clear socket");
                                frontend.Close();
                                backend.Close();
                                outchannel.Close();
                                outClient.Close();
                            }
                            else//如果服务没有停止
                            {
                                //DateTime now = DateTime.Now;
                                //if ((now - _lasthb).TotalSeconds >= 5)
                                //{
                                //    this.SendTickHeartBeat();
                                //    _lasthb = now;
                                //}
                            }
                        }
                        catch (ZmqException e)
                        {
                            logger.Error("MainThread[ZmqExcetion]" + e.ToString());
                        }
                        catch (System.Exception ex)
                        {
                            logger.Error("MainThread[Excetion]" + ex.ToString());
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
                        poller.Poll(pollerTimeOut);
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

        public void Send(IPacket packet, bool isReq = false)
        {
            this.Send(packet.Data, packet.ClientID, isReq);  
        }

        public void Send(byte[] data, string address, bool isReq = false)
        {
            if (_outputChanel == null)
                throw new InvalidOperationException("out channel is null");
            lock (_outputChanel)
            {
                ZMessage zmsg = new ZMessage(data);
                if (isReq)
                {
                    zmsg.Wrap(Encoding.UTF8.GetBytes(address), Encoding.UTF8.GetBytes(""));
                }
                else
                {
                    zmsg.Wrap(Encoding.UTF8.GetBytes(address), null);
                }
                zmsg.Send(_outputChanel);
            }
        }

        /// <summary>
        /// 记录客户端注册的连接
        /// </summary>
        ConcurrentDictionary<string, IConnection> _sessionMap = new ConcurrentDictionary<string, IConnection>();

        /// <summary>
        /// MD_ZMQ服务端处理直接连接的客户端请求 每个请求均有一个对应的地址
        /// 
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="id"></param>
        void MessageProcess(ZmqSocket worker, int id)
        {
            try
            {
                ZMessage zrequest = new ZMessage(worker);
                string address = zrequest.AddressToString();
                Message message = Message.gotmessage(zrequest.Body);

                logger.Info(string.Format("ServiceHost got message type:{0} content:{1}", message.Type, message.Content));
                //响应客户端服务查询
                switch(message.Type)
                {
                    //响应客户端服务查询
                    case MessageTypes.SERVICEREQUEST:
                        {
                            QryServiceRequest request = RequestTemplate<QryServiceRequest>.SrvRecvRequest("", address, message.Content);
                            RspQryServiceResponse response = QryService(request);
                            this.Send(response,true);
                            logger.Info(string.Format("Got QryServiceRequest from:{0} request:{1} reponse:{2}", address, request, response));
                            return;
                        }


                    //响应客户端连接注册 该注册是底层连接部分的注册 需要ServiceHost维护该连接在客户端登入成功后还建立漏记连接Connection
                    case MessageTypes.REGISTERCLIENT:
                        {
                            RegisterClientRequest request = RequestTemplate<RegisterClientRequest>.SrvRecvRequest("", address, message.Content);
                            if (request != null)
                            {
                                IConnection conn = null;
                                //连接已经建立直接返回
                                if (_sessionMap.TryGetValue(address,out conn))
                                {
                                    logger.Warn(string.Format("Client:{0} already exist", address));
                                    return;
                                }
                                
                                //创建连接
                                conn = new ZMQConnection(this, address);
                                _sessionMap.TryAdd(address, conn);

                                //发送回报
                                RspRegisterClientResponse response = ResponseTemplate<RspRegisterClientResponse>.SrvSendRspResponse(request);
                                response.SessionID = address;
                                conn.Send(response);

                                logger.Info(string.Format("Client:{0} registed to server", address));
                                //向逻辑成抛出连接建立事件
                                OnSessionCreated(conn);
                                
                            }
                            return;
                        }
                    default:
                        {
                            IConnection conn = null;
                            if (!_sessionMap.TryGetValue(address, out conn))
                            {
                                logger.Warn(string.Format("Client:{0} is not registed to server, ignore request", address));
                                return;
                            }

                            IPacket packet = PacketHelper.SrvRecvRequest(message.Type, message.Content, "", address);
                            if (packet != null && RequestEvent != null)
                            {
                                RequestEvent(this,conn,packet);
                            }
                        }
                        return;
                }
            }
            catch (ZmqException e)
            {
                logger.Error("Worker[" + id.ToString() + "][ZmqExcetion]" + e.ToString());
                return;
            }
            //捕捉QSTradingServerError(客户端向服务端请求操作所引发的异常)
            catch (QSTradingServerError ex)
            {
                logger.Error("Worker[" + id.ToString() + "][QSExcetion]" + ex.Label + " " + ex.RawException.Message);
                return;
            }
            catch (System.Exception ex)
            {
                logger.Error("Worker[" + id.ToString() + "][Excetion]" + ex.ToString());
                return;
            }
    }


        /// <summary>
        /// 创建Connection对象
        /// DataServer不负责传输层面逻辑，只负责业务层面的逻辑,当DataServer认证通过后需要调用ServiceHost来创建Connection
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public IConnection CreateConnection(string sessionID)
        {

            return null;
        }

        void OnSessionCreated(IConnection conn)
        {
            if (SessionCreatedEvent != null)
            {
                SessionCreatedEvent(this, conn);
            }
        }

        void OnSessionClosed(IConnection conn)
        {
            if (SessionClosedEvent != null)
            {
                SessionClosedEvent(this, conn);
            }
        }
        private void HandleMessage(MessageTypes type, string body,string address)
        { 
            
        }

        /// <summary>
        /// 查询服务是否可用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private RspQryServiceResponse QryService(QryServiceRequest request)
        {
            RspQryServiceResponse response = null;
            if (ServiceEvent != null)
            {
                IPacket packet = ServiceEvent(this, request);
                
                if (packet!=null && packet.Type == MessageTypes.SERVICERESPONSE)
                {
                    response = packet as RspQryServiceResponse;
                    response.APIType = QSEnumAPIType.MD_ZMQ;
                    response.APIVersion = "1.0.0";
                }
            }
            if (response == null)
            {
                response = ResponseTemplate<RspQryServiceResponse>.SrvSendRspResponse(request);
                response.APIType = QSEnumAPIType.ERROR;
                response.APIVersion = "";
            }
            return response;
        }
        public event Action<IServiceHost,IConnection,IPacket> RequestEvent;

        public event Func<IServiceHost, IPacket, IPacket> ServiceEvent;

        /// <summary>
        /// 客户端连接建立
        /// </summary>
        public event Action<IServiceHost, IConnection> SessionCreatedEvent;

        /// <summary>
        /// 客户端连接关闭
        /// </summary>
        public event Action<IServiceHost, IConnection> SessionClosedEvent;
    }
}
