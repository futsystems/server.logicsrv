using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public static class AgentUtils_object
    {
        public static AgentStatistic GetAgentStatistic(this Agent agent)
        {
            AgentStatistic st = new AgentStatistic();
            st.Account = agent.Account;
            st.CommissioinIncome = agent.CommissionIncome;
            st.CommissionCost = agent.CommissionCost;

            return st;
        }
    }
}
