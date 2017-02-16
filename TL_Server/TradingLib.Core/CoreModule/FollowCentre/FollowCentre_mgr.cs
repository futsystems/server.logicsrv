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
        /// 查询可用策略帐户列表
        /// 已经被策略绑定的策略帐户无法再次绑定到某个策略
        /// 策略帐户是用于策略运行时下单
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAvabileStrategyAccount", "QryAvabileStrategyAccount - qry  avabile strategy account list", "查询可用策略帐户列表")]
        public void CTE_QryAvabileStrategyAccount(ISession session)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }

            IEnumerable<IAccount> acclist = TLCtxHelper.ModuleAccountManager.Accounts.Where(acc => acc.Category == QSEnumAccountCategory.STRATEGYACCOUNT);

            IEnumerable<IAccount> avabile = acclist.Where(acc => !FollowTracker.StrategyCfgTracker.StrategyConfigs.Any(cfg => cfg.Account == acc.ID));
            string[] accounts = avabile.Select(acc=>acc.ID).ToArray();
            //for (int i = 0; i < accounts.Length; i++)
            {
                session.ReplyMgr(accounts);
            }
        }


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

            FollowStrategyConfig cfg = payload.DeserializeObject<FollowStrategyConfig>();
            if (cfg != null)
            {

                bool isadd = cfg.ID == 0;
                if (isadd)
                {
                    //新增判断token重复
                    if (FollowTracker.StrategyCfgTracker[cfg.ID] != null)
                    {
                        throw new FutsRspError("跟单策略标识:" + cfg.Token + "已存在");
                    }
                    //添加跟单策略
                    FollowTracker.StrategyCfgTracker.UpdateFollowStrategyConfig(cfg);
                    //初始化跟单策略
                    InitStrategy(FollowTracker.StrategyCfgTracker[cfg.ID]);
                    //启动跟单策略
                    FollowStrategy strategy = ID2FollowStrategy(cfg.ID);
                    strategy.Start();

                    NotifyFollowStrategyConfig(FollowTracker.StrategyCfgTracker[cfg.ID]);
                    session.RspMessage("添加跟单策略成功");
                    
                }
                else
                {

                    FollowTracker.StrategyCfgTracker.UpdateFollowStrategyConfig(cfg);

                    NotifyFollowStrategyConfig(FollowTracker.StrategyCfgTracker[cfg.ID]);
                    session.RspMessage("更新跟单策略参数成功");
                }
            }
        }

        /// <summary>
        /// 设置策略工作状态
        /// </summary>
        /// <param name="session"></param>
        /// <param name="strategyid"></param>
        /// <param name="state"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "SetStrategyWorkState", "SetStrategyWorkState - set follow strategy workstate", "设置跟单策略工作状态")]
        public void CTE_SetWorkState(ISession session,int strategyid,QSEnumFollowWorkState state)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }

            FollowStrategy strategy = ID2FollowStrategy(strategyid);
            if (strategy == null)
            {
                throw new FutsRspError(string.Format("策略:{0}不存在", strategyid));
            }

            strategy.WorkState = state;
            session.RspMessage(string.Format("策略:{0}-{1}工作状态:{2}", strategy.Config.ID, strategy.Config.Token, Util.GetEnumDescription(strategy.WorkState)));
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

        /// <summary>
        /// 将信号源从策略移除
        /// </summary>
        /// <param name="session"></param>
        /// <param name="signalID"></param>
        /// <param name="strategyID"></param>
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
        /// 查询某个跟单策略的开仓跟单项目列表
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

        /// <summary>
        /// 查询某个跟单策略的平仓跟单项目列表
        /// </summary>
        /// <param name="session"></param>
        /// <param name="strategy_id"></param>
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

        /// <summary>
        /// 查询跟单项目明细
        /// </summary>
        /// <param name="session"></param>
        /// <param name="followkey"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFollowItemDetail", "QryFollowItemDetail - qry follow item detail", "查询跟单项目明细信息",QSEnumArgParseType.Json)]
        public void CTE_QryFollowItemDetail(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            var data = json.DeserializeObject();
            int strategyId = int.Parse(data["StrategyID"].ToString());
            string followkey = data["FollowKey"].ToString();

            FollowStrategy strategy = null;
            if (!strategyMap.TryGetValue(strategyId, out strategy))
            {
                throw new FutsRspError(string.Format("跟单策略:{0} 不存在", strategyId));
            }

            TradeFollowItem item = strategy.GetFollowItem(followkey);
            if (item == null)
            {
                throw new FutsRspError(string.Format("跟单项:{0} 不存在", followkey));
            }

            FollowItemDetail detail = item.GenFollowItemDetail();
            session.ReplyMgr(detail);

            
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "FlatFollowItem", "FlatFollowItem - flat follow item", "强平某个跟单项目", QSEnumArgParseType.Json)]
        public void CTE_FlatFollowItem(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            var data = json.DeserializeObject();
            int strategyId = int.Parse(data["StrategyID"].ToString());
            string followkey = data["FollowKey"].ToString();

            FollowStrategy strategy = null;
            if (!strategyMap.TryGetValue(strategyId, out strategy))
            {
                throw new FutsRspError(string.Format("跟单策略:{0} 不存在", strategyId));
            }

            TradeFollowItem item = strategy.GetFollowItem(followkey);
            if (item == null)
            {
                throw new FutsRspError(string.Format("跟单项:{0} 不存在", followkey));
            }
            //如果提供的是平仓跟单项followkey则找到对应的开仓跟单项目
            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                item = item.EntryFollowItem;
            }

            if (!item.NeedExitFollow)
            {
                throw new FutsRspError("跟单项目无需平仓");
            }

            //执行手工平仓操作
            TradeFollowItem exit = TradeFollowItem.CreateFlatFollowItem(item);
            item.Link(exit);

            item.Strategy.NewFollowItem(exit);

           
        }
    }
}
