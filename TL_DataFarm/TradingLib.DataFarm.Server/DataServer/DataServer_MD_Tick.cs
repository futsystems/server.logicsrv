using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm;

namespace TradingLib.DataFarm.Common
{

    public partial class DataServer
    {
        ConcurrentDictionary<string, ThreadSafeList<DataFarm.API.IConnection>> symRegMap = new ConcurrentDictionary<string, ThreadSafeList<API.IConnection>>();

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


        //异步处理行情组件,行情源组件获得行情更新后放入环形队列中进行处理
        AsyncResponse asyncTick = new AsyncResponse("Tick");

        FastTickDataFeed datafeed = null;
        /// <summary>
        /// 初始化行情服务
        /// </summary>
        void InitTickService()
        {

            datafeed = new FastTickDataFeed("139.196.49.175", "127.0.0.1", 9000, 9001);

            datafeed.OnConnectEvent += new Action(datafeed_OnConnectEvent);
            datafeed.OnDisconnectEvent += new Action(datafeed_OnDisconnectEvent);
            datafeed.OnTickEvent += new Action<Tick>(OnTickEvent);

            asyncTick.GotTick += new TickDelegate(asyncTick_GotTick);
            asyncTick.Start();

            datafeed.Start();
        }



        void datafeed_OnDisconnectEvent()
        {
            logger.Info("DataFeed Disconnected");
        }

        void datafeed_OnConnectEvent()
        {
            logger.Info("DataFeed Connected");
        }


        /// <summary>
        /// 响应行情源获得行情回报
        /// </summary>
        void OnTickEvent(Tick k)
        {
            //行情过滤
            if (k == null) return;
            if (!k.isValid) return;
            //logger.Info("datafeed got tick");
            asyncTick.newTick(k);
            
        }

        void asyncTick_GotTick(Tick k)
        {
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
            freqService.ProcessTick(k);

            //保存行情
            SaveTick(k);

            //
        }


        /// <summary>
        /// 向客户端通知行情回报
        /// </summary>
        /// <param name="k"></param>
        void NotifyTick2Connections(Tick k)
        { 
            ThreadSafeList<DataFarm.API.IConnection> target = null;
            if (symRegMap.TryGetValue(k.Symbol, out target))
            {
                foreach (var conn in target)
                {
                    conn.SendTick(k);
                }
            }
        }





    }
}
