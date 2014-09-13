using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;
using ZeroMQ;

namespace DataFeed.SimTick
{
    public class SimTick : IDataFeed
    {
        TimeSpan timeout = new TimeSpan(0, 0, 5);
        public string Title { get { return "SimTick数据通道"; } }
        bool _verb = true;
        public bool VerboseDebugging { get { return _verb; } set { _verb = value; } }
        /// <summary>
        /// 当数据服务器登入成功后调用
        /// </summary>
        public event IConnecterParamDel Connected;
        /// <summary>
        /// 当数据服务器断开后触发事件
        /// </summary>
        public event IConnecterParamDel Disconnected;
        /// <summary>
        /// 当有日志信息输出时调用
        /// </summary>
        public event DebugDelegate SendDebugEvent;
        /// <summary>
        /// 当数据服务得到一个新的tick时调用
        /// </summary>
        public event TickDelegate GotTickEvent;


        ConfigHelper cfg;
        string server;
        string port;
        
        //string reqport;
        public SimTick()
        {
            cfg = new ConfigHelper(CfgConstDFSimTick.XMLFN);
            server = cfg.GetConfig(CfgConstDFSimTick.Server);
            port = cfg.GetConfig(CfgConstDFSimTick.Port);

        }

        public bool IsLive
        {
            get
            {
                return _tickreceiveruning;
            }
        }

        bool _debugEnable = true;
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }
        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.INFO;
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

        void msgdebug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        public void Stop()
        {
            if (!_tickgo) return;
            _tickgo = false;
            int _wait = 0;
            while (_tickthread.IsAlive && (_wait++ < 5))
            {
                debug("#:" + _wait.ToString() + "  FastTick is stoping...." + "MessageThread Status:" + _tickthread.IsAlive.ToString(), QSEnumDebugLevel.INFO);
                Thread.Sleep(1000);
            }
            if (!_tickthread.IsAlive)
            {
                _tickthread = null;
                debug("FastTick Stopped successfull...", QSEnumDebugLevel.INFO);
                if (Disconnected != null)
                    Disconnected(this);

            }
            else
            {
                debug("Some Error Happend In Stoping FastTick", QSEnumDebugLevel.ERROR);
            }

        }

        public void Start()
        {
            if (_tickgo) return;
            _tickgo = true;
            _tickthread = new Thread(TickHandler);
            _tickthread.IsBackground = true;
            _tickthread.Name = "SimTickDF TickHandler";
            _tickthread.Start();
            ThreadTracker.Register(_tickthread);
        }

        ZmqSocket _subscriber;//sub socket which receive data
        //ZmqSocket _symbolreq;//push socket which tell tickserver to regist data
        bool _tickgo;
        Thread _tickthread;
        bool _tickreceiveruning = false;
        DateTime start = DateTime.Now;
        private void TickHandler()
        {
            using (var context = ZmqContext.Create())
            {
                using (ZmqSocket subscriber = context.CreateSocket(SocketType.SUB), symbolreq = context.CreateSocket(SocketType.REQ))
                {
                    debug("Subscribe to SimTick Publisher server:" + server + " Port:" + port, QSEnumDebugLevel.INFO);
                    subscriber.Connect("tcp://" + server + ":" + port);
                    _subscriber = subscriber;
                    _subscriber.SubscribeAll();
                    subscriber.ReceiveReady += (s, e) =>
                    {
                        try
                        {
                            string tickstr = subscriber.Receive(Encoding.UTF8);
                            string[] p = tickstr.Split('^');
                            if (p.Length > 1)
                            {
                                //debug("tick got:" + tickstr, QSEnumDebugLevel.INFO);
                                //if (DateTime.Now.Subtract(start).TotalSeconds > 10)
                                //{
                                    //debug("datafeed got tick:" + DateTime.Now.ToString(), QSEnumDebugLevel.MUST);
                                //    start = DateTime.Now;
                                //}
                                string symbol = p[0];
                                string tickcontent = p[1];
                                Tick k = TickImpl.Deserialize(tickcontent);
                                if (GotTickEvent != null && k.isValid)
                                {
                                    GotTickEvent(k);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            debug("got tick error:" + ex.ToString(), QSEnumDebugLevel.ERROR);

                        }
                    };
                    var poller = new Poller(new List<ZmqSocket> { subscriber });

                    _tickreceiveruning = true;
                    if (Connected != null)
                        Connected(this);
                    while (_tickgo)
                    {
                        try
                        {
                            poller.Poll(timeout);
                            if (!_tickgo)
                            {
                                debug("tigo false,stop subscriber......", QSEnumDebugLevel.INFO);
                                subscriber.Close();

                            }
                        }
                        catch (ZmqException ex)
                        {
                            debug("Tick Sock错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                        }
                        catch (System.Exception ex)
                        {
                            debug("Tick数据处理错误" + ex.ToString(), QSEnumDebugLevel.ERROR);
                        }
                    }
                    _tickreceiveruning = false;
                    if (Disconnected != null)
                        Disconnected(this);
                }
            }
        }

        /// <summary>
        /// 注册市场数据
        /// </summary>
        /// <param name="symbols"></param>
        public void RegisterSymbols(string[] symbols, QSEnumDataFeedTypes type = QSEnumDataFeedTypes.DEFAULT)
        {
            try
            {
                debug("regist symbols,no need register, register all symbols");
            }
            catch (Exception ex)
            {
                debug(":请求市场数据异常" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

    }
}
