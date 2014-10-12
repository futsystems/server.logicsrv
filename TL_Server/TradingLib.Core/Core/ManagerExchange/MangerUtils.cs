using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Core
{
    public static class MangerUtils
    {
        /// <summary>
        /// 获得某个Manager对应主域的权益信息
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static JsonWrapperAgentBalance GetAgentBalance(this Manager manager)
        {
            int agentfk = manager.mgr_fk;
            return ORM.MAgentFinance.GetAgentBalance(agentfk);
        }


        /// <summary>
        /// 获得某个代理某天的结算信息
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static JsonWrapperAgentSettle GetAgentSettlement(this Manager manager, int settleday)
        {
            int agentfk = manager.mgr_fk;
            return ORM.MAgentFinance.GetAgentSettle(agentfk, settleday);
        }


        /// <summary>
        /// 获得某个代理主域下的银行卡信息
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static JsonWrapperBankAccount GetAgentBankAccount(this Manager manager)
        {
            int agentfk = manager.mgr_fk;
            JsonWrapperBankAccount bank =  ORM.MAgentFinance.GetAgentBankAccount(agentfk);
            if(bank != null)
            {
                ContractBank cb = BasicTracker.ContractBankTracker[bank.bank_id];
                if(cb != null)
                {
                    bank.Bank = cb.ToJsonWrapperBank();
                }
            }
            return bank;
        }


        /// <summary>
        /// 获得主域下最近一个月的所有出入金操作
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static JsonWrapperCashOperation[] GetAgentLatestCashOperation(this Manager manager)
        {
            int agentfk = manager.mgr_fk;
            return ORM.MAgentFinance.GetAgentLatestCashOperation(agentfk).ToArray();
        }
        /// <summary>
        /// 获得某个Manager对应主域的财务信息
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static JsonWrapperAgentFinanceInfo GetAgentFinanceInfo(this Manager manager)
        {
            JsonWrapperAgentFinanceInfo info = new JsonWrapperAgentFinanceInfo();
            info.BaseMGRFK = manager.mgr_fk;

            info.Balance = GetAgentBalance(manager);
            info.BankAccount = GetAgentBankAccount(manager);
            if (info.Balance != null)
            {
                info.LastSettle = GetAgentSettlement(manager, info.Balance.Settleday);
            }
            info.LatestCashOperations = GetAgentLatestCashOperation(manager);
            return info;
        }
    }
}
