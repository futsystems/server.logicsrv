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

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryPermmissionTemplateList", "QueryPermmissionTemplateList - Query Permmission lsit", "查询权限模板列表")]
        public void CTE_QueryPermissionTemplateList(ISession session)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    session.SendJsonReplyMgr(UIAccessTracker.GetUIAccessList().ToArray());
                }
                else
                {
                    throw new FutsRspError("无权查询权限模板列表");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        /// <summary>
        /// 更新权限模板
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdatePermission", "UpdatePermission - update permission config", "更新权限模板",true)]
        public void CTE_UpdatePermissionTemplateList(ISession session,string playload)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    UIAccess access = Mixins.LitJson.JsonMapper.ToObject<UIAccess>(playload);
                    //session.SendJsonReplyMgr(UIAccessTracker.GetUIAccessList().ToArray());
                    UIAccessTracker.UpdateUIAccess(access);//更新
                }
                else
                {
                    throw new FutsRspError("无权查询权限模板列表");
                }

            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAgentPermission", "QueryAgentPermission - query agent permission", "查询某个代理的权限设置")]
        public void CTE_QueryAgentPermission(ISession session, int managerid)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {

                    UIAccess access = UIAccessTracker.GetAgentUIAccess(managerid);
                    session.SendJsonReplyMgr(access);
                }
                else
                {
                    throw new FutsRspError("无权查询代理权限设置");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAgentPermission", "UpdateAgentPermission - updaet agent permission", "更新管理员的权限设置")]
        public void CTE_UpdateAgentPermission(ISession session,int managerid,int accessid)
        {
             try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    Manager m = BasicTracker.ManagerTracker[managerid];
                    if (m == null)
                    {
                        throw new FutsRspError("指定的管理员不存在");
                    }
                    UIAccessTracker.UpdateAgentPermission(managerid, accessid);
                    
                }
                else
                {
                    throw new FutsRspError("无权更新代理权限设置");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }




    }
}
