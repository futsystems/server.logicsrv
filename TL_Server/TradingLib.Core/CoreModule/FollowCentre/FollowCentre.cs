using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{

    public class FollowCentre : BaseSrvObject,IModuleFollowCentre
    {

        const string CoreName = "FollowCentre";

        public string CoreId { get { return CoreName; } }


        public FollowCentre()
            : base(FollowCentre.CoreName)
        { 
            
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

        

        public void Stop()
        { 
        
        }
    }
}
