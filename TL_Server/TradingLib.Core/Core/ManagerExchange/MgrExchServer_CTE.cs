using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryBank", "QryBank - query bank", "查询银行列表")]
        public void CTE_QryBank(ISession session)
        {
            JsonWrapperBank[] splist = BasicTracker.ContractBankTracker.Banks.Select(b => b.ToJsonWrapperBank()).ToArray();
            session.SendJsonReplyMgr(splist);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAgentPaymentInfo", "QryAgentPaymentInfo - query payment Info", "查询代理支付信息")]
        public void CTE_QryPaymentInfo(ISession session,int agentfk)
        {
            Manager manger = BasicTracker.ManagerTracker[agentfk];
            JsonWrapperAgentPaymentInfo info = manger.GetPaymentInfo();
            session.SendJsonReplyMgr(info);
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFinanceInfo", "QryFinanceInfo - query agent finance", "查询代理财务信息")]
        public void CTE_QryFinanceInfo(ISession session)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperAgentFinanceInfo info = manger.GetAgentFinanceInfo();
                session.SendJsonReplyMgr(info);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFinanceInfoLite", "QryFinanceInfoLite - query agent finance lite", "查询代精简理财务信息")]
        public void CTE_QryFinanceInfoLite(ISession session)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperAgentFinanceInfoLite info = manger.GetAgentFinanceInfoLite();
                session.SendJsonReplyMgr(info);
            }
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAgentBankAccount", "UpdateAgentBankAccount -update bankaccount of agent", "更新代理银行卡信息",true)]
        public void CTE_UpdateAgentBankAccount(ISession session, string playload)
        { 
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperBankAccount bankaccount = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperBankAccount>(playload);
                //强制设定银行帐号的主域id为当前manger主域id
                bankaccount.mgr_fk = manger.mgr_fk;
                if (bankaccount != null && bankaccount.mgr_fk==manger.mgr_fk)
                {
                    ORM.MAgentFinance.UpdateAgentBankAccount(bankaccount);
                    bankaccount.Bank = BasicTracker.ContractBankTracker[bankaccount.bank_id].ToJsonWrapperBank();
                    session.SendJsonReplyMgr(bankaccount);
                }
                //debug("update agent bank account: id:" + bankaccount.Bank.ID + " name:" + bankaccount.Bank.Name, QSEnumDebugLevel.INFO);
                
            }

        }





    }
}
