using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Core
{
    public static partial class MangerUtils
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
        /// 获得某个Manager主域的待处理提现
        /// </summary>
        /// <param name="manger"></param>
        /// <returns></returns>
        public static decimal GetPendingWithdraw(this Manager manger)
        {
            int agentfk = manger.mgr_fk;
            return ORM.MAgentFinance.GetPendingWithdraw(agentfk);
        }

        /// <summary>
        /// 获得某个Manager主域的待处理充值
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static decimal GetPendingDeposit(this Manager manager)
        {
            int agentfk = manager.mgr_fk;
            return ORM.MAgentFinance.GetPendingDeposit(agentfk);
        }

        /// <summary>
        /// 获得未结算的提现金额
        /// 由于每个交易日必须结算，因此上个交易日内的充值提现已经通过结算计入当前Balance
        /// 而当前结算周期内的充值提现记录没有反应到Balance需要单独计算并显示
        /// </summary>
        /// <param name="manger"></param>
        /// <returns></returns>
        public static decimal GetWithdrawNotSettled(this Manager manger)
        {
            int agentfk = manger.mgr_fk;
            return ORM.MAgentFinance.GetWithdrawOfTradingDay(agentfk, TLCtxHelper.Ctx.SettleCentre.NextTradingday);
        }

        /// <summary>
        /// 获得未结算的充值金额
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static decimal GetDepositNotSettled(this Manager manager)
        {
            int agentfk = manager.mgr_fk;
            return ORM.MAgentFinance.GetDepositOfTradingDay(agentfk, TLCtxHelper.Ctx.SettleCentre.NextTradingday);
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
            info.PendingDeposit = GetPendingDeposit(manager);
            info.PendingWithDraw = GetPendingWithdraw(manager);
            info.CashIn = GetDepositNotSettled(manager);
            info.CashOut = GetWithdrawNotSettled(manager);
            return info;
        }

        /// <summary>
        /// 获得代理精简财务信息
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static JsonWrapperAgentFinanceInfoLite GetAgentFinanceInfoLite(this Manager manager)
        {
            JsonWrapperAgentFinanceInfoLite info = new JsonWrapperAgentFinanceInfoLite();
            info.BaseMGRFK = manager.mgr_fk;

            info.Balance = GetAgentBalance(manager);
            info.PendingDeposit = GetPendingDeposit(manager);
            info.PendingWithDraw = GetPendingWithdraw(manager);
            info.CashIn = GetDepositNotSettled(manager);
            info.CashOut = GetWithdrawNotSettled(manager);
            return info;
        }

        /// <summary>
        /// 获得某个manager的支付信息
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static JsonWrapperAgentPaymentInfo GetPaymentInfo(this Manager manager)
        {
            int agentfk = manager.mgr_fk;
            JsonWrapperAgentPaymentInfo info = new JsonWrapperAgentPaymentInfo();
            info.BaseMGRFK = agentfk;
            info.BankAccount = GetAgentBankAccount(manager);
            info.Mobile = manager.BaseManager.Mobile;
            info.QQ = manager.BaseManager.QQ;
            info.Name = manager.BaseManager.Name;

            return info;

        }


    }
}
