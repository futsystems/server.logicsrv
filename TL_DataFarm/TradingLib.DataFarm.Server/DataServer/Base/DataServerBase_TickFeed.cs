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

        
        /// <summary>
        /// 加载行情服务
        /// </summary>
        void LoadTickFeeds()
        {
            if (_tickFeedLoad) return;
            string filter = this.ConfigFile["TickFeedFilter"].AsString();
            //如果TickFeed为* 加载所有 否则指定对应的类型
            foreach (var feed in LoadPlugins<ITickFeed>(_TickFeedFolder, filter))
            {
                _tickFeeds.Add(feed);
            }
            _tickFeedLoad = true;
        }

        protected void StartTickFeeds()
        {
            if (asyncTick == null)
            {
                asyncTick = new AsyncResponse("Tick");
                asyncTick.GotTick +=new TickDelegate(asyncTick_GotTick);
            }
            //启动Tick异步响应组件
            asyncTick.Start();

            //加载TickFeeds
            LoadTickFeeds();

            //启动TickFeeds
            foreach (var feed in _tickFeeds)
            {
                StartTickFeed(feed);
            }
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
            
            //行情过滤
            if (k == null) return;
            if (!k.isValid) return;
            //logger.Debug("datafeed got tick");
            asyncTick.newTick(k);

        }

        void asyncTick_GotTick(Tick k)
        {
            //logger.Debug("Process Tick");
            if (!tickLastTimeMap.Keys.Contains(k.Symbol))
            {
                tickLastTimeMap.Add(k.Symbol, k.Datetime);
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
                    conn.SendTick(k);
                }
            }
        }

        void OnRegisterSymbol(IConnection conn, RegisterSymbolTickRequest request)
        {
            foreach (var symbol in request.SymbolList)
            {
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


        void OnUngisterSymbol(IConnection conn, UnregisterSymbolTickRequest request)
        {
            foreach (var symbol in request.SymbolList)
            { 
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
