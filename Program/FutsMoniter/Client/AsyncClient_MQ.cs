﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMQ;

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

        public void Stop()
        {
            try
            {
                debug("___________________AnsyncClient Stop Thread and Socket....", QSEnumDebugLevel.INFO);
                bool a = StopRecvThread();
                bool b = StopTickReciver();
                bool c = StopSendThread();
                debug("___________________Stop Task Report: " + "[MessageThread Stop]:" + a.ToString() + "  [TickThread Stop]:" + b.ToString()+"  [SendThread Stop]:" + b.ToString(), QSEnumDebugLevel.INFO);

            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":stop error :" + ex.ToString());
            }
        }

        /// <summary>
        /// 与服务器连接时候 获得的唯一的ID标示 用于区分客户端
        /// </summary>
        public string ID { get { return _identity; } }
        public string Name { get { return ID; } }



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

        /// <summary>
        /// 启动客户端连接
        /// </summary>
        public void Start()
        {
            //启动数据接收线程
            StartRecvThread();
            int _wait = 0;
            while (!isConnected && (_wait++ < 5))
            {
                //等待1秒,当后台正式启动完毕后方可有进入下面的程序端运行
                Thread.Sleep(500);
                debug(PROGRAME + "#:" + _wait.ToString() + "Starting....");
            }

            //启动数据发送线程
            StartSendThread();
        }


        #region 管理通道
        Thread _cliThread = null;
        NetMQSocket _client = null;
        Poller _msgpoller = null;
        bool _started = false;

        void StartRecvThread()
        {
            if (_started)
                return;
            v("[AsyncClient] starting ....");
            _cliThread = new Thread(new ThreadStart(MessageTranslate));
            _cliThread.IsBackground = true;
            _cliThread.Start();
            
        }

        bool StopRecvThread()
        {
            if (!_started)
                return true;
            debug("Stop Client Message Reciving Thread....");
            int _wait = 0;
            while (_cliThread.IsAlive && (_wait++ < 5))
            {
                debug("#:" + _wait.ToString() + "  AsynClient is stoping...." + "MessageThread Status:" + _cliThread.IsAlive.ToString(), QSEnumDebugLevel.INFO);
                if (_msgpoller.IsStarted)//如果poller处于启动状态 则停止poller
                {
                    //debug("stop msgpoller");
                    _msgpoller.Stop();
                }
                Thread.Sleep(1000);
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

        TimeSpan timeout = new TimeSpan(0, 0, 2);
        //消息翻译线程,当socket有新的数据进来时候,我们将数据转换成TL交易协议的内部信息,并触发SendTLMessage事件,从而TLClient可以用于调用对应的处理逻辑对信息进行处理
        private void MessageTranslate()
        {
            using (NetMQContext _mctx = NetMQContext.Create())
            {
                using (_client = _mctx.CreateDealerSocket())
                {
                    _identity = System.Guid.NewGuid().ToString();
                    _client.Options.Identity = Encoding.UTF8.GetBytes(_identity);
                    string cstr = "tcp://" + _serverip.ToString() + ":" + Port.ToString();
                    debug(PROGRAME + ":Connect to Message Server:" + cstr, QSEnumDebugLevel.MUST);
                    _client.Connect(cstr);

                    //当客户端有消息近来时,我们读取消息并调用handleMessage出来消息 
                    _client.ReceiveReady += (s, e) =>
                    {
                        lock (_client)
                        {
                            NetMQMessage zmsg = e.Socket.ReceiveMessage(timeout);
                            Message msg = Message.gotmessage(zmsg.Last.Buffer);
                            handleMessage(msg.Type, msg.Content);
                        }
                    };

                    using (var poller = new Poller())
                    {
                        _msgpoller = poller;
                        poller.AddSocket(_client);
                        //当我们运行到这里的时候才可以认为服务启动完毕
                        _started = true;
                        poller.Start();
                        _client.Disconnect(cstr);
                        _client.Close();
                        _client = null;
                    }
                    _started = false;
                }
            }
        }

        #endregion


        #region 行情连接
        NetMQSocket subscriber;
        Thread _tickthread;
        bool _tickreceiveruning = false;
        Poller _tickpoller = null;

        bool _suballtick = false;
        /// <summary>
        /// 启动Tick数据接收,如果TLClient所连接的服务器支持Tick数据,则我们可以启动单独的Tick对话流程,用于接收数据
        /// </summary>
        public void StartTickReciver(bool suballtick = false)
        {
            _suballtick = suballtick;
            if (_tickreceiveruning)
                return;
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

        public bool StopTickReciver()
        {
            if (!_tickreceiveruning)
                return true;
            debug("Stop Client Tick Reciving Thread....");
            int _wait = 0;
            while (_tickthread.IsAlive && (_wait++ < 5))
            {
                //等待1秒,当后台正式启动完毕后方可有进入下面的程序端运行
                debug("#:" + _wait.ToString() + "  AsynClient[Tick Reciver] is stoping...." + "TickThread Status:" + _tickthread.IsAlive.ToString());
                if (_tickpoller.IsStarted)
                {
                    //debug("tick poller stop");
                    _tickpoller.Stop();
                }
                Thread.Sleep(1000);
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

        private void TickHandler()
        {
            using (var context = NetMQContext.Create())
            {
                using (subscriber = context.CreateSubscriberSocket())
                {
                    string cstr = "tcp://" + _tickip.ToString() + ":" + _tickport.ToString();
                    debug(PROGRAME + ":Connect to TickServer :" + cstr, QSEnumDebugLevel.MUST);
                    subscriber.Connect(cstr);
                    SubscribeTickHeartBeat();

                    if (_suballtick)
                    {
                        subscriber.Subscribe("");
                    }

                    subscriber.ReceiveReady += (s, e) =>
                    {
                        string tickstr = subscriber.ReceiveString(Encoding.UTF8);
                        string[] p = tickstr.Split('^');
                        if (p.Length == 2)
                        {
                            string symbol = p[0];
                            string tickcontent = p[1];

                            handleMessage(MessageTypes.TICKNOTIFY, tickcontent);
                        }
                        else if (p[0] == "TICKHEARTBEAT")
                        {
                            handleMessage(MessageTypes.TICKHEARTBEAT, "TICKHEARTBEAT");
                        }
                    };

                    using (var poller = new Poller())
                    {
                        _tickpoller = poller;
                        poller.AddSocket(subscriber);
                        _tickreceiveruning = true;
                        poller.Start();
                        subscriber.Close();
                        subscriber = null;
                    }
                    _tickreceiveruning = false;
                }
            }
        }

        #endregion


        #region 消息发送线程

        /// <summary>
        /// 启动消息发送线程
        /// </summary>
        void StartSendThread()
        {
            if (msggo) return;
            //启动消息处理线程
            _msgthread = new Thread(procmessageout);
            _msgthread.IsBackground = true;
            msggo = true;
            _msgthread.Start();
        }

        /// <summary>
        /// 停止消息发送线程
        /// </summary>
        bool StopSendThread()
        {
            if (!msggo) return true;
            debug("Stop Client Send Thread....");
            int _wait = 0;
            while (_msgthread.IsAlive && (_wait++ < 5))
            {
                //等待1秒,当后台正式启动完毕后方可有进入下面的程序端运行
                debug("#:" + _wait.ToString() + "  AsynClient[Message Sender] is stoping...." + "SenderThread Status:" + _msgthread.IsAlive.ToString());
                if (_msgthread.IsAlive)
                {
                    msggo = false;
                }
                Thread.Sleep(1000);
            }
            if (!_msgthread.IsAlive)
            {
                _msgthread = null;
                debug("SenderThread Stopped successfull...", QSEnumDebugLevel.INFO);
                return true;
            }
            debug("Some Error Happend In Stoping SenderThread", QSEnumDebugLevel.ERROR);
            return false;
        }
        //发送byte信息
        public void Send(byte[] msg)
        {
            //放入消息缓存统一对外发送
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
                        byte[] data = msgcache.Read();
                        if (_client != null)
                        {
                            _client.Send(data);
                        }
                    }
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    debug("client send message out error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
            //debug("SendThread stop to here");
        }
        #endregion


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
            using (NetMQContext context = NetMQContext.Create())
            {
                using (NetMQSocket requester = context.CreateRequestSocket())
                {
                    string cstr = "tcp://" + ip + ":" + (port + 1).ToString();
                    requester.Options.ReceiveTimeout = new TimeSpan(0,0,2);
                    requester.Connect(cstr);
                    BrokerNameResponse br=null;
                    requester.ReceiveReady += (s, e) =>
                    {
                        NetMQMessage msg = requester.ReceiveMessage(new TimeSpan(0, 0, 2));

                        TradingLib.Common.Message message = TradingLib.Common.Message.gotmessage(msg.Last.Buffer);
                        br= ResponseTemplate<BrokerNameResponse>.CliRecvResponse(message.Content);
                        debug("response:" + br.ToString());
                        
                    };

                    using (var poller = new Poller())
                    {
                        poller.AddSocket(requester);
                        BrokerNameRequest package = new BrokerNameRequest();
                        package.SetRequestID(10001);
                        requester.Send(package.Data);
                        //debug("send to here");
                        poller.PollOnce();
                        //debug("polled once");
                    }
                    requester.Disconnect(cstr);
                    requester.Close();
                    //debug("socket closed....");
                    if (br == null)
                    {
                        throw new QSNoServerException();
                    }
                    return ((int)br.Provider).ToString();
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
            //subscriber.SubscribeAll();
            SubscribeTickHeartBeat();
        }

        ///// <summary>
        ///// 注销symbol数据
        ///// </summary>
        ///// <param name="symbol"></param>
        //public void Unsubscribe(string symbol)
        //{
        //    if (subscriber == null) return;
        //    string prefix = symbol + "^";
        //    subscriber.Unsubscribe(Encoding.UTF8.GetBytes(prefix));
        //    //SubscribeTickHeartBeat();
        //}

        //public void Unsubscribe()
        //{

        //    if (subscriber == null) return;
        //    subscriber.UnsubscribeAll();
        //    debug("ansycsocket 注销所有市场订阅...", QSEnumDebugLevel.ERROR);
        //    SubscribeTickHeartBeat();
        //}
    }
}