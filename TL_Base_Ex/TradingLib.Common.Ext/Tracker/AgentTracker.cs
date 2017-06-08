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
        ConcurrentDictionary<int, AgentImpl> agentIDMap = new ConcurrentDictionary<int, AgentImpl>();
        ConcurrentDictionary<string, AgentImpl> agentAccountMap = new ConcurrentDictionary<string, AgentImpl>();

        public AgentTracker()
        {
            foreach (var agent in ORM.MAgent.SelectAgent())
            {
                agentIDMap.TryAdd(agent.ID, agent);
                agentAccountMap.TryAdd(agent.Account, agent);
            }
        }

        public AgentImpl this[int id]
        {
            get
            {
                AgentImpl target = null;
                if (agentIDMap.TryGetValue(id, out target)) return target;
                return null;
            }
        }

        public AgentImpl this[string account]
        {
            get
            {
                AgentImpl target = null;
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
            AgentImpl target = null;
            if (agentIDMap.TryGetValue(agent.ID, out target))
            {
                target.Commission_ID = agent.Commission_ID;
                target.Margin_ID = agent.Margin_ID;
                target.ExStrategy_ID = agent.ExStrategy_ID;

                ORM.MAgent.UpdateAgentTemplate(target);
            }
            else
            {
                target = new AgentImpl();
                target.Account = agent.Account;
                target.Currency = agent.Currency;
                target.AgentType = agent.AgentType;
                target.Commission_ID = agent.Commission_ID;
                target.Margin_ID = agent.Margin_ID;
                target.ExStrategy_ID = agent.ExStrategy_ID;

                ORM.MAgent.AddAgent(target);
                agent.ID = target.ID;

                agentIDMap.TryAdd(target.ID, target);
                agentAccountMap.TryAdd(target.Account, target);
            }
        }
    }
}
