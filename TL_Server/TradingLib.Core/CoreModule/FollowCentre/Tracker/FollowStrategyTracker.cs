using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 跟单策略维护器
    /// </summary>
    public class FollowStrategyTracker
    {
        ConcurrentDictionary<int, FollowStrategyConfig> id2configMap = new ConcurrentDictionary<int, FollowStrategyConfig>();
        ConcurrentDictionary<string, FollowStrategyConfig> token2configMap = new ConcurrentDictionary<string, FollowStrategyConfig>();



        public FollowStrategyTracker()
        {
            foreach (var cfg in ORM.MStrategy.SelectFollowStrategyConfigs())
            {
                id2configMap.TryAdd(cfg.ID, cfg);
                token2configMap.TryAdd(cfg.Token, cfg);
            }
        }


    }
}
