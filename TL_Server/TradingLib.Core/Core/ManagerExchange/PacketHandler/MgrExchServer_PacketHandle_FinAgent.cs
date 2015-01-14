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
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFinanceInfo", "QryFinanceInfo - query agent finance", "查询代理财务信息")]
        public void CTE_QryFinanceInfo(ISession session)
        {
            Manager manger = session.GetManager();
            JsonWrapperAgentFinanceInfo info = manger.GetAgentFinanceInfo();
            session.ReplyMgr(info);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFinanceInfoLite", "QryFinanceInfoLite - query agent finance lite", "查询代精简理财务信息")]
        public void CTE_QryFinanceInfoLite(ISession session)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperAgentFinanceInfoLite info = manger.GetAgentFinanceInfoLite();
                session.ReplyMgr(info);
            }
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAgentBankAccount", "UpdateAgentBankAccount -update bankaccount of agent", "更新代理银行卡信息", QSEnumArgParseType.Json)]
        public void CTE_UpdateAgentBankAccount(ISession session, string playload)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperBankAccount bankaccount = Mixins.Json.JsonMapper.ToObject<JsonWrapperBankAccount>(playload);
                //强制设定银行帐号的主域id为当前manger主域id
                bankaccount.mgr_fk = manger.mgr_fk;
                if (bankaccount != null && bankaccount.mgr_fk == manger.mgr_fk)
                {
                    ORM.MAgentFinance.UpdateAgentBankAccount(bankaccount);
                    bankaccount.Bank = BasicTracker.ContractBankTracker[bankaccount.bank_id].ToJsonWrapperBank();
                    session.ReplyMgr(bankaccount);
                }
            }

        }

    }
}
