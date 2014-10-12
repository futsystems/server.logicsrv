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
            SendJsonReplyMgr(session, splist);
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFinanceInfo", "QryFinanceInfo - query agent finance", "查询代理财务信息")]
        public void CTE_QryFinanceInfo(ISession session)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperAgentFinanceInfo info = manger.GetAgentFinanceInfo();
                SendJsonReplyMgr(session, info);
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
                    SendJsonReplyMgr(session, bankaccount);
                }
                //debug("update agent bank account: id:" + bankaccount.Bank.ID + " name:" + bankaccount.Bank.Name, QSEnumDebugLevel.INFO);
                
            }

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RequestCashOperation", "RequestCashOperation -rquest deposit or withdraw", "请求出入金操作", true)]
        public void CTE_RequestCashOperation(ISession session, string playload)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCashOperation request = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                if (request != null)
                {
                    request.mgr_fk = manger.mgr_fk;
                    
                    request.DateTime = Util.ToTLDateTime();
                    request.Ref = "ref0000";

                    request.Status = QSEnumCashInOutStatus.PENDING;


                    ORM.MAgentFinance.InsertAgentCashOperation(request);

                    //bankaccount.Bank = BasicTracker.ContractBankTracker[bankaccount.bank_id].ToJsonWrapperBank();
                    SendJsonReplyMgr(session,request);
                }
                //debug("update agent bank account: id:" + bankaccount.Bank.ID + " name:" + bankaccount.Bank.Name, QSEnumDebugLevel.INFO);

            }
        }

    }
}
