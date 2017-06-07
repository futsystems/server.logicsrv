using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 管理员
    /// </summary>
    public partial class MgrExchServer
    {
        /// <summary>
        /// 设定观察交易账户列表
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "WatchAgents", "WatchAgents - watch  agent", "设置当前观察代理", QSEnumArgParseType.Json)]
        public void CTE_WatchAgents(ISession session, string json)
        {
            string[] accounts = json.DeserializeObject<string[]>();
            var c = customerExInfoMap[session.Location.ClientID];
            c.WatchAgents(accounts);
        }


        /// <summary>
        /// 查询代理财务账户
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAgent", "QryAgent - query agent", "查询代理账户")]
        public void CTE_QryAgent(ISession session,string account)
        {
            Manager manager = session.GetManager();

            Manager target = BasicTracker.ManagerTracker[account];
            if (target == null)
            {
                throw new FutsRspError(string.Format("代理:{0} 不存在", account));
            }

            Agent agent = BasicTracker.AgentTracker[account];
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

            Agent agent = BasicTracker.AgentTracker[account];
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


    }
}
