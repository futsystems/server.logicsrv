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

namespace TradingLib.Common.DataFarm
{
    public partial class DataServerBase
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
        /// 保存合约的成交数据
        /// 用户客户端查询成交数据
        /// 系统启动时需要加载
        /// </summary>
        Dictionary<string, ThreadSafeList<Tick>> symbolTradsMap = new Dictionary<string, ThreadSafeList<Tick>>();

        /// <summary>
        /// 记录了每个合约最近更新时间,当前tick时间要大于等于最近更新时间否则过滤掉
        /// </summary>
        Dictionary<string, DateTime> tickLastTimeMap = new Dictionary<string, DateTime>();

        readonly List<ITickFeed> _tickFeeds = new List<ITickFeed>();

        //异步处理行情组件,行情源组件获得行情更新后放入环形队列中进行处理
        AsyncResponse asyncTick;

        bool _tickFeedLoad = false;

        string _prefixStr = string.Empty;
        List<string> _prefixList = new List<string>();

        bool _acceptTick = true;

        protected void StartTickFeeds()
        {
            logger.Info("Start TickFeeds");
            //启动异步行情处理组件
            if (asyncTick == null)
            {
                logger.Info("Start async tick process");
                asyncTick = new AsyncResponse("Tick");
                asyncTick.GotTick +=new TickDelegate(asyncTick_GotTick);
            }
            asyncTick.Start();

            //加载TickFeeds
            LoadTickFeeds();

            //启动TickFeeds
            foreach (var feed in _tickFeeds)
            {
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
            
            //异步执行订阅操作
            foreach (var prefix in _prefixStr.Split(' '))
            {
                _prefixList.Add(prefix);
                tickfeed.Register(Encoding.UTF8.GetBytes(prefix));
            }
            //IEnumerable<string> symbols = MDBasicTracker.SymbolTracker.Symbols.Where(sym=>sym.Exchange=="NYMEX").Select(sym=>sym.Symbol);
            //tickfeed.RegisterSymbols(QSEnumDataFeedTypes.IQFEED, "NYMEX", symbols.ToList());

            foreach (var g in MDBasicTracker.SymbolTracker.Symbols.GroupBy(sym => sym.Exchange))
            { 
                IExchange exch = MDBasicTracker.ExchagneTracker[g.Key];
                List<Symbol> list = g.ToList<Symbol>();
                tickfeed.RegisterSymbols(exch, list);
            }
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
        /// 合约快照维护期
        /// </summary>
        TickTracker tickTracker = new TickTracker();

        void asyncTick_GotTick(Tick k)
        {
            Symbol symbol = MDBasicTracker.SymbolTracker[k.Exchange,k.Symbol];
            if(symbol == null) return;
            //if (symbol.Exchange != "HKEX") return;
            //更新行情最近更新时间
            if (!tickLastTimeMap.Keys.Contains(k.Symbol))
            {
                tickLastTimeMap.Add(k.Symbol, k.DateTime());
            }
            //执行行情事件检查

            //更新合约快照维护器 用于维护当前合约的一个最新状态
            tickTracker.UpdateTick(k);

            //如果是成交数据,盘口双边报价,统计数据 则我们生成行情快照对外发送 这里可以使用定时发送或者根据行情源事件类型来触发发送,为了提高效率与可考虑采用500ms快照方式发送，这样即保证时效性，又节约资源
            if (k.UpdateType == "X" || k.UpdateType == "Q" || k.UpdateType == "F" || k.UpdateType == "S")
            {
                //转发实时行情
                Tick snapshot = tickTracker[k.Exchange, k.Symbol];
                NotifyTick2Connections(snapshot);
            }

            //通过成交数据以及合约市场事件 驱动Bar数据生成器生成Bar数据
            if (k.UpdateType == "X" || k.UpdateType == "E")
            {
                FrequencyServiceProcessTick(k);
            }

            if (k.UpdateType == "S")
            {
                RestoreServiceProcessTickSnapshot(symbol, k);
            }

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
                foreach (var conn in target.Values)
                {
                    //logger.Info("send tick:" + k.Symbol);
                    //conn.SendTick(k);
                        TickNotify ticknotify = new TickNotify();
                        ticknotify.Tick = k;
                        //conn.Send(ticknotify);
                        this.SendData(conn, ticknotify);
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
                    Tick k = tickTracker[request.Exchange, symbol];
                    if (k != null)
                    {
                        TickNotify ticknotify = new TickNotify();
                        ticknotify.Tick = k;
                        this.SendData(conn, ticknotify);
                    }
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
