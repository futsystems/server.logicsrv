using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class AgentUtils_object
    {
        public static AgentStatistic GetAgentStatistic(this IAgent agent)
        {
            AgentStatistic st = new AgentStatistic();
            st.Account = agent.Account;
            st.CommissioinIncome = agent.CommissionIncome;
            st.CommissionCost = agent.CommissionCost;
            st.CashIn = agent.CashIn;
            st.CashOut = agent.CashOut;
            st.CreditCashIn = agent.CreditCashIn;
            st.CreditCashOut = agent.CreditCashOut;

            st.RealizedPL = agent.RealizedPL;
            st.UnRealizedPL = agent.UnRealizedPL;
            st.NowEquity = agent.NowEquity;
            

            return st;
        }

        public static AgentFinanceInfo GetAgentFinanceInfo(this IAgent agent)
        {
            AgentFinanceInfo info = new AgentFinanceInfo();
            info.Account = agent.Account;
            info.Currency = agent.Currency;
            info.RealizedPL = agent.RealizedPL;
            info.UnRealizedPL = agent.UnRealizedPL;
            info.CommissioinIncome = agent.CommissionIncome;
            info.CommissionCost = agent.CommissionCost;
            info.Margin = 0;
            info.MarginFrozen = 0;

            info.LastEquity = agent.LastEquity;
            info.CashIn = agent.CashIn;
            info.CashOut = agent.CashOut;
            info.LastCredit = agent.LastCredit;
            info.CreditCashIn = agent.CreditCashIn;
            info.CreditCashOut = agent.CreditCashOut;

            info.NowEquity = agent.NowEquity;
            info.StaticEquity = agent.StaticEquity;
            info.SubStaticEquity = agent.SubStaticEquity;


            return info;
        }
    }
}
