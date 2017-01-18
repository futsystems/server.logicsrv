using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;

namespace TradingLib.DataFarm.Common
{
    public partial class DataServer
    {
        /// <summary>
        /// TickFeed插件目录
        /// </summary>
        private readonly string _TickFeedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TickFeed");
       
        /// <summary>
        /// 实时行情注册map
        /// 合约唯一键值 与 Connection的映射关系
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<string, IConnection>> symKeyRegMap = new ConcurrentDictionary<string, ConcurrentDictionary<string, IConnection>>();

        /// <summary>
        /// 行情插件列表
        /// </summary>
        readonly List<ITickFeed> _tickFeeds = new List<ITickFeed>();

        //异步处理行情组件,行情源组件获得行情更新后放入环形队列中进行处理
        AsyncResponse asyncTick;

        bool _tickFeedLoad = false;

        string _prefixStr = string.Empty;
        List<string> _prefixList = new List<string>();

        bool _acceptTick = true;

        ITickFeed _defaultFeed = null;
        protected void StartTickFeeds()
        {
            logger.Info("[Start Tick Feeds]");
            //启动异步行情处理组件
            if (asyncTick == null)
            {
                asyncTick = new AsyncResponse("Tick");
                asyncTick.GotTick +=new TickDelegate(asyncTick_GotTick);
            }
            asyncTick.Start();

            //加载TickFeeds
            LoadTickFeeds();

            //启动TickFeeds
            foreach (var feed in _tickFeeds)
            {
                if (_defaultFeed == null)
                {
                    _defaultFeed = feed;
                }
                StartTickFeed(feed);
            }
        }

        /// <summary>
        /// 加载行情服务
        /// </summary>
        void LoadTickFeeds()
        {
            if (_tickFeedLoad) return;
            logger.Info("Load TickFeeds plugin");
            string filter = this.ConfigFile["TickFeedFilter"].AsString();
            //如果TickFeed为* 加载所有 否则指定对应的类型
            foreach (var feed in LoadPlugins<ITickFeed>(_TickFeedFolder, filter))
            {
                _tickFeeds.Add(feed);
            }
            _tickFeedLoad = true;
        }


        void StartTickFeed(ITickFeed tickfeed)
        {
            tickfeed.ConnectEvent += new Action<ITickFeed>(TickFeed_OnConnectEvent);
            tickfeed.DisconnectEvent += new Action<ITickFeed>(TickFeed_OnDisconnectEvent);
            tickfeed.TickEvent +=new Action<ITickFeed,Tick>(TickFeed_OnTickEvent);
            tickfeed.Start();
        }


        void TickFeed_OnDisconnectEvent(ITickFeed tickfeed)
        {
            logger.Info(string.Format("TickFeed[{0}] disconnected", tickfeed.Name));
        }

        
        void TickFeed_OnConnectEvent(ITickFeed tickfeed)
        {
            

            _prefixStr = ConfigFile["Prefix"].AsString();

            logger.Info(string.Format("TickFeed[{0}] connected, will register prefix:{1}",tickfeed.Name,_prefixStr));
            
            //订阅前缀
            foreach (var prefix in _prefixStr.Split(' '))
            {
                _prefixList.Add(prefix);
                tickfeed.Register(Encoding.UTF8.GetBytes(prefix));
            }
            //向TickPubSrv发起合约实时行情注册请求
            foreach (var g in MDBasicTracker.SymbolTracker.Symbols.GroupBy(sym => sym.Exchange))
            { 
                Exchange exch = MDBasicTracker.ExchagneTracker[g.Key];
                List<Symbol> list = g.ToList<Symbol>();
                tickfeed.RegisterSymbols(exch, list);
            }

            _connectedTime = DateTime.Now;
            _switching = false;
        }

        /// <summary>
        /// 响应行情源获得行情回报
        /// </summary>
        void TickFeed_OnTickEvent(ITickFeed tickfeed, Tick k)
        {
            //行情过滤
            if (k == null) return;
            if (_acceptTick)
            {
                asyncTick.newTick(k);
            }

        }

        /// <summary>
        /// 行情源时间维护Map
        /// </summary>
        Dictionary<QSEnumDataFeedTypes, DataFeedTime> dfTimeMap = new Dictionary<QSEnumDataFeedTypes, DataFeedTime>();

        DataFeedTime GetDataFeedTime(QSEnumDataFeedTypes df)
        { 
            DataFeedTime target = null;
            if(dfTimeMap.TryGetValue(df,out target))
            {
                return target;
            }
            return null;
        }

        bool _switching = false;
        DateTime _connectedTime = DateTime.Now;
        void SwitchTickSrv(string msg)
        {
            if (_switching) return;
            _switching = true;
            logger.Info(msg);
            if (_tickFeeds.Count > 0)
            {
                ITickFeed feed = _tickFeeds[0];
                logger.Info(string.Format("Switch TickSrv of Feed:{0}", feed.Name));
                feed.SwitchTickSrv();
            }
            else
            {
                logger.Warn("TickFeed not loaded");
            }
        }

