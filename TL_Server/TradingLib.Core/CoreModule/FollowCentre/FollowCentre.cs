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
        bool _followstart = false;
        public void Start()
        {

            FollowTracker.Init();

            logger.Info("从配置文件加载跟单策略实例");
            //初始化跟单策略
            foreach (var cfg in FollowTracker.StrategyCfgTracker.StrategyConfigs)
            {
                InitStrategy(cfg);
            }
            
            //恢复跟单项目数据
            RestoreFollowItemData();

            //启动跟单策略
            foreach (var strategy in strategyMap.Values)
            {
                strategy.Start();
            }

            _followstart = true;

        }


        

        

        public void Stop()
        { 
        
        }
    }
}
