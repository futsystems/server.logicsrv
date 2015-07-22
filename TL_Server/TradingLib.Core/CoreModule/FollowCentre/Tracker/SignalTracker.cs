using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using Common.Logging;

namespace TradingLib.Core
{
    /// <summary>
    /// 信号维护器
    /// 维护了所有信号源和跟单策略的信号组设定
    /// 
    /// </summary>
    public class SignalTracker
    {
        /// <summary>
        /// 信号设定map
        /// </summary>
        ConcurrentDictionary<int, SignalConfig> configmap = new ConcurrentDictionary<int, SignalConfig>();

        /// <summary>
        /// 信号数据库ID与信号的映射关系
        /// </summary>
        ConcurrentDictionary<int, ISignal> signalmap = new ConcurrentDictionary<int, ISignal>();

        /// <summary>
        /// 跟单策略数据库ID与策略信号组的映射关系，每个跟单策略有一组信号设定
        /// </summary>
        ConcurrentDictionary<int, ConcurrentDictionary<int, ISignal>> strategysignalmap = new ConcurrentDictionary<int, ConcurrentDictionary<int, ISignal>>();

        ILog logger = null;
        public SignalTracker()
        {
            logger = LogManager.GetLogger("Follow-SignalTracker");

            logger.Info("加载信号设置,初始化信号对象");
            //从数据库加载信号配置
            foreach (var cfg in ORM.MSignal.SelectSignalConfigs())
            {
                
                configmap.TryAdd(cfg.ID, cfg);

                try
                {
                    //创建信号实例对象加载到map
                    ISignal signal = SignalFactory.CreateSignal(cfg);
                    if (signal != null)
                    {
                        signalmap.TryAdd(cfg.ID, signal);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            //加载跟单策略的信号map 
            logger.Info("初始化策略信号映射关系");
            foreach (var item in ORM.MSignal.SelectStrategySignalItems())
            {
                if (!strategysignalmap.Keys.Contains(item.StrategyID))
                {
                    strategysignalmap.TryAdd(item.StrategyID, new ConcurrentDictionary<int, ISignal>());
                }

                ISignal signal = this[item.SignalID];
                if (signal != null)
                {
                    strategysignalmap[item.StrategyID].TryAdd(item.SignalID, signal);
                }

            }
            //
        }

        /// <summary>
        /// 信号统一使用数据库ID进行标识
        /// 交易账户token和通道token有可能出现重复
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ISignal this[int id]
        {
            get
            {
                ISignal target = null;
                if (signalmap.TryGetValue(id, out target))
                {
                    return target;
                }
                return null;
            }
        }


        /// <summary>
        /// 查找某个跟单策略的信号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ConcurrentDictionary<int, ISignal> GetStrategySignals(int strategy_id)
        { 
            if (!strategysignalmap.Keys.Contains(strategy_id))
            {
                strategysignalmap.TryAdd(strategy_id, new ConcurrentDictionary<int, ISignal>());
            }
            return strategysignalmap[strategy_id];
        }


    }
}
