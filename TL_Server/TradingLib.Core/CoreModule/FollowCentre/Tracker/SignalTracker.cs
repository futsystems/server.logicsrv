using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;

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
        ConcurrentDictionary<int, ConcurrentDictionary<int, ISignal>> stragysignalmap = new ConcurrentDictionary<int, ConcurrentDictionary<int, ISignal>>();
        
        public SignalTracker()
        {
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
            foreach (var item in ORM.MSignal.SelectStrategySignalItems())
            {
                if (!stragysignalmap.Keys.Contains(item.StrategyID))
                {
                    stragysignalmap.TryAdd(item.StrategyID, new ConcurrentDictionary<int, ISignal>());
                }

                ISignal signal = this[item.SignalID];
                if (signal != null)
                {
                    stragysignalmap[item.StrategyID].TryAdd(item.SignalID, signal);
                }

            }
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
            ConcurrentDictionary<int, ISignal> target = null;
            if (stragysignalmap.TryGetValue(strategy_id, out target))
            {
                return target;
            }
            return null;
        }


    }
}
