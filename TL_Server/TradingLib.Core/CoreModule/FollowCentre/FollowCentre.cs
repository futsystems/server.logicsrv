using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{

    public partial class FollowCentre : BaseSrvObject,IModuleFollowCentre
    {

        const string CoreName = "FollowCentre";

        public string CoreId { get { return CoreName; } }


        public FollowCentre()
            : base(FollowCentre.CoreName)
        {
            FollowTracker.NotifyTradeFollowItemEvent += new Action<TradeFollowItem>(NotifyFollowItem);
        }


        ConcurrentDictionary<int, FollowStrategy> strategyMap = new ConcurrentDictionary<int, FollowStrategy>();
        public void Start()
        {

            FollowTracker.Init();

            //从配置文件中加载策略
            logger.Info("从配置文件加载跟单策略实例");
            foreach (var cfg in FollowTracker.StrategyCfgTracker.StrategyConfigs)
            {
                FollowStrategy strategy = FollowStrategy.CreateStrategy(cfg);
                strategyMap.TryAdd(strategy.ID, strategy);
                //启动跟单策略
                strategy.Start();
            }
            
        }


        /// <summary>
        /// 对外通知跟单项目
        /// </summary>
        /// <param name="item"></param>
        public void NotifyFollowItem(TradeFollowItem item)
        {
            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                TLCtxHelper.ModuleMgrExchange.Notify("FollowCentre", "EntryFollowItemNotify", item.GenEntryFollowItemStruct(), null);
            }
            else
            {
                TLCtxHelper.ModuleMgrExchange.Notify("FollowCentre", "ExitFollowItemNotify", item.GenExitFollowItemStruct(), null);
            }
        }

        /// <summary>
        /// 通过策略ID编号获得策略对象
        /// </summary>
        /// <param name="strategyID"></param>
        /// <returns></returns>
        FollowStrategy ID2FollowStrategy(int strategyID)
        {
            FollowStrategy target = null;
            if (strategyMap.TryGetValue(strategyID, out target))
            {
                return target;
            }
            return null;
        }

        public void Stop()
        { 
        
        }
    }
}
