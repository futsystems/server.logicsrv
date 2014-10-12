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

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAgentCashOperationTotal", "QryAgentCashOperationTotal - query agent pending cash operation", "查询所有代理待处理委托")]
        public void CTE_QryAgentCashOperationTotal(ISession session)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCashOperation[] ops = ORM.MAgentFinance.GetAgentLatestCashOperationTotal().ToArray();
                session.SendJsonReplyMgr(ops);
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


        IdTracker cashopref = new IdTracker();
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
                    request.Ref = cashopref.AssignId.ToString();

                    request.Status = QSEnumCashInOutStatus.PENDING;


                    ORM.MAgentFinance.InsertAgentCashOperation(request);

                    //bankaccount.Bank = BasicTracker.ContractBankTracker[bankaccount.bank_id].ToJsonWrapperBank();
                    session.SendJsonReplyMgr(request);
                }
                //debug("update agent bank account: id:" + bankaccount.Bank.ID + " name:" + bankaccount.Bank.Name, QSEnumDebugLevel.INFO);

            }
        }

        /// <summary>
        /// 确认出入金操作 
        /// 出入金操作确认后将会形成资金划转
        /// 形成确切的出入金记录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="playload"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ConfirmCashOperation", "ConfirmCashOperation -confirm deposit or withdraw", "确认出入金操作请求", true)]
        public void CTE_ConfirmCashOperation(ISession session, string playload)
        {
            debug("确认出入金操作请求", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCashOperation request = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                if (request != null)
                {
                    request.Status = QSEnumCashInOutStatus.CONFIRMED;
                    ORM.MAgentFinance.ConfirmAgentCashOperation(request);
                    session.SendJsonReplyMgr(request);
                }
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "CancelCashOperation", "CancelCashOperation -cancel deposit or withdraw", "取消出入金操作请求", true)]
        public void CTE_CancelCashOperation(ISession session, string playload)
        {
            debug("取消出入金操作请求", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCashOperation request = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                if (request != null)
                {
                    ORM.MAgentFinance.CancelAgentCashOperation(request);
                    session.SendJsonReplyMgr(request);
                }
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RejectCashOperation", "RejectCashOperation -reject deposit or withdraw", "拒绝出入金操作请求", true)]
        public void CTE_RejectCashOperation(ISession session, string playload)
        {
            debug("拒绝出入金操作请求", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCashOperation request = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                if (request != null)
                {
                    ORM.MAgentFinance.RejectAgentCashOperation(request);
                    session.SendJsonReplyMgr(request);
                }
            }
        }


    }
}