        void CheckIQFeedTimeTick()
        { 
            DataFeedTime dft = null;
            if (dfTimeMap.TryGetValue(QSEnumDataFeedTypes.IQFEED, out dft))
            {
                //建立连接10秒之内不执行检查 需要等待连接建立且服务端发送数据
                //TimeTick 5秒内未更新 则触发切换操作
                if (DateTime.Now.Subtract(_connectedTime).TotalSeconds> 5 && DateTime.Now.Subtract(dft.LastHeartBeat).TotalSeconds > 5)
                {

                    string msg = string.Format("IQFeed DateTime Tick Stream Lost");
                    SwitchTickSrv(msg);
                }
            }
        }

        void asyncTick_GotTick(Tick k)
        {
            //更新行情源时间 
            if (k.UpdateType == "T")
            {
                DataFeedTime dft = null;
                if (!dfTimeMap.TryGetValue(k.DataFeed, out dft))
                {
                    dft = new DataFeedTime(k.DataFeed);
                    dft.StartTime = k.DateTime();
                    dft.LastHeartBeat = DateTime.Now;
                    dfTimeMap.Add(k.DataFeed, dft);
                }
                dft.CurrentTime = k.DateTime();
                dft.LastHeartBeat = DateTime.Now;
                return;
            }

            //获得行情Tick对应合约
            Symbol symbol = MDBasicTracker.SymbolTracker[k.Exchange,k.Symbol];
            if(symbol == null) return;

            //更新合约快照维护器 用于维护当前合约的一个最新状态
            Global.TickTracker.UpdateTick(k);

            //如果是成交数据,盘口双边报价,统计数据 则我们生成行情快照对外发送 这里可以使用定时发送或者根据行情源事件类型来触发发送,为了提高效率与可考虑采用500ms快照方式发送，这样即保证时效性，又节约资源
            if (k.UpdateType == "X" || k.UpdateType == "Q" || k.UpdateType == "F" || k.UpdateType == "S")
            {
                Tick snapshot = Global.TickTracker[k.Exchange, k.Symbol];
                //每次成交推送行情快照
                if (k.UpdateType == "X")
                {
                    NotifyTick2Connections(snapshot);
                    snapshot.QuoteUpdate = false;
                }
                else
                {
                    //每500ms推送其余数据
                    snapshot.QuoteUpdate = true;
                }
            }

            //通过成交数据以及合约市场事件 驱动Bar数据生成器生成Bar数据
            if (k.UpdateType == "X" || k.UpdateType == "E")
            {
                FrequencyServiceProcessTick(k);
            }

            //Eod服务保存成交数据
            if (k.UpdateType == "X" || k.UpdateType == "F" || k.UpdateType == "S")
            {
                EodServiceProcessTick(k);
            }

            //if (k.UpdateType == "S")
            //{
            //    RestoreServiceProcessTickSnapshot(symbol, k);
            //}
        }

        void SendTickSnapshot()
        {
            foreach (var tick in Global.TickTracker.TickSnapshots)
            {
                if (!tick.QuoteUpdate) continue;
                NotifyTick2Connections(tick);
                tick.QuoteUpdate = false;
                
            }
        }

        byte[] CreateTLTickData(Tick k)
        {
            TickNotify ticknotify = new TickNotify();
            ticknotify.Tick = k;
            return  ticknotify.Data;
        }
        byte[] CreateXLTickData(Tick k)
        {
            XLPacketData pkt = new XLPacketData(XLMessageType.T_RTN_MARKETDATA);
            XLDepthMarketDataField data = new XLDepthMarketDataField();

            data.Date = k.Date;
            data.Time = k.Time;
            data.TradingDay = 0;

            data.SymbolID = k.Symbol;
            data.ExchangeID = k.Exchange;
            data.LastPrice = (double)k.Trade;
            data.PreSettlementPrice = (double)k.PreSettlement;
            data.PreClosePrice = (double)k.PreClose;
            data.PreOpenInterest = k.PreOpenInterest;
            data.OpenPrice = (double)k.Open;
            data.HighestPrice = (double)k.High;
            data.LowestPrice = (double)k.Low;
            data.Volume = k.Vol;
            data.OpenInterest = k.OpenInterest;
            data.ClosePrice = (double)k.Trade;
            data.SettlementPrice = (double)k.Settlement;
            data.UpperLimitPrice = (double)k.UpperLimit;
            data.LowerLimitPrice = (double)k.LowerLimit;
            data.BidPrice1 = (double)k.BidPrice;
            data.BidVolume1 = k.BidSize;
            data.AskPrice1 = (double)k.AskPrice;
            data.AskVolume1 = k.AskSize;
            pkt.AddField(data);

            return XLPacketData.PackToBytes(pkt, XLEnumSeqType.SeqRtn, (uint)0, (uint)0, true);
        }


