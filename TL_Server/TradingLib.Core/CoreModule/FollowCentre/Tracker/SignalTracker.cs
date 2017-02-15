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
            //遍历所有交易账户 恢复信号设置

            IEnumerable<SignalConfig> tmp = ORM.MSignal.SelectSignalConfigs();
            
            //遍历所有子账户和信号账户
            foreach (IAccount account in TLCtxHelper.ModuleAccountManager.Accounts.Where(acc => (acc.Category == QSEnumAccountCategory.SUBACCOUNT || acc.Category == QSEnumAccountCategory.SIGACCOUNT)))
            {
                //如果信号列表中不包含该帐户 则添加
                if (!tmp.Any(sig => sig.SignalToken == account.ID))
                {
                    ORM.MSignal.InsertSignalConfig(account.ID);
                }
            }
            //遍历信号 将没有对应交易账户的信号删除

            //从数据库加载信号配置并创建信号对象
            foreach (var cfg in ORM.MSignal.SelectSignalConfigs())
            {
                configmap.TryAdd(cfg.ID, cfg);
                try
                {
                    ISignal signal = SignalFactory.CreateSignal(cfg);
                    if (signal != null)
                    {
                        signalmap.TryAdd(cfg.ID, signal);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Create Signal Error:" + ex.ToString());
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
        /// 获得某个信号源设置
        /// </summary>
        /// <param name="signalID"></param>
        /// <returns></returns>
        public SignalConfig GetSignalConfig(int signalID)
        {
            SignalConfig cfg = null;
            if (configmap.TryGetValue(signalID, out cfg))
            {
                return cfg;
            }
            return null;
        }

        /// <summary>
        /// 获得所有信号配置信息
        /// </summary>
        public IEnumerable<SignalConfig> SignalConfigs
        {
            get
            {
                return configmap.Values;
            }
        }

        /// <summary>
        /// 将信号源添加到跟单策略
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="strategy"></param>
        public void AppendSignalToStrategy(int signalID, int  strategyID)
        {
            if (!strategysignalmap.Keys.Contains(strategyID))
            {
                strategysignalmap.TryAdd(strategyID, new ConcurrentDictionary<int, ISignal>());
            }

            ISignal signal = this[signalID];
            //信号存在且 映射中不包含该信号源
            if (signal != null && !strategysignalmap[strategyID].Keys.Contains(signalID))
            {
                //更新内存映射
                strategysignalmap[strategyID].TryAdd(signalID, signal);
                //跟新数据库
                ORM.MSignal.AppendSignalToStrategy(signalID, strategyID);
            }
        }

        /// <summary>
        /// 将信号源头从跟单策略中删除
        /// </summary>
        /// <param name="signalID"></param>
        /// <param name="strategyID"></param>
        public void RemoveSignalFromStrategy(int signalID, int strategyID)
        {
            if (!strategysignalmap.Keys.Contains(strategyID))
            {
                return;
            }

            ISignal signal = this[signalID];
            if (signal != null)
            {
                //跟新内存映射
                strategysignalmap[strategyID].TryRemove(signalID,out signal);
                //跟新数据库记录
                ORM.MSignal.RemoveSignalFromStrategy(signalID, strategyID);
            }
        }
        /// <summary>
        /// 获得所有信号对象
        /// </summary>
        public IEnumerable<ISignal> Signlas
        {
            get
            {
                return signalmap.Values;
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
