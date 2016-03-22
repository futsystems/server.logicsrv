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

namespace TradingLib.DataFarm.Common
{
    public partial class DataServerBase
    {
        /// <summary>
        /// TickFeed插件目录
        /// </summary>
        private readonly string _TickFeedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TickFeed");
        private readonly List<ITickFeed> _TickFeeds = new List<ITickFeed>();


        ConcurrentDictionary<string, ConcurrentDictionary<string, IConnection>> symRegMap = new ConcurrentDictionary<string, ConcurrentDictionary<string, IConnection>>();

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
            logger.Info(string.Format("TickFeed[{0}] connected",tickfeed.Name));
        }

        /// <summary>
        /// 响应行情源获得行情回报
        /// </summary>
        void TickFeed_OnTickEvent(ITickFeed tickfeed, Tick k)
        {
            //logger.Info("0000");
            //行情过滤
            if (k == null) return;
            if (!k.IsValid()) return;
            asyncTick.newTick(k);

        }

        void asyncTick_GotTick(Tick k)
        {
            //logger.Debug("Process Tick");
            //更新行情最近更新时间
            if (!tickLastTimeMap.Keys.Contains(k.Symbol))
            {
                tickLastTimeMap.Add(k.Symbol, k.DateTime());
            }
            //执行行情事件检查

            //if(k.datetickLastTimeMap[k.Symbol])
            //logger.Info("async process tick");

            //转发实时行情
            NotifyTick2Connections(k);

            //生成Bar数据
            //freqService.ProcessTick(k);

            //保存行情
            //SaveTick(k);

            //
        }


        /// <summary>
        /// 向客户端通知行情回报
        /// </summary>
        /// <param name="k"></param>
        void NotifyTick2Connections(Tick k)
        {
            ConcurrentDictionary<string, IConnection> target = null;
            if (symRegMap.TryGetValue(k.Symbol, out target))
            {
                foreach (var conn in target.Values)
                {
                    logger.Info("send tick:" + k.Symbol);
                    conn.SendTick(k);
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
            foreach (var regpair in symRegMap)
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
            foreach (var symbol in request.SymbolList)
            {
                if (string.IsNullOrEmpty(symbol)) continue;
                Symbol sym = MDBasicTracker.SymbolTracker[symbol];
                if (sym == null) continue;

                if (!symRegMap.Keys.Contains(symbol))
                {
                    symRegMap.TryAdd(symbol, new  ConcurrentDictionary<string,IConnection>());
                }
                ConcurrentDictionary<string,IConnection> regmap = symRegMap[symbol];
                if(!regmap.Keys.Contains(conn.SessionID))
                {
                    regmap.TryAdd(conn.SessionID,conn);
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

                if (symRegMap.Keys.Contains(symbol))
                {
                    ConcurrentDictionary<string, IConnection> regmap = symRegMap[symbol];
                    IConnection target = null;
                    regmap.TryRemove(conn.SessionID, out target);
                }
            }
        }


    }
}
