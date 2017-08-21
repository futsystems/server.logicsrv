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
            st.LastEquity = agent.LastEquity;
            st.LastCredit = agent.LastCredit;
            st.NowEquity = agent.NowEquity;
            st.NowCredit = agent.NowCredit;
            st.CommissioinIncome = agent.CommissionIncome;
            st.CommissionCost = agent.CommissionCost;

            st.StaticEquity = agent.StaticEquity;
            st.SubStaticEquity = agent.SubStaticEquity;
            st.FlatEquity = agent.FlatEquity;
            st.Freezed = agent.Freezed;


            st.CustMargin = agent.CustMargin;
            st.CustForzenMargin = agent.CustForzenMargin;
            st.CustRealizedPL = agent.CustRealizedPL;
            st.CustUnRealizedPL = agent.CustUnRealizedPL;

            st.CustCashIn = agent.CustCashIn;
            st.CustCashOut = agent.CustCashOut;
            st.CustLongPositionSize = agent.CustLongPositionSize;
            st.CustShortPositionSize = agent.CustShortPositionSize;


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
