using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;
using NetMQ;
using TradingLib.BrokerXAPI;
using Common.Logging;


namespace DataFeed.FastTick
{

    public class FastTick :TLDataFeedBase,IDataFeed
    {
        TimeSpan timeout = new TimeSpan(0, 0, 1);
        
        string master="127.0.0.1";
        string slave = "127.0.0.1";
		int port=6000;
		int reqport=6001;

        ConfigDB _cfgdb;
        

        string _prefixStr = string.Empty;
        List<string> _prefixList = new List<string>();


        public FastTick()
            :base("FastTick")
        {
            //加载配置信息
            _cfgdb = new ConfigDB("FastTickDataFeed");
            if (!_cfgdb.HaveConfig("TickServerVersion"))
            {
                _cfgdb.UpdateConfig("TickServerVersion", QSEnumCfgType.Int, 1, "行情服务器版本");
               
            }

            if (!_cfgdb.HaveConfig("TickPrefix"))
            {
                _cfgdb.UpdateConfig("TickPrefix", QSEnumCfgType.String, "X, Q, F, S,", "实时行情类别");
            }
            _prefixStr = _cfgdb["TickPrefix"].AsString();
            foreach (var prefix in _prefixStr.Split(' '))
            {
                _prefixList.Add(prefix);
            }

            if (!_cfgdb.HaveConfig("ReqSubscriber"))
            {
                _cfgdb.UpdateConfig("ReqSubscriber", QSEnumCfgType.Bool, false, "建立Re Port发起注册请求");
            }
            
        }


        public bool IsLive
        {
            get
            {
                return _tickreceiveruning;
            }
        }


        public void Stop()
        {

            StopHB();

            StopTickHandler();
            //重新将行情服务器标识设置为主，这样停止后会再次重连主服务器
            _usemaster = true;
        }

        bool _usemaster = true;

        string CurrentServer
        {
            get
            {
                if (string.IsNullOrEmpty(slave)) return master;
                return _usemaster ? master : slave;
            }
        }
        public void Start()
        {
            master = _cfg.srvinfo_ipaddress;
            slave = _cfg.srvinfo_field2;
            port = _cfg.srvinfo_port;
            int.TryParse(_cfg.srvinfo_field1, out reqport);

            logger.Info(string.Format("MasterServer:{0} SlaveServer:{1} Port:{2} ReqPort:{3}", master, slave, port, reqport));
            
            StartTickHandler();
            
            StartHB();
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
                    logger.Warn("TickHeartBeat lost, try to ReConnect to tick server");
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


        #region 行情服务主线程
        void StopTickHandler()
        {
            if (!_tickgo) return;
            _tickgo = false;
            if (tickPoller != null && tickPoller.IsRunning)
            {
                tickPoller.Stop();
            }
            _tickthread.Join();
            logger.Info("FastTick Stopped successfull...");
        }

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
                logger.Info("Datafeed Start....");
            }
        }

        NetMQ.Sockets.SubscriberSocket _subscriber;//sub socket which receive data
        bool _tickgo;
        Thread _tickthread;
        bool _tickreceiveruning = false;
        NetMQPoller tickPoller = null;
        private void TickHandler()
        {
            using (NetMQ.Sockets.SubscriberSocket subscriber = new NetMQ.Sockets.SubscriberSocket()) 
            {
                string subadd = "tcp://" + CurrentServer + ":" + port;
                subscriber.Options.Linger = System.TimeSpan.FromSeconds(1);
                subscriber.Connect(subadd);

                logger.Info(string.Format("Connect to FastTick Server:{0} DataPort{1}", CurrentServer, port));
  
                //订阅行情心跳数据
                subscriber.Subscribe(Encoding.UTF8.GetBytes("H,"));
                _subscriber = subscriber;

                tickPoller = new NetMQPoller { subscriber };
                subscriber.ReceiveReady += (s, a) =>
                    {
                        try
                        {
                            var zmsg = a.Socket.ReceiveMultipartMessage();
                            var tickstr = zmsg.First.ConvertToString(Encoding.UTF8);
                            //logger.Info("ticksr:" + tickstr);
                            Tick k = TickImpl.Deserialize2(tickstr);
                            if (k != null && k.UpdateType != "H")
                                NotifyTick(k);
                            //记录数据到达时间
                            _lastheartbeat = DateTime.Now;
                        }
                        catch (NetMQException ex)
                        {
                            logger.Error("Tick Sock错误:" + ex.ToString());

                        }
                        catch (System.Exception ex)
                        {
                            logger.Error("Tick数据处理错误" + ex.ToString());
                        }
                    };
                _tickreceiveruning = true;
                //行情源连接事件 DataRouter会订阅该事件 同时进行合约注册操作 该过程可能会消耗比较多的时间，因此造成这里阻塞 导致心跳接受异常 需要将订阅操作放入线程池中运行
                NotifyConnected();

                tickPoller.Run();
            }
            
            _tickreceiveruning = false;
            NotifyDisconnected();

        }
        #endregion


        /// <summary>
        /// 注册市场数据
        /// 行情订阅统一由历史行情服务器进行
        /// 交易服务器只需要通过设定订阅前缀来过滤本地接收到的行情数据
        /// 行情发布系统默认全订阅所有行情
        /// </summary>
        /// <param name="symbols"></param>
        public void RegisterSymbols(List<Symbol> symbols)
        {
            foreach (var sym in symbols)
            {
                foreach (var prefix in _prefixList)
                {
                    string p = prefix + sym.Symbol;
                    if (_subscriber != null)
                    {
                        _subscriber.Subscribe(Encoding.UTF8.GetBytes(p));
                    }
                }
            }
        }
    }
}
