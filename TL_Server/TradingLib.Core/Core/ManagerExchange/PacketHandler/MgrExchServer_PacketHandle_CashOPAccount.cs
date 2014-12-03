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
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountPaymentInfo", "QryAccountPaymentInfo - query payment Info", "查询交易帐户支付信息")]
        public void CTE_QryAccountPaymentInfo(ISession session, string account)
        {
            IAccount acc = clearcentre[account];
            if (acc == null)
            { 
            }
            session.SendJsonReplyMgr(acc.GetBankAC());
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountCashOperationTotal", "QryAccountCashOperationTotal - query account pending cash operation", "查询所有交易帐户待处理委托")]
        public void CTE_QryAccountCashOperationTotal(ISession session)
        {
            debug("查询交易帐户所有出入金请求记录", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger != null)
            {

                JsonWrapperCashOperation[] ops = new JsonWrapperCashOperation[] { };
                IEnumerable<JsonWrapperCashOperation> list = manger.Domain.GetAccountCashOperation();
                if (manger.RightRootDomain())
                {
                    ops = list.ToArray();
                }
                else //如果不是root则过滤 代理商只能看到有权限的交易帐号的出入金请求
                { 
                    ops = list.Where(op=>manger.RightAccessAccount(clearcentre[op.Account])).ToArray();
                }
                
                session.SendJsonReplyMgr(ops);
            }
        }


        /// <summary>
        /// 确认出入金操作 
        /// 出入金操作确认后将会形成资金划转
        /// 形成确切的出入金记录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="playload"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ConfirmAccountCashOperation", "ConfirmCashOperation -confirm deposit or withdraw", "确认出入金操作请求", true)]
        public void CTE_ConfirmAccountCashOperation(ISession session, string playload)
        {
            debug("确认出入金操作请求", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger != null)
            {
                
                JsonWrapperCashOperation request = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                if (request != null)
                {
                    //如果是出金则需要进行资金检查
                    if (request.Operation == QSEnumCashOperation.WithDraw)
                    { 
                        
                    }
                    TLCtxHelper.CmdAuthCashOperation.ConfirmCashOperation(request.Ref);
                    //重新从数据库加载数据 返回当前记录的数据
                    request = ORM.MCashOpAccount.GetAccountCashOperation(request.Ref);
                    session.SendJsonReplyMgr(request);
                    //通过事件中继触发事件
                    TLCtxHelper.CashOperationEvent.FireCashOperation(this, QSEnumCashOpEventType.Confirm, request);
                }
            }
        }

        /// <summary>
        /// 取消交易帐户出入金操作
        /// </summary>
        /// <param name="session"></param>
        /// <param name="playload"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "CancelAccountCashOperation", "CancelCashOperation -cancel deposit or withdraw", "取消出入金操作请求", true)]
        public void CTE_CancelAccountCashOperation(ISession session, string playload)
        {
            debug("取消出入金操作请求", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCashOperation request = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                if (request != null)
                {
                    ORM.MCashOpAccount.CancelAccountCashOperation(request);
                    session.SendJsonReplyMgr(request);
                    //通过事件中继触发事件
                    TLCtxHelper.CashOperationEvent.FireCashOperation(this, QSEnumCashOpEventType.Cancel, request);
                }
            }
        }


        /// <summary>
        /// 拒绝帐户出入金操作
        /// </summary>
        /// <param name="session"></param>
        /// <param name="playload"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RejectAccountCashOperation", "RejectCashOperation -reject deposit or withdraw", "拒绝出入金操作请求", true)]
        public void CTE_RejectAccountCashOperation(ISession session, string playload)
        {
            debug("拒绝出入金操作请求", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCashOperation request = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                if (request != null)
                {
                    ORM.MCashOpAccount.RejectAccountCashOperation(request);
                    session.SendJsonReplyMgr(request);
                    //通过事件中继触发事件
                    TLCtxHelper.CashOperationEvent.FireCashOperation(this, QSEnumCashOpEventType.Reject, request);
                }
            }
        }

        /// <summary>
        /// 查询交易帐户的出入金记录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountCashTrans", "QueryAccountCashTrans -query account cashtrans", "查询交易帐户出入金记录")]
        public void CTE_QueryAccountCashTrans(ISession session, string account, long start, long end)
        {
            debug("查询出入金记录", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCasnTrans[] trans = ORM.MCashOpAccount.SelectAccountCashTrans(account, start, end).ToArray();
                session.SendJsonReplyMgr(trans);
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RequestAccountCashOperation", "RequestAccountCashOperation -rquest deposit or withdraw", "请求交易帐户出入金操作", true)]
        public void CTE_RequestAccountCashOperation(ISession session, string playload)
        {
            try
            {
                debug("管理员请求交易帐户出入金:" + playload, QSEnumDebugLevel.INFO);
                Manager manger = session.GetManager();
                if (manger != null)
                {
                    JsonWrapperCashOperation request = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                    if (request != null)
                    {
                        //request.mgr_fk = manger.mgr_fk;
                        //检查请求的交易帐号
                        if (string.IsNullOrEmpty(request.Account))
                        {
                            throw new FutsRspError("请设定交易帐号");
                        }
                        IAccount account = clearcentre[request.Account];
                        if (account == null)
                        {
                            throw new FutsRspError(string.Format("交易帐号[{0}]不存在", request.Account));
                        }
                        if (!manger.RightAccessAccount(account))
                        {
                            throw new FutsRspError(string.Format("无权操作交易帐户[{0}]", request.Account));
                        }

                        request.DateTime = Util.ToTLDateTime();
                        request.Ref = cashopref.AssignId.ToString();
                        request.Source = QSEnumCashOPSource.Manual;
                        request.Status = QSEnumCashInOutStatus.PENDING;

                        ORM.MCashOpAccount.InsertAccountCashOperation(request);

                        //通过事件中继触发事件
                        TLCtxHelper.CashOperationEvent.FireCashOperation(this, QSEnumCashOpEventType.Request, request);

                        session.OperationSuccess("提交交易帐户出入金成功");
                    }
                    //debug("update agent bank account: id:" + bankaccount.Bank.ID + " name:" + bankaccount.Bank.Name, QSEnumDebugLevel.INFO);
                }
            }
            catch (FutsRspError ex)//捕获到FutsRspError则向管理端发送对应回报
            {
                session.OperationError(ex);
            }
        }

    }
}
