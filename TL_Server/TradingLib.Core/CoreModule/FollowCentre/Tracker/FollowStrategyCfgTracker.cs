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
        public FollowStrategyCfgTracker()
        {
            foreach (var cfg in ORM.MStrategy.SelectFollowStrategyConfigs())
            {
                id2configMap.TryAdd(cfg.ID, cfg);
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
        /// 获得所有跟单策略配置
        /// </summary>
        public IEnumerable<FollowStrategyConfig> StrategyConfigs
        {
            get
            {
                return id2configMap.Values;
            }
        }

        /// <summary>
        /// 更新跟单策略(包含添加跟单策略逻辑)
        /// </summary>
        /// <param name="cfg"></param>
        public void UpdateFollowStrategyConfig(FollowStrategyConfig cfg)
        {
            FollowStrategyConfig target = null;
            if (id2configMap.TryGetValue(cfg.ID, out target))
            {
                //方向/token不可修改
                target.FollowPower = cfg.FollowPower;
                target.EntryPriceType = cfg.EntryPriceType;
                target.EntryOffsetTicks = cfg.EntryOffsetTicks;
                target.EntryPendingThresholdType = cfg.EntryPendingThresholdType;
                target.EntryPendingThresholdValue = cfg.EntryPendingThresholdValue;
                target.EntryPendingOperationType = cfg.EntryPendingOperationType;

                target.ExitPriceType = cfg.ExitPriceType;
                target.ExitOffsetTicks = cfg.ExitOffsetTicks;
                target.ExitPendingThreadsholdType = cfg.ExitPendingThreadsholdType;
                target.ExitPendingThresholdValue = cfg.ExitPendingThresholdValue;
                target.ExitPendingOperationType = cfg.ExitPendingOperationType;

                target.SecFilter = cfg.SecFilter;
                target.TimeFilter = cfg.TimeFilter;
                target.SizeFilter = cfg.SizeFilter;
                target.StopEnable = cfg.StopEnable;
                target.StopValue = cfg.StopValue;
                target.StopValueType = cfg.StopValueType;
                target.Profit1Enable = cfg.Profit1Enable;
                target.Profit1Value = cfg.Profit1Value;
                target.Profit1ValueType = cfg.Profit1ValueType;
                target.Profit2Enable = cfg.Profit2Enable;
                target.Profit2Value1 = cfg.Profit2Value1;
                target.Profit2Trailing1 = cfg.Profit2Trailing1;
                target.Profit2Value1Type = cfg.Profit2Value1Type;
                target.Profit2Value2 = cfg.Profit2Value2;
                target.Profit2Trailing2 = cfg.Profit2Trailing2;
                target.Profit2Value2Type = cfg.Profit2Value2Type;

                target.Desp = cfg.Desp;

                ORM.MStrategy.UpdateFollowStrategyConfig(target);

            }
            else
            {
                target = new FollowStrategyConfig();

                target.Account = cfg.Account;
                target.Domain_ID = cfg.Domain_ID;
                target.FollowDirection = cfg.FollowDirection;
                target.FollowPower = cfg.FollowPower;
                target.Token = cfg.Token;
                target.Desp = cfg.Desp;

                target.EntryPriceType = cfg.EntryPriceType;
                target.EntryOffsetTicks = cfg.EntryOffsetTicks;
                target.EntryPendingThresholdType = cfg.EntryPendingThresholdType;
                target.EntryPendingThresholdValue = cfg.EntryPendingThresholdValue;
                target.EntryPendingOperationType = cfg.EntryPendingOperationType;

                target.ExitPriceType = cfg.ExitPriceType;
                target.ExitOffsetTicks = cfg.ExitOffsetTicks;
                target.ExitPendingThreadsholdType = cfg.ExitPendingThreadsholdType;
                target.ExitPendingThresholdValue = cfg.ExitPendingThresholdValue;
                target.ExitPendingOperationType = cfg.ExitPendingOperationType;

                target.SecFilter = cfg.SecFilter;
                target.TimeFilter = cfg.TimeFilter;
                target.SizeFilter = cfg.SizeFilter;
                target.StopEnable = cfg.StopEnable;
                target.StopValue = cfg.StopValue;
                target.StopValueType = cfg.StopValueType;
                target.Profit1Enable = cfg.Profit1Enable;
                target.Profit1Value = cfg.Profit1Value;
                target.Profit1ValueType = cfg.Profit1ValueType;
                target.Profit2Enable = cfg.Profit2Enable;
                target.Profit2Value1 = cfg.Profit2Value1;
                target.Profit2Trailing1 = cfg.Profit2Trailing1;
                target.Profit2Value1Type = cfg.Profit2Value1Type;
                target.Profit2Value2 = cfg.Profit2Value2;
                target.Profit2Trailing2 = cfg.Profit2Trailing2;
                target.Profit2Value2Type = cfg.Profit2Value2Type;

                target.Desp = cfg.Desp;

                ORM.MStrategy.InsertFollowStrategyConfig(target);

                //放入内存数据结构
                id2configMap.TryAdd(target.ID, target);
                cfg.ID = target.ID;
            }
        }

    }
}
