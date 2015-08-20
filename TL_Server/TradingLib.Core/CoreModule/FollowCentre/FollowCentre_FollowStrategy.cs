using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{

    public partial class FollowCentre
    {

        /// <summary>
        /// 初始化跟单策略
        /// </summary>
        /// <param name="cfg"></param>
        void InitStrategy(FollowStrategyConfig cfg)
        {
            FollowStrategy strategy = FollowStrategy.CreateStrategy(cfg);
            strategyMap.TryAdd(strategy.ID, strategy);
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
    }
}
