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
    public class FollowStrategyCfgTracker
    {
        ConcurrentDictionary<int, FollowStrategyConfig> id2configMap = new ConcurrentDictionary<int, FollowStrategyConfig>();
        ConcurrentDictionary<string, FollowStrategyConfig> token2configMap = new ConcurrentDictionary<string, FollowStrategyConfig>();



        public FollowStrategyCfgTracker()
        {
            foreach (var cfg in ORM.MStrategy.SelectFollowStrategyConfigs())
            {
                id2configMap.TryAdd(cfg.ID, cfg);
                token2configMap.TryAdd(cfg.Token, cfg);
            }
        }




        /// <summary>
        /// 通过数据库ID获得测量配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FollowStrategyConfig this[int id]
        {
            get
            {
                FollowStrategyConfig cfg = null;
                if (id2configMap.TryGetValue(id, out cfg))
                {
                    return cfg;
                }
                return null;
            }
        }

        /// <summary>
        /// 通过跟单Token获得跟单配置
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public FollowStrategyConfig this[string token]
        {
            get
            {
                FollowStrategyConfig cfg = null;
                if (token2configMap.TryGetValue(token, out cfg))
                {
                    return cfg;
                }
                return null;
            }
        }

        /// <summary>
        /// 获得所有跟单策略配置
        /// </summary>
        public IEnumerable<FollowStrategyConfig> StrategyConfigs
        {
            get
            {
                return id2configMap.Values;
            }
        }

    }
}
