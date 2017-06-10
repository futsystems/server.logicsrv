﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;

namespace TradingLib.Core
{
   
    public partial class AgentManager
    {

        /// <summary>
        /// 查询代理财务账户
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAgent", "QryAgent - query agent", "查询代理账户")]
        public void CTE_QryAgent(ISession session, string account)
        {
            Manager manager = session.GetManager();

            Manager target = BasicTracker.ManagerTracker[account];
            if (target == null)
            {
                throw new FutsRspError(string.Format("代理:{0} 不存在", account));
            }

            IAgent agent = BasicTracker.AgentTracker[account];
            if (agent == null)
            {
                throw new FutsRspError(string.Format("代理财务账户:{0} 不存在", account));
            }
            if (!manager.RightAccessManager(target))
            {
                throw new FutsRspError(string.Format("无权查看代理:{0} 不存在", account));
            }

            session.ReplyMgr(agent);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAllAgent", "QryAllAgent - query all agent", "查询所有代理账户")]
        public void CTE_QryAgent(ISession session)
        {
            Manager manager = session.GetManager();

            List<AgentSetting> agentlist = new List<AgentSetting>();
            foreach (var mgr in manager.GetVisibleManager())
            {
                if (mgr.AgentAccount == null) continue;
                var agent = mgr.AgentAccount as AgentSetting;
                if (agent == null) continue;
                agentlist.Add(agent);
            }
            session.ReplyMgr(agentlist.ToArray());
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAgentTemplate", "UpdateAgentTemplate - update agent template", "更新代理账户手续费模板", QSEnumArgParseType.Json)]
        public void CTE_UpdateAgentTemplate(ISession session, string json)
        {
            Manager manager = session.GetManager();
            var data = json.DeserializeObject();
            var account = data["account"].ToString();

            Manager target = BasicTracker.ManagerTracker[account];
            if (target == null)
            {
                throw new FutsRspError(string.Format("代理:{0} 不存在", account));
            }

            AgentImpl agent = BasicTracker.AgentTracker[account];
            if (agent == null)
            {
                throw new FutsRspError(string.Format("代理财务账户:{0} 不存在", account));
            }
            if (!manager.RightAccessManager(target))
            {
                throw new FutsRspError(string.Format("无权查看代理:{0} 不存在", account));
            }

            agent.Commission_ID = int.Parse(data["commission_id"].ToString());
            agent.Margin_ID = int.Parse(data["margin_id"].ToString());
            agent.ExStrategy_ID = int.Parse(data["exstrategy_id"].ToString());

            ORM.MAgent.UpdateAgentTemplate(agent);

            session.NotifyMgr("NotifyAgent", agent);

            session.RspMessage("更新模板成功");

        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAgentFinanceInfo", "QryAgentFinanceInfo - qry agent finance info", "查询代理财务信息")]
        public void CTE_QryAgentFinanceInfo(ISession session, string account)
        {
            Manager manager = session.GetManager();

            Manager target = BasicTracker.ManagerTracker[account];
            if (target == null)
            {
                throw new FutsRspError(string.Format("代理:{0} 不存在", account));
            }

            AgentImpl agent = BasicTracker.AgentTracker[account];
            if (agent == null)
            {
                throw new FutsRspError(string.Format("代理财务账户:{0} 不存在", account));
            }
            if (!manager.RightAccessManager(target))
            {
                throw new FutsRspError(string.Format("无权查看代理:{0} 不存在", account));
            }

            session.ReplyMgr(agent.GetAgentFinanceInfo());

        }

        /// <summary>
        /// 交易账户 出入金
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AgentCashOperation", "AgentCashOperation - agent cash operation", "代理帐户出入去金", QSEnumArgParseType.Json)]
        public void CTE_CashOperation(ISession session, string json)
        {
            Manager manager = session.GetManager();
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var amount = decimal.Parse(req["amount"].ToString());
            var txnref = req["txnref"].ToString();
            var comment = req["comment"].ToString();
            var equity_type = Util.ParseEnum<QSEnumEquityType>(req["equity_type"].ToString());

            IAgent agent = BasicTracker.AgentTracker[account];
            if (agent == null)
            {
                throw new FutsRspError(string.Format("代理账户:{0}不存在", account));
            }

            Manager manger = session.GetManager();

            if (!manager.RightAccessManager(account))
            {
                throw new FutsRspError("无权操作该代理");
            }

            CashTransaction txn = new CashTransactionImpl();
            txn.Account = account;
            txn.Amount = Math.Abs(amount);
            txn.Comment = comment;
            txn.DateTime = Util.ToTLDateTime();
            txn.EquityType = equity_type;
            txn.Operator = manager.Login;
            txn.Settled = false;
            txn.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            txn.TxnRef = txnref;
            txn.TxnType = amount > 0 ? QSEnumCashOperation.Deposit : QSEnumCashOperation.WithDraw;


            //执行出入金操作
            this.CashOperation(txn);

            //出入金操作后返回帐户信息更新
            session.NotifyMgr("NotifyAgentFinInfo",agent.GetAgentFinanceInfo());
            session.RspMessage("代理出入金操作成功");
        }





        #region 历史记录查询
        /// <summary>
        /// 查询交易帐户的出入金记录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAgentCashTxn", "QueryAgentCashTxn -query agent cashtrans", "查询代理帐户出入金记录", QSEnumArgParseType.Json)]
        public void CTE_QueryAgentCashTrans(ISession session, string json)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                var data = json.DeserializeObject();
                string account = data["account"].ToString();
                long start = long.Parse(data["start"].ToString());
                long end = long.Parse(data["end"].ToString());

                CashTransactionImpl[] trans = ORM.MAgentCashTransaction.SelectHistCashTransactions(account, start, end).ToArray();
                session.ReplyMgrArray(trans);
            }
        }

        #endregion

    }
}
