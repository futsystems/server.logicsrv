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
        /// 获得跟单策略列表
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFollowStrategyList", "QryFollowStrategyList - qry follow strategy list", "查询跟单策略列表")]
        public void CTE_QryFollowStrategyList(ISession session)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }

            FollowStrategyConfig[] cfgs = FollowTracker.StrategyCfgTracker.StrategyConfigs.ToArray();
            for (int i = 0; i < cfgs.Length; i++)
            {
                session.ReplyMgr(cfgs[i], i == cfgs.Length - 1);
            }
        }

        /// <summary>
        /// 更新跟单策略设置
        /// </summary>
        /// <param name="session"></param>
        /// <param name="payload"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateFollowStrategyCfg", "UpdateFollowStrategyCfg - update config of follow strategy", "更新跟单策略参数",QSEnumArgParseType.Json)]
        public void CTE_QryFollowStrategyList(ISession session,string payload)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }

            FollowStrategyConfig cfg = TradingLib.Mixins.Json.JsonMapper.ToObject<FollowStrategyConfig>(payload);
            if (cfg != null)
            {

                //新增判断token重复
                if (cfg.ID==0 && FollowTracker.StrategyCfgTracker[cfg.Token] != null)
                {
                    throw new FutsRspError("跟单策略标识:" + cfg.Token + "已存在");
                }
                FollowTracker.StrategyCfgTracker.UpdateFollowStrategyConfig(cfg);

                session.OperationSuccess("更新跟单策略参数成功");
            }
        }


        /// <summary>
        /// 查询所有信号设置列表
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QrySignalConfigList", "QrySignalConfigList - qry signal config list", "查询信号配置列表")]
        public void CTE_QrySignalConfigList(ISession session)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }

            SignalConfig[] cfgs = FollowTracker.SignalTracker.SignalConfigs.ToArray();
            for (int i = 0; i < cfgs.Length; i++)
            {
                session.ReplyMgr(cfgs[i], i == cfgs.Length - 1);
            }
            if (cfgs.Length == 0)
            {
                session.ReplyMgr(null, true);
            }
        }

        /// <summary>
        /// 查询跟单策略信号列表
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryStrategySignalList", "QryStrategySignalList - qry strategy signal list", "查询跟单策略信号列表")]
        public void CTE_QryStrategySignalList(ISession session,int strategyID)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }

            SignalConfig[] cfgs = FollowTracker.SignalTracker.GetStrategySignals(strategyID).Values.Select(sig=>FollowTracker.SignalTracker.GetSignalConfig(sig.ID)).ToArray();
            for (int i = 0; i < cfgs.Length; i++)
            {
                session.ReplyMgr(cfgs[i], i == cfgs.Length - 1);
            }
            if (cfgs.Length == 0)
            {
                session.ReplyMgr(null,true);
            }
        }

        /// <summary>
        /// 将信号源添加到跟单策略
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AppendSignalToStrategy", "AppendSignalToStrategy - add signal into strategy's signal list", "将信号源添加到跟单策略信号列表")]
        public void CTE_AppendSignalToStrategy(ISession session,int signalID,int strategyID)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }

            ISignal signal = FollowTracker.SignalTracker[signalID];
            if (signal == null)
            {
                throw new FutsRspError(string.Format("信号源:{0}不存在", signalID));
            }

            FollowStrategy strategy = ID2FollowStrategy(strategyID);
            if (strategy == null)
            {
                throw new FutsRspError(string.Format("跟单策略:{0}不存在", strategyID));
            }

            //更新信号维护器
            FollowTracker.SignalTracker.AppendSignalToStrategy(signalID, strategyID);

            //跟单策略添加信号
            strategy.AppendSignal(signal);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RemoveSignalFromStrategy", "RemoveSignalFromStrategy - remove signal from strategy's signal list", "将信号源从跟单策略信号列表删除")]
        public void CTE_RemoveSignalFromStrategy(ISession session, int signalID, int strategyID)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }

            ISignal signal = FollowTracker.SignalTracker[signalID];
            if (signal == null)
            {
                throw new FutsRspError(string.Format("信号源:{0}不存在", signalID));
            }

            FollowStrategy strategy = ID2FollowStrategy(strategyID);
            if (strategy == null)
            {
                throw new FutsRspError(string.Format("跟单策略:{0}不存在", strategyID));
            }

            //更新信号维护器
            FollowTracker.SignalTracker.RemoveSignalFromStrategy(signalID, strategyID);

            //跟单策略删除信号
            strategy.RemoveSignal(signal);
        }



        /// <summary>
        /// 查询某个跟单策略的跟单项目列表
        /// </summary>
        /// <param name="session"></param>
        /// <param name="strategy_id"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryEntryFollowItemList", "QryEntryFollowItemList - qry follow item list", "查询跟单项目列表")]
        public void CTE_QryEntryFollowItemList(ISession session, int strategy_id)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            FollowStrategy strategy = null;
            //查找对应的策略对象
            if (strategyMap.TryGetValue(strategy_id, out strategy))
            {

                EntryFollowItemStruct[] items = strategy.GetEntryFollowItemStructs().ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    session.ReplyMgr(items[i], i == items.Length - 1);
                }
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryExitFollowItemList", "QryExitFollowItemList - qry follow item list", "查询跟单项目列表")]
        public void CTE_QryExitFollowItemList(ISession session, int strategy_id)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            FollowStrategy strategy = null;
            //查找对应的策略对象
            if (strategyMap.TryGetValue(strategy_id, out strategy))
            {

                ExitFollowItemStruct[] items = strategy.GetExitFollowItemStructs().ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    session.ReplyMgr(items[i], i == items.Length - 1);
                }
            }
        }

    }
}
