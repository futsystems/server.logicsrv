using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class AgentTracker
    {
        ConcurrentDictionary<int, Agent> agentIDMap = new ConcurrentDictionary<int, Agent>();
        ConcurrentDictionary<string, Agent> agentAccountMap = new ConcurrentDictionary<string, Agent>();

        public AgentTracker()
        {
            foreach (var agent in ORM.MAgent.SelectAgent())
            {
                agentIDMap.TryAdd(agent.ID, agent);
                agentAccountMap.TryAdd(agent.Account, agent);
            }
        }

        public Agent this[int id]
        {
            get
            {
                Agent target = null;
                if (agentIDMap.TryGetValue(id, out target)) return target;
                return null;
            }
        }

        public Agent this[string account]
        {
            get
            {
                Agent target = null;
                if (agentAccountMap.TryGetValue(account, out target)) return target;
                return null;
            }
        }

        /// <summary>
        /// 更新代理账户
        /// </summary>
        /// <param name="agent"></param>
        public void UpdateAgent(AgentSetting agent)
        {
            Agent target = null;
            if (agentIDMap.TryGetValue(agent.ID, out target))
            {
                target.Commission_ID = agent.Commission_ID;
                target.Margin_ID = agent.Margin_ID;
                target.ExStrategy_ID = agent.ExStrategy_ID;

                ORM.MAgent.UpdateAgentTemplate(target);
            }
            else
            {
                target = new Agent();
                target.Account = agent.Account;
                target.Currency = agent.Currency;
                target.AgentType = agent.AgentType;
                target.Commission_ID = agent.Commission_ID;
                target.Margin_ID = agent.Margin_ID;
                target.ExStrategy_ID = agent.ExStrategy_ID;

                ORM.MAgent.AddAgent(target);
                agentIDMap.TryAdd(target.ID, target);
                agentAccountMap.TryAdd(target.Account, target);
            }
        }
    }
}
