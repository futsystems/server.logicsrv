using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;

using TradingLib.Common;
using System.Threading;
using System.Net;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 底层传输客户端,用于发起向服务器的底层传输连接,实现消息的收发功能
    /// client端会给自己绑定一个ID,用于通讯的唯一标识,但是经过实际运行发现,该ID可能会与已经登入的客户端重复.
    /// 因此导致登入信息错误，收不到注册回报估计就是该问题,大概想一下应该是这样
    /// </summary>
    public class AsyncClient
    {

        const string PROGRAME = "AsyncClient";
        public event DebugDelegate SendDebugEvent;
        public event HandleTLMessageClient SendTLMessage;
        //public event TickDelegate GotTick;

        /// <summary>
        /// 消息处理函数事件,当客户端接收到消息 解析后调用该函数实现相应函数调用
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        private void handleMessage(MessageTypes type, string msg)
        {
            if (SendTLMessage != null)
                SendTLMessage(type, msg);

        }

        bool _noverb = false;
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }
        private void v(string msg)
        {
            if (!_noverb)
                msgdebug(msg);
        }

        bool _debugEnable = true;
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }
        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.DEBUG;
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            if ((int)level <= (int)_debuglevel && _debugEnable)
                msgdebug("[" + level.ToString() + "] " + msg);
        }

        private void msgdebug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverip">服务器地址</param>
        /// <param name="port">服务器端口</param>
        /// <param name="verbos">是否日志输出</param>
        public AsyncClient(string serverip, int port, bool verbos)
            : this(serverip, port, serverip, port + 2, verbos) { }

        /// <summary>
        /// 将tickserver/exserver分开，分别提供行情ip 端口 交易ip 交易端口
        /// </summary>
        /// <param name="serverip"></param>
        /// <param name="port"></param>
        /// <param name="tickserver"></param>
        /// <param name="tickport"></param>
        /// <param name="verbos"></param>
        public AsyncClient(string serverip, int port, string tickserver, int tickport, bool verbos)
        {
            _serverip = serverip;
            _serverport = port;
            VerboseDebugging = verbos;

            _tickip = tickserver;
            _tickport = tickport;

        }

        /// <summary>
        /// 断开与服务器的连接
        /// </summary>
        public void Disconnect()
        {
            Stop();
        }
        /// <summary>
        /// 客户端是否已经连接
        /// </summary>
        public bool isConnected { get { return _started; } }

        public bool isTickConnected { get { return _tickreceiveruning; } }
        /// <summary>
        /// 启动客户端连接
        /// </summary>
        public void Start()
        {
            if (_started)
                return;
            v("[AsyncClient] starting ....");
            _cligo = true;
            _cliThread = new Thread(new ThreadStart(MessageTranslate));
            _cliThread.IsBackground = true;
            _cliThread.Start();

            int _wait = 0;
            while (!isConnected && (_wait++ < 5))
            {
                //等待1秒,当后台正式启动完毕后方可有进入下面的程序端运行
                Thread.Sleep(500);
                debug(PROGRAME + "#:" + _wait.ToString() + "Starting....");
            }
            //注意这里是通过启动线程来运行底层传输的，Start返回后 后台的传输线程并没有完全启动完毕
            //这里我们需要有一个循环用于等待启动完毕 _started = true;放在启动函数里面
            //bool ok = false;

            if (!isConnected)
                throw new QSAsyncClientError();
            else
                debug(PROGRAME + ":Started successfull");


            _msgthread = new Thread(procmessageout);
            _msgthread.IsBackground = true;
            msggo = true;
            _msgthread.Start();
        }

        /// <summary>
        /// 启动Tick数据接收,如果TLClient所连接的服务器支持Tick数据,则我们可以启动单独的Tick对话流程,用于接收数据
        /// </summary>
        public void StartTickReciver()
        {
            if (_tickreceiveruning)
                return;
            _tickgo = true;
            v("Start Client Tick Reciving Thread....");
            _tickthread = new Thread(new ThreadStart(TickHandler));
            _tickthread.IsBackground = true;
            _tickthread.Start();

            int _wait = 0;
            while (!_tickreceiveruning && (_wait++ < 5))
            {
                //等待1秒,当后台正式启动完毕后方可有进入下面的程序端运行
                Thread.Sleep(500);
                debug(PROGRAME + "#:" + _wait.ToString() + "  AsynClient[Tick Reciver] is connecting....");
            }
            if (!_tickreceiveruning)
                throw new QSAsyncClientError();
            else
                debug(PROGRAME + ":[TickReciver] started successfull");
        }

        public void Stop()
        {
            try
            {
                debug("___________________AnsyncClient Stop Thread and Socket....", QSEnumDebugLevel.INFO);
                bool a = StopMessageReciver();
                bool b = StopTickReciver();
                debug("___________________Stop Task Report: " + "[MessageThread Stop]:" + a.ToString() + "  [TickThread Stop]:" + b.ToString(), QSEnumDebugLevel.INFO);

                msggo = false;
                _msgthread.Abort();
                _msgthread = null;
            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":stop error :" + ex.ToString());
            }
        }
        bool StopMessageReciver()
        {
            if (!_started)
                return true;
            debug("Stop Client Message Reciving Thread....");
            _cligo = false;//设定消息处理线程运行interrupt标志 当设定该标志后 try 异常就会导致循环退出
            //关闭协议客户端_client 关闭该客户端会引发 poller.中的异常 在while(_cligo){poller.Poll();}的异常处理中可以正常退出线程
            int _wait = 0;
            while (_cliThread.IsAlive && (_wait++ < 5))
            {
                debug("#:" + _wait.ToString() + "  AsynClient is stoping...." + "MessageThread Status:" + _cliThread.IsAlive.ToString(), QSEnumDebugLevel.INFO);
                _client.Close();//关闭线程需要消耗一定时间,如果马上继续运行部做等待，则下面的线程活动判断语句任然会有问题
                Thread.Sleep(500);
            }
            if (!_cliThread.IsAlive)
            {
                _cliThread = null;
                debug("MessageThread Stopped successfull...", QSEnumDebugLevel.INFO);
                return true;
            }
            debug("Some Error Happend In Stoping MessageThread", QSEnumDebugLevel.ERROR);
            return false;
        }
        public bool StopTickReciver()
        {
            if (!_tickreceiveruning)
                return true;
            debug("Stop Client Tick Reciving Thread....");
            _tickgo = false;
            int _wait = 0;
            while (_tickthread.IsAlive && (_wait++ < 5))
            {
                //等待1秒,当后台正式启动完毕后方可有进入下面的程序端运行
                debug("#:" + _wait.ToString() + "  AsynClient[Tick Reciver] is stoping...." + "TickThread Status:" + _tickthread.IsAlive.ToString());
                subscriber.Close();
                Thread.Sleep(500);
            }
            if (!_tickthread.IsAlive)
            {
                _tickthread = null;
                debug("TickThread Stopped successfull...", QSEnumDebugLevel.INFO);
                return true;
            }
            debug("Some Error Happend In Stoping TickThread", QSEnumDebugLevel.ERROR);
            return false;

        }

        /// <summary>
        /// 与服务器连接时候 获得的唯一的ID标示 用于区分客户端
        /// </summary>
        public string ID { get { return _identity; } }
        public string Name { get { return ID; } }

        Thread _cliThread = null;
        ZmqContext _mctx = null;
        ZmqSocket _client = null;
        bool _cligo = false;
        bool _started = false;
        string _identity = "";
        string _serverip = "127.0.0.1";
        public string ServerAddress { get { return _serverip; } set { _serverip = value; } }
        int _serverport = 5570;

        string _tickip = "127.0.0.1";
        int _tickport = 5572;

        public int Port
        {
            get { return _serverport; }
            set
            {

                if (value < 1000)
                    _serverport = 5570;
                else
                    _serverport = value;
            }
        }

        //消息翻译线程,当socket有新的数据进来时候,我们将数据转换成TL交易协议的内部信息,并触发SendTLMessage事件,从而TLClient可以用于调用对应的处理逻辑对信息进行处理
        private void MessageTranslate()
        {
            using (_mctx = ZmqContext.Create())
            {
                using (_client = _mctx.CreateSocket(SocketType.DEALER))
                {
                    _identity = System.Guid.NewGuid().ToString();
                    _client.Identity = Encoding.UTF8.GetBytes(_identity);
                    string cstr = "tcp://" + _serverip.ToString() + ":" + Port.ToString();
                    debug(PROGRAME + ":Connect to Message Server:" + cstr, QSEnumDebugLevel.MUST);
                    _client.Connect(cstr);


                    //当客户端有消息近来时,我们读取消息并调用handleMessage出来消息 
                    //
                    _client.ReceiveReady += (s, e) =>
                    {
                        lock (_client)
                        {
                            var zmsg = new ZMessage(e.Socket);

                            Message msg = Message.gotmessage(zmsg.Body);
#if DEBUG
                            v("AsyncClient:" + msg.Type.ToString() + "|" + msg.Content.ToString());
#endif
                            handleMessage(msg.Type, msg.Content);
                        }
                    };
                    var poller = new Poller(new List<ZmqSocket> { _client });
                    //当我们运行到这里的时候才可以认为服务启动完毕
                    _started = true;
                    while (_cligo)
                    {
                        try
                        {
                            poller.Poll();
                        }
                        catch (ZmqException ex)
                        {
                            debug("Message Socket错误" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            _client.Dispose();
                            _mctx.Dispose();

                        }
                        catch (System.Exception ex)
                        {
                            debug("客户端协议信息错误" + ex.ToString(), QSEnumDebugLevel.ERROR);
                        }
                    }
                    _started = false;
                    //当我们运行到这里的时候 才可以认为线程正常退出了
                }
            }
        }







        ZmqSocket subscriber;
        bool _tickgo;
        Thread _tickthread;
        bool _tickreceiveruning = false;
        private void TickHandler()
        {
            using (var context = ZmqContext.Create())
            {
                using (subscriber = context.CreateSocket(SocketType.SUB))
                {
                    //  Generate printable identity for the client
                    string cstr = "tcp://" + _tickip.ToString() + ":" + _tickport.ToString();
                    debug(PROGRAME + ":Connect to TickServer :" + cstr, QSEnumDebugLevel.MUST);
                    subscriber.Connect(cstr);
                    SubscribeTickHeartBeat();
                    subscriber.ReceiveReady += (s, e) =>
                    {
						
                        string tickstr = subscriber.Receive(Encoding.UTF8);
						//string tickstr = subscriber.Receive(Encoding.UTF8);
                        //debug("tickstr:" + tickstr.ToString());
                        string[] p = tickstr.Split('^');
                        if (p.Length == 2)
                        {
                            string symbol = p[0];
                            string tickcontent = p[1];

                            handleMessage(MessageTypes.TICKNOTIFY, tickcontent);
                        }
                        else if (p[0] == "TICKHEARTBEAT")
                        {
                            //debug("got tickheartbeat", QSEnumDebugLevel.MUST);
                            handleMessage(MessageTypes.TICKHEARTBEAT, "TICKHEARTBEAT");
                        }
                    };
                    var poller = new Poller(new List<ZmqSocket> { subscriber });

                    _tickreceiveruning = true;
                    while (_tickgo)
                    {
                        try
                        {
                            poller.Poll();
                        }
                        catch (ZmqException ex)
                        {
                            debug("Tick Sock错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            subscriber.Dispose();
                            context.Dispose();
                        }
                        catch (System.Exception ex)
                        {
                            debug("Tick数据处理错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                        }
                    }
                    _tickreceiveruning = false;
                }
            }
        }

        /// <summary>
        /// 用于检查某个ip地址是否提供有效服务,没有有效服务端则引发QSNoServerException异常,如果有数据或交易服务 则服务器会返回一个Provider名称 用于标识服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string HelloServer(string ip, int port, DebugDelegate debug)
        {

            debug("[AsyncClient]Start say hello to server...");
            string rep = null;
            //初始化context 与 requester
            ZmqContext context = ZmqContext.Create();
            ZmqSocket requester = context.CreateSocket(SocketType.REQ);
            string cstr = "tcp://" + ip + ":" + (port + 1).ToString();
            requester.Connect(cstr);

            //初始化消息
            //Message msg = new Message(MessageTypes., " ");
            //byte[] data = Message.sendmessage(msg);
            //IPacket package = new BrokerNameRequest();
            //ZMessage zmsg = new ZMessage(package.Data);
            //发送消息并得到服务端回报
            //zmsg.Send(requester);
            //rep = requester.Receive(Encoding.UTF8, new TimeSpan(0, 0, IPUtil.SOCKETREPLAYTIMEOUT));
            //debug("helloServer response:" + rep);
            BrokerNameRequest package = new BrokerNameRequest();
            package.SetRequestID(10001);
            debug(package.ToString());
            ZMessage zmsg = new ZMessage(package.Data);
            //发送消息并得到服务端回报
            zmsg.Send(requester);
            byte[] response = new byte[0];
            int size = 0;
            response = requester.Receive(response, new TimeSpan(0, 0, IPUtil.SOCKETREPLAYTIMEOUT), out size);
            TradingLib.Common.Message message = TradingLib.Common.Message.gotmessage(response);
            debug("got raw size:" + size + " type:" + message.Type + " content:" + message.Content);

            BrokerNameResponse br = ResponseTemplate<BrokerNameResponse>.CliRecvResponse(message.Content);

            debug("response:" + br.ToString());

            //关闭网络连接资源
            requester.Disconnect(cstr);
            requester.Close();
            context.Dispose();


            //if (rep == null)
            //{
            //    throw (new QSNoServerException());
           // }
            return ((int)br.Provider).ToString();
        }

        /*
        public static string RequestAccept(string ip, int port, string localip, DebugDelegate debug)
        {
            debug("[AsyncClient]Request server accept client...");
            string rep = null;
            //初始化context 与 requester
            ZmqContext context = ZmqContext.Create();
            ZmqSocket requester = context.CreateSocket(SocketType.REQ);
            string cstr = "tcp://" + ip + ":" + (port + 1).ToString();
            requester.Connect(cstr);

            //初始化消息
            Message msg = new Message(MessageTypes.REQUESTIPACCEPT, localip);
            byte[] data = Message.sendmessage(msg);
            ZMessage zmsg = new ZMessage(data);
            //发送消息并得到服务端回报
            zmsg.Send(requester);
            rep = requester.Receive(Encoding.UTF8, new TimeSpan(0, 0, IPUtil.SOCKETREPLAYTIMEOUT));
            //debug("helloServer response:" + rep);
            if (rep == null)
            {
                throw (new QSNoServerException());
            }
            return rep;
        }**/

        //发送byte信息
        public void Send(byte[] msg)
        {
            /*
            //发送消息时候 锁定_client，其他线程需要访问_client需要等待解锁
            lock (_client)
            {
                ZMessage zmsg = new ZMessage(msg);
                zmsg.Send(_client);
            }**/
            msgcache.Write(msg);
        }


        Thread _msgthread = null;
        bool msggo = false;
        RingBuffer<byte[]> msgcache = new RingBuffer<byte[]>(1000);
        void procmessageout()
        {
            while (msggo)
            {
                try
                {
                    while (msgcache.hasItems)
                    {
                        ZMessage zmsg = new ZMessage(msgcache.Read());
                        zmsg.Send(_client);
                    }
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    debug("client send message out error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }


            }


        }

        /// <summary>
        /// 用于注册publisher的heartbeat
        /// </summary>
        void SubscribeTickHeartBeat()
        {
            if (subscriber == null) return;
            string prefix = "TICKHEARTBEAT";
            subscriber.Subscribe(Encoding.UTF8.GetBytes(prefix));
        }
        /// <summary>
        /// 订阅某个合约的数据
        /// </summary>
        /// <param name="symbol"></param>
        public void Subscribe(string symbol)
        {
            if (subscriber == null) return;
            string prefix = symbol + "^";
            subscriber.Subscribe(Encoding.UTF8.GetBytes(prefix));
            //SubscribeTickHeartBeat();
        }

        public void SubscribeAll()
        {
            if (subscriber == null) return;
            subscriber.SubscribeAll();
            SubscribeTickHeartBeat();
        }

        /// <summary>
        /// 注销symbol数据
        /// </summary>
        /// <param name="symbol"></param>
        public void Unsubscribe(string symbol)
        {
            if (subscriber == null) return;
            string prefix = symbol + "^";
            subscriber.Unsubscribe(Encoding.UTF8.GetBytes(prefix));
            //SubscribeTickHeartBeat();
        }

        public void Unsubscribe()
        {
            
            if (subscriber == null) return;
            subscriber.UnsubscribeAll();
            debug("ansycsocket 注销所有市场订阅...", QSEnumDebugLevel.ERROR);
            SubscribeTickHeartBeat();
        }
    }
}