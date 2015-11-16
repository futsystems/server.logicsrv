using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using ZeroMQ;
using Common.Logging;
using TradingLib.DataFarm.API;

namespace TradingLib.DataFarm
{
    /// <summary>
    /// 行情源
    /// 用于连接到TickSrv接受实时行情
    /// </summary>
    public class FastTickDataFeed : ITickFeed
    {
        ILog logger;

        TimeSpan timeout = new TimeSpan(0, 0, 1);

        string _master = "127.0.0.1";
        string _slave = "127.0.0.1";
        int _port = 6000;
        int _reqport = 6001;

        public bool IsLive { get { return _tickreceiveruning; } }

        bool _usemaster = true;

        const string NAME = "FastTickFeed";
        public string Name { get { return NAME; } }
        string CurrentServer
        {
            get
            {
                if (string.IsNullOrEmpty(_slave)) return _master;
                return _usemaster ? _master : _slave;
            }
        }
        public FastTickDataFeed()
        {
            logger = LogManager.GetLogger(this.Name);
            ConfigFile _cfg = ConfigFile.GetConfigFile("FastTickFeed.cfg");
            _master = _cfg["TickSrvMaster"].AsString();
            _slave = _cfg["TickSrvSlave"].AsString();
            _port = _cfg["TickPort"].AsInt();
            _reqport = _cfg["ReqPort"].AsInt();
        }

        public FastTickDataFeed(string masterAddress, string slaveAddress, int dataport, int reqport)
        {
            logger = LogManager.GetLogger(this.Name);
            _master = masterAddress;
            _slave = slaveAddress;
            _port = dataport;
            _reqport = reqport;
        }

        public event Action<ITickFeed,Tick> TickEvent;
        void OnTick(Tick k)
        {
            if (TickEvent != null)
                TickEvent(this, k);
        }

        public event Action<ITickFeed> ConnectEvent;
        void OnConnected()
        {
            if (ConnectEvent != null)
                ConnectEvent(this);
        }

        public event Action<ITickFeed> DisconnectEvent;
        void OnDisconnected()
        {
            if (DisconnectEvent != null)
                DisconnectEvent(this);
        }


        public void Start()
        {
            logger.Info(string.Format("MasterServer:{0} SlaveServer:{1} Port:{2} ReqPort:{3}", _master, _slave, _port, _reqport));

            StartTickHandler();

            StartHB();
        }

        public void Stop()
        {

            StopHB();

            StopTickHandler();
            //重新将行情服务器标识设置为主，这样停止后会再次重连主服务器
            _usemaster = true;
        }


        #region 行情服务监控线程 用于当行情服务停止时 切换到备用服务器
        void StartHB()
        {
            if (_hb) return;
            _hb = true;
            _hbthread = new Thread(HeartBeatWatch);
            _hbthread.IsBackground = true;
            _hbthread.Name = "FasktTickDF HBWatch";
            _hbthread.Start();
            ThreadTracker.Register(_hbthread);
            _lastheartbeat = DateTime.Now;
        }

        void StopHB()
        {
            if (!_hb) return;
            _hb = false;
            int _wait = 0;
            while (_hbthread.IsAlive && (_wait++ < 5))
            {
                logger.Info("#:" + _wait.ToString() + "  FastTickHB is stoping...." + "MessageThread Status:" + _hbthread.IsAlive.ToString());
                Thread.Sleep(500);
            }
            if (!_hbthread.IsAlive)
            {
                ThreadTracker.Unregister(_hbthread);
                _hbthread = null;
                logger.Info("FastTickHB Stopped successfull...");
            }
            else
            {
                logger.Error("Some Error Happend In Stoping FastTickHB");
            }
        }


        DateTime _lastheartbeat = DateTime.Now;
        bool _hb = false;
        Thread _hbthread = null;

        private void HeartBeatWatch()
        {
            while (_hb)
            {
                if (DateTime.Now.Subtract(_lastheartbeat).TotalSeconds > 5)
                {
                    logger.Error("TickHeartBeat lost, try to ReConnect to tick server");
                    if (_tickgo)
                    {
                        _usemaster = !_usemaster;
                        //停止行情服务线程
                        StopTickHandler();
                        //启动行情服务
                        StartTickHandler();
                        //更新行情心跳时间
                        _lastheartbeat = DateTime.Now;
                        logger.Info("Connect to TickServer success");
                    }
                }
                Util.sleep(100);
            }
        }

