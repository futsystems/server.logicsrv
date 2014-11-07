using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;
using ZeroMQ;
using TradingLib.BrokerXAPI;

namespace DataFeed.FastTick
{
       

    public class FastTick :TLDataFeedBase,IDataFeed
    {
        TimeSpan timeout = new TimeSpan(0, 0, 5);
        

		ConfigHelper cfg;
        string server;
		int port;
		int reqport;
        public FastTick()
        {
			cfg = new ConfigHelper(CfgConstDFFastTick.XMLFN);
			server = cfg.GetConfig(CfgConstDFFastTick.Server);
			port = int.Parse(cfg.GetConfig(CfgConstDFFastTick.Port));
			reqport = int.Parse(cfg.GetConfig(CfgConstDFFastTick.ReqPort));
			
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
            if (!_tickgo) return;
            _tickgo = false;
            int _wait = 0;
            //_tickthread.Abort();
            while (_tickthread.IsAlive && (_wait++ < 5))
            {
                debug("#:" + _wait.ToString() + "  FastTick is stoping...." + "MessageThread Status:" + _tickthread.IsAlive.ToString(), QSEnumDebugLevel.INFO);
                _subscriber.Close();//关闭线程需要消耗一定时间,如果马上继续运行部做等待，则下面的线程活动判断语句任然会有问题
                _symbolreq.Close();
                Thread.Sleep(500);
            }
            if (!_tickthread.IsAlive)
            {
                _tickthread = null;
                debug("FastTick Stopped successfull...", QSEnumDebugLevel.INFO);
                NotifyConnected();

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
            _tickthread.Name = "FasktTickDF TickHandler";
            _tickthread.Start();
            ThreadTracker.Register(_tickthread);

            int num = 5;
            int i=0;
            while (!_tickreceiveruning & i < 5)
            {
                Thread.Sleep(500);
                i++;
                debug("wait datafeed start....");
            }
        }

        ZmqSocket _subscriber;//sub socket which receive data
        ZmqSocket _symbolreq;//push socket which tell tickserver to regist data
        bool _tickgo;
        Thread _tickthread;
        bool _tickreceiveruning = false;
        private void TickHandler()
        {
            using (var context = ZmqContext.Create())
            {
                using ( ZmqSocket subscriber = context.CreateSocket(SocketType.SUB) ,symbolreq= context.CreateSocket(SocketType.REQ))
                {
                    string reqadd = "tcp://" + server + ":" + reqport;
                    debug("Connect to FastTickServer:" + reqadd, QSEnumDebugLevel.INFO);
                    symbolreq.Connect(reqadd);
                    _symbolreq = symbolreq;

                    debug("Subscribe to FastTick Publisher server:" + server + " Port:" + port,QSEnumDebugLevel.INFO);
                    subscriber.Connect("tcp://" + server + ":" + port);
                    _subscriber = subscriber;
                    _subscriber.SubscribeAll();
                    //Send(MessageTypes.MGRSTARTDATAFEED, "SIM");
                    subscriber.ReceiveReady += (s, e) =>
                    {
                        try
                        {
                            //debug("....tick",QSEnumDebugLevel.INFO);
                            string tickstr = subscriber.Receive(Encoding.UTF8);
                            string[] p = tickstr.Split('^');
                            if (p.Length > 1)
                            {
                                string symbol = p[0];
                                string tickcontent = p[1];
                                Tick k = TickImpl.Deserialize(tickcontent);
                                if (k.isValid)
                                    NotifyTick(k);
                            }
                        }
                        catch (Exception ex)
                        {
                            debug("Tick process error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                        }
                    };
                    var poller = new Poller(new List<ZmqSocket> { subscriber });

                    _tickreceiveruning = true;
                    NotifyConnected();
                    while (_tickgo)
                    {
                        try
                        {
                            poller.Poll(timeout);
                            if (!_tickgo)
                            {
                                debug("Tick Thread Stopped,try to close socket", QSEnumDebugLevel.INFO);
                                _subscriber.Close();
                                _symbolreq.Close();
                            }
                        }
                        catch (ZmqException ex)
                        {
                            debug("Tick Sock错误:" + ex.ToString(),QSEnumDebugLevel.ERROR);
                            
                        }
                        catch (System.Exception ex)
                        {
                            debug("Tick数据处理错误"+ex.ToString(),QSEnumDebugLevel.ERROR);
                        }
                    }
                    _tickreceiveruning = false;
                    NotifyDisconnected();
                }
            }
        }

        /// <summary>
        /// 通过本地push向对应的FastTickServer发送消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        void Send(TradingLib.API.MessageTypes type, string msg)
        {
            if (_symbolreq != null)
            {
                lock (_symbolreq)
                {
                    try
                    {
                        string rep = null;
                        byte[] message = TradingLib.Common.Message.sendmessage(type, msg);
                        _symbolreq.Send(message);//非阻塞
                        rep = _symbolreq.Receive(Encoding.UTF8, timeout);

                    }
                    catch (Exception ex)
                    {
                        debug("发送消息异常:" + ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 注册市场数据
        /// </summary>
        /// <param name="symbols"></param>
        public void RegisterSymbols(string[] symbols,QSEnumDataFeedTypes type = QSEnumDataFeedTypes.DEFAULT)
        {
            try
            {
                debug("regist symbols" + string.Join(",", symbols) + "to fast tick");
                //将合约字头逐个向publisher进行订阅
                foreach (string s in symbols)
                {
                    string prefix = s + "^";
                    _subscriber.Subscribe(Encoding.UTF8.GetBytes(prefix));//subscribe对应的symbol字头用于获得tick数据
                }

                //通过FastTickServer的管理端口 请求FastTickServer向行情源订阅行情数据,Publisher的订阅是内部的一个分发订阅 不会产生向行情源订阅实际数据
                string sym = string.Join(",", symbols);
                string requeststr = (type == QSEnumDataFeedTypes.DEFAULT ? sym : (type.ToString() + ":" + sym));
                debug(Title+":注册市场数据 " + requeststr,QSEnumDebugLevel.INFO);
                Send(TradingLib.API.MessageTypes.MGRREGISTERSYMBOLS, requeststr);
            }
            catch (Exception ex)
            {
                debug(":请求市场数据异常" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

    }
}
