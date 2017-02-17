using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public static class Util_Domain_Follow
    {
        /// <summary>
        /// 返回某个分区的所有跟单策略实例
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<FollowStrategy> GetFollowStrategies(this Domain domain)
        {
            return FollowTracker.FollowStrategyTracker.FollowStrategies.Where(st => st.Config.Domain_ID == domain.ID);
        }

        /// <summary>
        /// 返回分区下所有跟单策略配置数据
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<FollowStrategyConfig> GetFollowStrategyConfigs(this Domain domain)
        {
            return FollowTracker.StrategyCfgTracker.StrategyConfigs.Where(cfg => cfg.Domain_ID == domain.ID);
        }

        /// <summary>
        /// 获得分区下所有可用信号列表
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<SignalConfig> GetSignalConfigs(this Domain domain)
        {
            return FollowTracker.SignalTracker.SignalConfigs.Where(cfg => cfg.Domain_ID == domain.ID);
        }
    }
}
