using System;
using System.Collections.Generic;
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
            logger.Info("Start Message Transport Service @" + _name);
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

        void MessageProcess(ZmqSocket worker, int id)
        {
            try
            {

                //服务端从这里得到客户端过来的消息
                ZMessage zmsg = new ZMessage(worker);
                //1.进行消息地址解析 zmessage 中含有多个frame frame[0]是消息主体,其余frame是附加的地址信息
                //通过zmessage frame数量判断,获得对应的地址信息
                string front = string.Empty;
                string address = string.Empty;
                Message msg = Message.gotmessage(zmsg.Body);

                logger.Info("Frames Count:" + zmsg.FrameCount.ToString() + " body:" + UTF8Encoding.Default.GetString(zmsg.Body) + " add:" + UTF8Encoding.Default.GetString(zmsg.Address));
                //注意:进行地址有效性检查,如果有空地址 则直接返回。空地址会造成下道逻辑的错误
                //带有2层地址,客户端从接入服务器登入
                //if (TradingLib.Core.CoreGlobal.EnableAccess)//如果允许前置接入 则消息可以带有2层或者1层地址
                //如果消息类型是前置发送到交易逻辑服务的心跳
                if (msg.Type == MessageTypes.LOGICLIVEREQUEST)
                {
                    if (zmsg.FrameCount == 3)
                    {
                        front = zmsg.AddressToString();//获得第一层地址
                        zmsg.Unwrap();//分离第一层地址
                        address = zmsg.AddressToString();

                        LogicLiveRequest request = (LogicLiveRequest)PacketHelper.SrvRecvRequest(msg.Type, msg.Content, front, address);

                        //Util.Debug(string.Format("LogicHeartBeat from:{0} address:{1}",front,address),QSEnumDebugLevel.DEBUG);
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
                if (true)
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

                //debug(string.Format("[AsyncServerMQ] Worker_{0} Got Message,FrameCount:{1} Front:{2} Address:{3} Type:{4} Content:{5}", id, zmsg.FrameCount,front, address, msg.Type, msg.Content),QSEnumDebugLevel.INFO);
                if (msg.isValid)
                    HandleMessage(msg.Type, msg.Content, front, address);//处理消息按照消息类型进行消息路由,如果没有该消息则则会被系统过滤掉
                return;
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


        private void HandleMessage(MessageTypes type, string body, string front, string address)
        { 
            
        }
        public event Action<IServiceHost, IPacket> RequestEvent;
    }
}