        /// <summary>
        /// 向客户端通知行情回报
        /// </summary>
        /// <param name="k"></param>
        void NotifyTick2Connections(Tick k)
        {
            ConcurrentDictionary<string, IConnection> target = null;
            if (symKeyRegMap.TryGetValue(k.GetSymbolUniqueKey(), out target))
            {
                //创建不同协议的行情数据
                var tldata = CreateTLTickData(k);
                var xldata = CreateXLTickData(k);
                //遍历所有连接 按连接类型将数据发送到客户端
                foreach (var conn in target.Values)
                {
                    switch (conn.ProtocolType)
                    {
                        case EnumConnProtocolType.TL:
                            {
                                this.SendData(conn, tldata);
                                break;
                            }
                        case EnumConnProtocolType.XL:
                            {
                                this.SendData(conn, xldata);
                                break;
                            }
                        default:
                            logger.Warn(string.Format("Conn ProtocolType:{0} not handled", conn.ProtocolType));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 注销某个连接的所有行情注册
        /// </summary>
        /// <param name="conn"></param>
        void ClearSymbolRegisted(IConnection conn)
        {
            logger.Info(string.Format("Clear symbols registed for conn:{0}", conn.SessionID));
            IConnection target = null;
            foreach (var regpair in symKeyRegMap)
            {
                if (regpair.Value.Keys.Contains(conn.SessionID))
                {
                    regpair.Value.TryRemove(conn.SessionID, out target);
                }
            }
        }

        /// <summary>
        /// 某个连接注册合约行情
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        void OnRegisterSymbol(IConnection conn, RegisterSymbolTickRequest request)
        {
            if (string.IsNullOrEmpty(request.Exchange))
            {
                logger.Warn("Register Symbol Tick Need Exhcnange");
                return;
            }
            foreach (var symbol in request.SymbolList)
            {
                if (string.IsNullOrEmpty(symbol)) continue;
                string key = string.Format("{0}-{1}", request.Exchange, symbol);
                
                Symbol sym = MDBasicTracker.SymbolTracker[request.Exchange,symbol];
                if (sym == null) continue;

                if (!symKeyRegMap.Keys.Contains(key))
                {
                    symKeyRegMap.TryAdd(key, new ConcurrentDictionary<string, IConnection>());
                }
                ConcurrentDictionary<string, IConnection> regmap = symKeyRegMap[key];
                if(!regmap.Keys.Contains(conn.SessionID))
                {
                    regmap.TryAdd(conn.SessionID,conn);
                    //客户端订阅后发送当前市场快照
                    Tick k = Global.TickTracker[request.Exchange, symbol];
                    if (k != null)
                    {
                        TickNotify ticknotify = new TickNotify();
                        ticknotify.Tick = k;
                        this.SendData(conn, ticknotify);
                    }
                }
            }
        }

        void OnXLRegisterSymbol(IConnection conn, XLSpecificSymbolField request)
        {
            if (string.IsNullOrEmpty(request.SymbolID))
            {
                logger.Warn("Symbol Filed Empty");
            }
            Symbol sym = MDBasicTracker.SymbolTracker[string.Empty,request.SymbolID];
            if (sym == null)
            {
                logger.Warn(string.Format("Symbol:{0} do not exist", request.SymbolID));
            }
            string key = sym.UniqueKey;
            if (!symKeyRegMap.Keys.Contains(key))
            {
                symKeyRegMap.TryAdd(key, new ConcurrentDictionary<string, IConnection>());
            }
            ConcurrentDictionary<string, IConnection> regmap = symKeyRegMap[key];
            if(!regmap.Keys.Contains(conn.SessionID))
            {
                regmap.TryAdd(conn.SessionID,conn);
                logger.Info(string.Format("Symbol:{0} Registed", request.SymbolID));
                //客户端订阅后发送当前市场快照
                Tick k = Global.TickTracker[sym.Exchange,sym.Symbol];
                if (k != null)
                {
                    //TickNotify ticknotify = new TickNotify();
                    //ticknotify.Tick = k;
                    //this.SendData(conn, ticknotify);
                }
            }
        
        }

        /// <summary>
        /// 某个连接注销合约行情
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        void OnUngisterSymbol(IConnection conn, UnregisterSymbolTickRequest request)
        {
            foreach (var symbol in request.SymbolList)
            {
                if (string.IsNullOrEmpty(symbol)) continue;

                //注销所有合约
                if (symbol == "*")
                {
                    ClearSymbolRegisted(conn);
                    break;
                }
                if (symKeyRegMap.Keys.Contains(symbol))
                {
                    ConcurrentDictionary<string, IConnection> regmap = symKeyRegMap[symbol];
                    IConnection target = null;
                    regmap.TryRemove(conn.SessionID, out target);
                }
            }
        }


    }
}
