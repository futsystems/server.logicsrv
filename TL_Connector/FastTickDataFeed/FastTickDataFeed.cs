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
        TimeSpan timeout = new TimeSpan(0, 0, 1);
        
        string master="127.0.0.1";
        string slave = "127.0.0.1";
		int port=6000;
		int reqport=6001;

        public FastTick()
        {
			
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

        //int _timeout = 3;
        //bool _switch = true;

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

            debug(string.Format("MasterServer:{0} SlaveServer:{1} Port:{2} ReqPort:{3}", master, slave, port, reqport), QSEnumDebugLevel.INFO);
            
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
                debug("#:" + _wait.ToString() + "  FastTickHB is stoping...." + "MessageThread Status:" + _hbthread.IsAlive.ToString(), QSEnumDebugLevel.INFO);
                Thread.Sleep(500);
            }
            if (!_hbthread.IsAlive)
            {
                ThreadTracker.Unregister(_hbthread);
                _hbthread = null;
                debug("FastTickHB Stopped successfull...", QSEnumDebugLevel.INFO);
            }
            else
            {
                debug("Some Error Happend In Stoping FastTickHB", QSEnumDebugLevel.ERROR);
            }
        }


        DateTime _lastheartbeat = DateTime.Now;
        bool _hb = false;
        Thread _hbthread = null;

        //bool _reconntick = true;
        //bool _stoptick = false;
        private void HeartBeatWatch()
        {
            while (_hb)
            {
                if (DateTime.Now.Subtract(_lastheartbeat).TotalSeconds > 5)
                {
                    debug("TickHeartBeat lost, try to ReConnect to tick server", QSEnumDebugLevel.ERROR);
                    if (_tickgo)
                    {
                        _usemaster = !_usemaster;
                        //停止行情服务线程
                        StopTickHandler();
                        //启动行情服务
                        StartTickHandler();
                        //更新行情心跳时间
                        _lastheartbeat = DateTime.Now;
                        debug("Connect to TickServer success", QSEnumDebugLevel.INFO);
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
            int _wait = 0;
            while (_tickthread.IsAlive && (_wait++ < 5))
            {
                debug("#:" + _wait.ToString() + "  FastTick is stoping...." + "MessageThread Status:" + _tickthread.IsAlive.ToString(), QSEnumDebugLevel.INFO);
                Thread.Sleep(500);
            }
            if (!_tickthread.IsAlive)
            {
                ThreadTracker.Unregister(_tickthread);
                _tickthread = null;
                debug("FastTick Stopped successfull...", QSEnumDebugLevel.INFO);
            }
            else
            {
                debug("Some Error Happend In Stoping FastTick", QSEnumDebugLevel.ERROR);
            }
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
                    string reqadd = "tcp://" + CurrentServer + ":" + reqport;
                    //debug("Connect to FastTick ReqServer:" + reqadd, QSEnumDebugLevel.INFO);
                    symbolreq.Connect(reqadd);

                    string subadd = "tcp://" + CurrentServer + ":" + port;
                    //debug("Subscribe to FastTick PubServer:" + subadd, QSEnumDebugLevel.INFO);
                    debug(string.Format("Connect to FastTick Server:{0} ReqPort:{1} DataPort{2}",CurrentServer,reqport,port),QSEnumDebugLevel.INFO);
                    subscriber.Connect(subadd);
                    //订阅行情心跳数据
                    subscriber.Subscribe(Encoding.UTF8.GetBytes("TICKHEARTBEAT"));

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
                                string symbol = p[0];
                                string tickcontent = p[1];
                                Tick k = TickImpl.Deserialize(tickcontent);
                                //Util.Debug("tick date:" + k.Date + " time time:" + k.Time);
                                if (k.isValid)
                                    NotifyTick(k);
                            }
                            else
                            {
                                //debug("tick str:" + tickstr, QSEnumDebugLevel.INFO);
                            }
                        }
                        catch (Exception ex)
                        {
                            debug("Tick process error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                        }
                        //记录数据到达时间
                        _lastheartbeat = DateTime.Now;
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
                                subscriber.Close();
                                symbolreq.Close();
                                debug("-----------------------", QSEnumDebugLevel.ERROR);
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
                //debug("----------step1", QSEnumDebugLevel.ERROR);
                //context.Terminate();
                //context.Dispose();
                //debug("content terminate", QSEnumDebugLevel.ERROR);
            }
            debug("----------step2", QSEnumDebugLevel.ERROR);
        }
        #endregion



        #region 通过请求端口执行API请求
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
        public void RegisterSymbols(SymbolBasket symbols)
        {
            try
            {
                //将合约按照行情源类型进行分组
                Dictionary<QSEnumDataFeedTypes, List<Symbol>> map = SplitSymbolViaDataFeedType(symbols);

                foreach (KeyValuePair<QSEnumDataFeedTypes, List<Symbol>> kv in map)
                {
                    //string symlist = string.Join(",", kv.Value.Select(sym=>sym.Symbol));
                    //将合约字头逐个向publisher进行订阅
                    foreach (Symbol s in kv.Value)
                    {
                        string prefix = s.Symbol + "^";
                        _subscriber.Subscribe(Encoding.UTF8.GetBytes(prefix));//subscribe对应的symbol字头用于获得tick数据
                    }

                    //通过FastTickServer的管理端口 请求FastTickServer向行情源订阅行情数据,Publisher的订阅是内部的一个分发订阅 不会产生向行情源订阅实际数据
                    //注册合约协议格式 DATAFEED:SYMBOL|EXCHANGE
                    foreach (var sym in kv.Value)
                    {
                        string tmpreq = (kv.Key.ToString() + ":" + sym.Symbol + "|" + sym.SecurityFamily.Exchange.EXCode);
                        debug(Token + " RegisterSymbol " + tmpreq, QSEnumDebugLevel.INFO);
                        Send(TradingLib.API.MessageTypes.MGRREGISTERSYMBOLS, tmpreq);
                    }

                }

                
            }
            catch (Exception ex)
            {
                debug(":请求市场数据异常" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        /// <summary>
        /// 将某个合约对象按FastTick进行拆分
        /// 所有的合约均通过FastTick行情通道进行订阅
        /// </summary>
        /// <param name="basket"></param>
        /// <returns></returns>
        Dictionary<QSEnumDataFeedTypes,List<Symbol>> SplitSymbolViaDataFeedType(SymbolBasket basket)
        {
            Dictionary<QSEnumDataFeedTypes, List<Symbol>> map = new Dictionary<QSEnumDataFeedTypes, List<Symbol>>();
            foreach (Symbol sym in basket)
            {
                QSEnumDataFeedTypes type = sym.SecurityFamily.DataFeed;////Symbol2DataFeedType(sym);
                if (map.Keys.Contains(type))
                {
                    map[type].Add(sym);
                }
                else
                {
                    map[type] = new List<Symbol>();
                    map[type].Add(sym);
                }
            }
            return map;

        }

        /// <summary>
        /// 按照一定的逻辑获得合约在FastTick中的行情通道类型
        /// 通过合约判断该行情从哪个FastTick 通道中进行注册
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        QSEnumDataFeedTypes Symbol2DataFeedType(Symbol symbol)
        {
            if (symbol.SecurityFamily.Exchange.Country == Country.CN)
            {
                if (symbol.SecurityFamily.Exchange.EXCode == "HKEX")//香港交易所通过香港直达期货公司订阅
                {
                    return QSEnumDataFeedTypes.SHZD;
                }
                if (symbol.SecurityType == SecurityType.FUT)
                {
                    return QSEnumDataFeedTypes.CTP;
                }

                if (symbol.SecurityType == SecurityType.OPT)
                {
                    return QSEnumDataFeedTypes.CTPOPT;
                }
            }
            //国外行情通过IQFeed获取
            if (symbol.SecurityFamily.Exchange.Country != Country.CN)
            {
                return QSEnumDataFeedTypes.IQFEED;
            }
            return QSEnumDataFeedTypes.CTP;
        }
        #endregion



    }
}