        #endregion


        #region 行情数据处理
        void StartTickHandler()
        {
            if (_tickgo) return;
            _tickgo = true;
            _tickthread = new Thread(TickHandler);
            _tickthread.IsBackground = true;
            _tickthread.Name = "FasktTickDF TickHandler";
            _tickthread.Start();
            ThreadTracker.Register(_tickthread);

            int i = 0;
            while (!_tickreceiveruning & i < 5)
            {
                Thread.Sleep(500);
                i++;
                logger.Info("wait datafeed start....");
            }
        }


        void StopTickHandler()
        {
            if (!_tickgo) return;
            _tickgo = false;
            int _wait = 0;
            while (_tickthread.IsAlive && (_wait++ < 5))
            {
                logger.Info("#:" + _wait.ToString() + "  FastTick is stoping...." + "MessageThread Status:" + _tickthread.IsAlive.ToString());
                Thread.Sleep(500);
            }
            if (!_tickthread.IsAlive)
            {
                ThreadTracker.Unregister(_tickthread);
                _tickthread = null;
                logger.Info("FastTick Stopped successfull...");
            }
            else
            {
                logger.Error("Some Error Happend In Stoping FastTick");
            }
        }





        ZmqSocket _subscriber;//sub socket receive real time market data
        ZmqSocket _symbolreq;//req socket send request for subscribe
        bool _tickgo;
        Thread _tickthread;
        bool _tickreceiveruning = false;

        private void TickHandler()
        {
            using (var context = ZmqContext.Create())
            {
                using (ZmqSocket subscriber = context.CreateSocket(SocketType.SUB), symbolreq = context.CreateSocket(SocketType.REQ))
                {
                    string reqadd = "tcp://" + CurrentServer + ":" + _reqport;
                    symbolreq.Connect(reqadd);
                    string subadd = "tcp://" + CurrentServer + ":" + _port;
                    subscriber.Connect(subadd);
                    logger.Info(string.Format("Connect to FastTick Server:{0} ReqPort:{1} DataPort{2}", CurrentServer, _reqport, _port));
                   
                    //订阅行情心跳数据
                    subscriber.Subscribe(Encoding.UTF8.GetBytes("TICKHEARTBEAT"));
                    string prefix ="HGZ5^";
                    subscriber.Subscribe(Encoding.UTF8.GetBytes(prefix));
                    //prefix = "HSIX5^";
                    //subscriber.Subscribe(Encoding.UTF8.GetBytes(prefix));
                    //subscriber.SubscribeAll();
                    _symbolreq = symbolreq;
                    _subscriber = subscriber;

                    subscriber.ReceiveReady += (s, e) =>
                    {
                        try
                        {
                            string tickstr = subscriber.Receive(Encoding.UTF8);
                            string[] p = tickstr.Split('^');
                            if (p.Length > 1)
                            {
                                //logger.Info("tick str:" + tickstr);
                                string symbol = p[0];
                                string tickcontent = p[1];
                                Tick k = TickImpl.Deserialize(tickcontent);

                                if (k.isValid)
                                    OnTick(k);
                            }
                            else
                            {
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Tick process error:" + ex.ToString());
                        }
                        //记录数据到达时间
                        _lastheartbeat = DateTime.Now;
                    };
                    var poller = new Poller(new List<ZmqSocket> { subscriber });

                    _tickreceiveruning = true;
                    OnConnected();
                    while (_tickgo)
                    {
                        try
                        {
                            poller.Poll(timeout);
                            if (!_tickgo)
                            {
                                logger.Info("Tick Thread Stopped,try to close socket");
                                subscriber.Close();
                                symbolreq.Close();
                            }
                        }
                        catch (ZmqException ex)
                        {
                            logger.Error("Tick Sock错误:" + ex.ToString());

                        }
                        catch (System.Exception ex)
                        {
                            logger.Error("Tick数据处理错误" + ex.ToString());
                        }

                    }
                    _tickreceiveruning = false;
                    OnDisconnected();
                }
            }
        }

        #endregion

    }
}
