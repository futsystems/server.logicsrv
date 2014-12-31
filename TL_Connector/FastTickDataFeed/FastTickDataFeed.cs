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
        
        string server="127.0.0.1";
        string server2 = string.Empty;
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

        int _timeout = 3;
        bool _switch = true;
        
        public void Start()
        {
            if (_switch)//优先使用主行情服务
            {
                server = _cfg.srvinfo_ipaddress;
                server2 = _cfg.srvinfo_field2;
            }
            else//反转，当主行情异常时连接备用行情
            {
                server = _cfg.srvinfo_field2;
                server2 = _cfg.srvinfo_ipaddress;
            }

            port = _cfg.srvinfo_port;
            int outreq = 6661;
            int.TryParse(_cfg.srvinfo_field1,out outreq);
            reqport = outreq;

            if (Util.IsServerPortOpened(server, port, _timeout))
            {
                debug("Server:" + server +" is avabile", QSEnumDebugLevel.INFO);
            }
            else
            {
                //如果第一个服务地址异常 则反转服务器连接顺序
                _switch = !_switch;
                debug("Server:" + server + " is not avabile,try another server.", QSEnumDebugLevel.WARNING);
                Util.sleep(1000);
                if (Util.IsServerPortOpened(server2, port, _timeout))
                {
                    debug("Server:" + server2 + " is avabile,use it", QSEnumDebugLevel.INFO);
                    server = server2;
                }
                else
                {
                    debug("Servers are not avabile both,stop...", QSEnumDebugLevel.ERROR);
                    return;
                }
            }

            if (_tickgo) return;
            _tickgo = true;
            _tickthread = new Thread(TickHandler);
            _tickthread.IsBackground = true;
            _tickthread.Name = "FasktTickDF TickHandler";
            _tickthread.Start();
            ThreadTracker.Register(_tickthread);

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
                    //subscriber.SubscribeAll();
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
                                subscriber.Close();
                                symbolreq.Close();
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
        public void RegisterSymbols(SymbolBasket symbols)
        {
            try
            {
                //将合约按照行情源类型进行分组
                Dictionary<QSEnumDataFeedTypes, List<Symbol>> map = SplitSymbolViaDataFeedType(symbols);

                foreach (KeyValuePair<QSEnumDataFeedTypes, List<Symbol>> kv in map)
                {
                    string symlist = string.Join(",", kv.Value.Select(sym=>sym.Symbol));
                    //将合约字头逐个向publisher进行订阅
                    foreach (Symbol s in kv.Value)
                    {
                        string prefix = s.Symbol + "^";
                        _subscriber.Subscribe(Encoding.UTF8.GetBytes(prefix));//subscribe对应的symbol字头用于获得tick数据
                    }

                    //通过FastTickServer的管理端口 请求FastTickServer向行情源订阅行情数据,Publisher的订阅是内部的一个分发订阅 不会产生向行情源订阅实际数据
                    string requeststr = (kv.Key.ToString() + ":" + symlist);
                    debug(Token + " RegisterSymbol " + requeststr, QSEnumDebugLevel.INFO);
                    Send(TradingLib.API.MessageTypes.MGRREGISTERSYMBOLS, requeststr);
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
                QSEnumDataFeedTypes type = Symbol2DataFeedType(sym);
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
            //国内期货合约通过CTP通道订阅
            if (symbol.SecurityType == SecurityType.FUT && symbol.SecurityFamily.Exchange.Country == Country.CN)
            {
                return QSEnumDataFeedTypes.CTP;
            }

            //国内期权合约通过CTPOPT通道订阅
            if (symbol.SecurityType == SecurityType.OPT && symbol.SecurityFamily.Exchange.Country == Country.CN)
            {
                return QSEnumDataFeedTypes.CTPOPT;
            }
            return QSEnumDataFeedTypes.CTP;
        }


    }
}
