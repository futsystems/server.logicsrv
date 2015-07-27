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
