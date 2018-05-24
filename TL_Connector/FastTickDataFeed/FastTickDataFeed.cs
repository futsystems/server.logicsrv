using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;
using ZeroMQ;
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
        int _tickversion = 1;

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
            _tickversion = _cfgdb["TickServerVersion"].AsInt();

            if (!_cfgdb.HaveConfig("TickPrefix"))
            {
                _cfgdb.UpdateConfig("TickPrefix", QSEnumCfgType.String, "X, Q, F, S,", "实时行情类别");
            }
            _prefixStr = _cfgdb["TickPrefix"].AsString();
            foreach (var prefix in _prefixStr.Split(' '))
            {
                _prefixList.Add(prefix);
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
            //int _wait = 0;
            //while (_tickthread.IsAlive && (_wait++ < 5))
            //{
            //    logger.Info("#:" + _wait.ToString() + "  FastTick is stoping...." + "MessageThread Status:" + _tickthread.IsAlive.ToString());
            //    Thread.Sleep(500);
            //}
            //if (!_tickthread.IsAlive)
            //{
            //    ThreadTracker.Unregister(_tickthread);
            //    _tickthread = null;
            //    logger.Info("FastTick Stopped successfull...");
            //}
            //else
            //{
            //    logger.Error("Some Error Happend In Stoping FastTick");
            //}
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
                logger.Info("wait datafeed start....");
            }
        }

        ZSocket _subscriber;//sub socket which receive data
        //ZSocket _symbolreq;//push socket which tell tickserver to regist data
        bool _tickgo;
        Thread _tickthread;
        bool _tickreceiveruning = false;
        private void TickHandler()
        {
            using (var context = new ZContext())
            {
                using (ZSocket subscriber = new ZSocket(context, ZSocketType.SUB))//, symbolreq = new ZSocket(context, ZSocketType.REQ))
                {
                    subscriber.SendHighWatermark = 1000000;
                    subscriber.ReceiveHighWatermark = 1000000;
                    //symbolreq.SendHighWatermark = 1000000;
                    //symbolreq.ReceiveHighWatermark = 1000000;
                    string reqadd = "tcp://" + CurrentServer + ":" + reqport;
                    //symbolreq.Connect(reqadd);

                    string subadd = "tcp://" + CurrentServer + ":" + port;
                    logger.Info(string.Format("Connect to FastTick Server:{0} ReqPort:{1} DataPort{2}", CurrentServer, reqport, port));
                    subscriber.Connect(subadd);
                    //订阅行情心跳数据
                    subscriber.Subscribe(Encoding.UTF8.GetBytes("H,"));

                    //_symbolreq = symbolreq;
                    _subscriber = subscriber;

                    _tickreceiveruning = true;
                    //行情源连接事件 DataRouter会订阅该事件 同时进行合约注册操作 该过程可能会消耗比较多的时间，因此造成这里阻塞 导致心跳接受异常 需要将订阅操作放入线程池中运行
                    NotifyConnected();

                    ZMessage tickdata;
                    ZError error;
                    string tickstr = string.Empty;
                    while (_tickgo)
                    {
                        try
                        {
                            if (null == (tickdata = subscriber.ReceiveMessage(out error)))
                            {
                                if (error == ZError.ETERM)
                                    return;	// Interrupted
                                throw new ZException(error);
                            }
                            else
                            {

                                tickstr =tickdata.First().ReadString(Encoding.UTF8);
                                //清空zmessage 否则内存溢出
                                tickdata.Clear();

                                //logger.Info("ticksr:" + tickstr);
                                Tick k = TickImpl.Deserialize2(tickstr);
                                if (k != null && k.UpdateType != "H")
                                    if (k.IsValid())
                                        NotifyTick(k);


                                //记录数据到达时间
                                _lastheartbeat = DateTime.Now;
                            }

                            if (!_tickgo)
                            {
                                logger.Info("Tick Thread Stopped,try to close socket");
                                subscriber.Close();
                                //symbolreq.Close();
                            }
                        }
                        catch (ZException ex)
                        {
                            logger.Error("Tick Sock错误:" + ex.ToString());

                        }
                        catch (System.Exception ex)
                        {
                            logger.Error("Tick数据处理错误" + ex.ToString());
                        }

                    }
                    _tickreceiveruning = false;
                    NotifyDisconnected();
                }
            }
        }
        #endregion



        #region 通过请求端口执行API请求

        void Send(IPacket packet)
        {
            //if (_symbolreq != null)
            //{
            //    lock (_symbolreq)
            //    {
            //        try
            //        {
            //            byte[] message = packet.Data;
            //            _symbolreq.Send(new ZFrame(message));
            //            ZMessage response;
            //            ZError error;
            //            var poller = ZPollItem.CreateReceiver();
            //            if (_symbolreq.PollIn(poller, out response, out error, timeout))
            //            {
            //                logger.Debug(string.Format("Got Rep Response:", response.First().ReadString(Encoding.UTF8)));
            //                response.Clear();
            //            }
            //            else
            //            {
            //                if (error == ZError.ETERM)
            //                {
            //                    return;
            //                }
            //                throw new ZException(error);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            logger.Error("发送消息异常:" + ex.ToString());
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 注册市场数据
        /// </summary>
        /// <param name="symbols"></param>
        public void RegisterSymbols(List<Symbol> symbols)
        {
            logger.Warn("Symbol register move to DataFarm,LogicServer will not subscribe sysmbol");

            try
            {
                //按交易所分组
                Dictionary<string, List<Symbol>> exSymMap = new Dictionary<string, List<Symbol>>();
                foreach (Symbol sym in symbols)
                {

                    List<Symbol> list = null;
                    if (!exSymMap.TryGetValue(sym.Exchange, out list))
                    {
                        list = new List<Symbol>();
                        exSymMap.Add(sym.Exchange, list);
                    }
                    list.Add(sym);
                }
                //注册交易下所有合约
                foreach (var pair in exSymMap)
                {
                   // MDRegisterSymbolsRequest request = RequestTemplate<MDRegisterSymbolsRequest>.CliSendRequest(0);
                    ExchangeImpl exch = BasicTracker.ExchagneTracker[pair.Key];
                    if (exch == null)
                    {
                        logger.Warn("Exchange:{0} do not exist".Put(pair.Key));
                    }
                    //request.DataFeed = exch.DataFeed;
                    //request.Exchange = pair.Key;
                    foreach (var sym in pair.Value)
                    {
                        //request.SymbolList.Add(sym.GetFullSymbol());
                        foreach (var prefix in _prefixList)
                        {
                            string p = prefix + sym.Symbol;
                            _subscriber.Subscribe(Encoding.UTF8.GetBytes(p));
                        }

                    }
                    //Send(request); //?股票为何单个注册
                }

                //TODO 更合理的方式是按交易所分组 单个交易所的行情 是统一一个行情源订阅的 这样可以实现 以交易所 批量订阅多个合约避免多次循环订阅的问题

                //将合约按照行情源类型进行分组
                //Dictionary<QSEnumDataFeedTypes, List<Symbol>> map = SplitSymbolViaDataFeedType(symbols);

                //foreach (KeyValuePair<QSEnumDataFeedTypes, List<Symbol>> kv in map)
                //{
                //    //string symlist = string.Join(",", kv.Value.Select(sym=>sym.Symbol));
                //    //将合约字头逐个向publisher进行订阅
                //    foreach (Symbol s in kv.Value)
                //    {
                //        string prefix = s.Symbol + "^";
                //        _subscriber.Subscribe(Encoding.UTF8.GetBytes(prefix));//subscribe对应的symbol字头用于获得tick数据
                //    }

                //    //通过FastTickServer的管理端口 请求FastTickServer向行情源订阅行情数据,Publisher的订阅是内部的一个分发订阅 不会产生向行情源订阅实际数据
                //    //注册合约协议格式 DATAFEED:SYMBOL|EXCHANGE
                //    foreach (var sym in kv.Value)
                //    {
                //        MDRegisterSymbolsRequest request = RequestTemplate<MDRegisterSymbolsRequest>.CliSendRequest(0);
                //        request.DataFeed = kv.Key;
                //        request.Exchange = sym.SecurityFamily.Exchange.EXCode;
                //        request.SymbolList.Add(sym.GetFullSymbol());
                //        //Send(request); //?股票为何单个注册
                //    }

                //}


            }
            catch (Exception ex)
            {
                logger.Error(":请求市场数据异常" + ex.ToString());
            }
        }

        /// <summary>
        /// 将合约按DataFeed进行分组
        /// </summary>
        /// <param name="basket"></param>
        /// <returns></returns>
        Dictionary<QSEnumDataFeedTypes,List<Symbol>> SplitSymbolViaDataFeedType(List<Symbol> basket)
        {
            Dictionary<QSEnumDataFeedTypes, List<Symbol>> map = new Dictionary<QSEnumDataFeedTypes, List<Symbol>>();
            foreach (Symbol sym in basket)
            {
                QSEnumDataFeedTypes type = sym.SecurityFamily.DataFeed;
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
        #endregion



    }
}
