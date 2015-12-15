﻿using System;
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
        //[ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountPaymentInfo", "QryAccountPaymentInfo - query payment Info", "查询交易帐户支付信息")]
        //public void CTE_QryAccountPaymentInfo(ISession session, string account)
        //{
        //    IAccount acc = TLCtxHelper.ModuleAccountManager[account];
        //    if (acc == null)
        //    { 
        //    }
        //    session.ReplyMgr(acc.GetBankAC());
        //}


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountCashOperationTotal", "QryAccountCashOperationTotal - query account pending cash operation", "查询所有交易帐户待处理委托")]
        public void CTE_QryAccountCashOperationTotal(ISession session)
        {
            logger.Info("查询交易帐户所有出入金请求记录");
            Manager manger = session.GetManager();
            if (manger != null)
            {

                JsonWrapperCashOperation[] ops = new JsonWrapperCashOperation[] { };
                IEnumerable<JsonWrapperCashOperation> list = manger.Domain.GetAccountCashOperation();
                if (manger.IsInRoot())
                {
                    ops = list.ToArray();
                }
                else //如果不是root则过滤 代理商只能看到有权限的交易帐号的出入金请求
                {
                    ops = list.Where(op => manger.RightAccessAccount(TLCtxHelper.ModuleAccountManager[op.Account])).ToArray();
                }

                session.ReplyMgr(ops);
            }
        }


        /// <summary>
        /// 确认出入金操作 
        /// 出入金操作确认后将会形成资金划转
        /// 形成确切的出入金记录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="playload"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ConfirmAccountCashOperation", "ConfirmCashOperation -confirm deposit or withdraw", "确认出入金操作请求", QSEnumArgParseType.Json)]
        public void CTE_ConfirmAccountCashOperation(ISession session, string playload)
        {
            try
            {
                logger.Info("确认出入金操作请求");
                Manager manger = session.GetManager();
                if (!manger.IsRoot())
                {
                    throw new FutsRspError("无权确认出入金请求");
                }

                JsonWrapperCashOperation request = Mixins.Json.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                
                if (request != null)
                {
                    IAccount account = TLCtxHelper.ModuleAccountManager[request.Account];
                    if (!manger.RightAccessAccount(account))
                    {
                        throw new FutsRspError("无权访问帐户:" + account.ID);
                    }

                    request = ORM.MCashOpAccount.GetAccountCashOperation(request.Ref);
                    if (request.Status != QSEnumCashInOutStatus.PENDING)
                    {
                        throw new FutsRspError("出入金请求已经关闭");
                    }

                    //调用清算中心出入金确认操作
                    //TLCtxHelper.CmdAuthCashOperation.ConfirmCashOperation(request.Ref);

                    //重新从数据库加载数据 返回当前记录的数据
                    request = ORM.MCashOpAccount.GetAccountCashOperation(request.Ref);
                    session.ReplyMgr(request);
                    //通过事件中继触发事件
                    TLCtxHelper.EventSystem.FireCashOperation(this, QSEnumCashOpEventType.Confirm, request);
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        /// <summary>
        /// 取消交易帐户出入金操作
        /// </summary>
        /// <param name="session"></param>
        /// <param name="playload"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "CancelAccountCashOperation", "CancelCashOperation -cancel deposit or withdraw", "取消出入金操作请求", QSEnumArgParseType.Json)]
        public void CTE_CancelAccountCashOperation(ISession session, string playload)
        {
            logger.Info("取消出入金操作请求");
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCashOperation request = Mixins.Json.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                if (request != null)
                {
                    request = ORM.MCashOpAccount.GetAccountCashOperation(request.Ref);
                    if (request.Status != QSEnumCashInOutStatus.PENDING)
                    {
                        throw new FutsRspError("出入金请求已经关闭");
                    }

                    ORM.MCashOpAccount.CancelAccountCashOperation(request);
                    session.ReplyMgr(request);
                    //通过事件中继触发事件
                    TLCtxHelper.EventSystem.FireCashOperation(this, QSEnumCashOpEventType.Cancel, request);
                }
            }
        }


        /// <summary>
        /// 拒绝帐户出入金操作
        /// </summary>
        /// <param name="session"></param>
        /// <param name="playload"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RejectAccountCashOperation", "RejectCashOperation -reject deposit or withdraw", "拒绝出入金操作请求", QSEnumArgParseType.Json)]
        public void CTE_RejectAccountCashOperation(ISession session, string playload)
        {
            logger.Info("拒绝出入金操作请求");
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCashOperation request = Mixins.Json.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                if (request != null)
                {
                    request = ORM.MCashOpAccount.GetAccountCashOperation(request.Ref);
                    if (request.Status != QSEnumCashInOutStatus.PENDING)
                    {
                        throw new FutsRspError("出入金请求已经关闭");
                    }

                    ORM.MCashOpAccount.RejectAccountCashOperation(request);
                    session.ReplyMgr(request);
                    //通过事件中继触发事件
                    TLCtxHelper.EventSystem.FireCashOperation(this, QSEnumCashOpEventType.Reject, request);
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
            logger.Info("查询出入金记录: start:" + start.ToString() + " end:" + end.ToString());
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCasnTrans[] trans = ORM.MCashOpAccount.SelectAccountCashTrans(account, start, end).ToArray();
                session.ReplyMgr(trans);
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RequestAccountCashOperation", "RequestAccountCashOperation -rquest deposit or withdraw", "请求交易帐户出入金操作", QSEnumArgParseType.Json)]
        public void CTE_RequestAccountCashOperation(ISession session, string playload)
        {
            try
            {
                logger.Info("管理员请求交易帐户出入金:" + playload);
                Manager manger = session.GetManager();
                if (manger != null)
                {
                    JsonWrapperCashOperation request = Mixins.Json.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                    if (request != null)
                    {
                        //request.mgr_fk = manger.mgr_fk;
                        //检查请求的交易帐号
                        if (string.IsNullOrEmpty(request.Account))
                        {
                            throw new FutsRspError("请设定交易帐号");
                        }
                        IAccount account = TLCtxHelper.ModuleAccountManager[request.Account];
                        if (account == null)
                        {
                            throw new FutsRspError(string.Format("交易帐号[{0}]不存在", request.Account));
                        }
                        if (!manger.RightAccessAccount(account))
                        {
                            throw new FutsRspError(string.Format("无权操作交易帐户[{0}]", request.Account));
                        }

                        UIAccess fatheraccess, access = null;
                        //如果是代理则需要检查父代理权限设置中的 子代理提交出入金权限和代理本身的提交出入金权限
                        if (manger.IsAgent())
                        {
                            //fatheraccess = BasicTracker.UIAccessTracker.GetUIAccess(manger.ParentManager);
                            //if (!fatheraccess.r_cashop_subagent)
                            //{
                            //    throw new FutsRspError("无权提交出入金");
                            //}

                            access = BasicTracker.UIAccessTracker.GetUIAccess(manger);

                            if (!access.r_cashop)
                                throw new FutsRspError("无权提交出入金");
                        }
                        

                        request.DateTime = Util.ToTLDateTime();
                        request.Ref = cashopref.AssignId.ToString();
                        request.Source = QSEnumCashOPSource.Manual;
                        request.Status = QSEnumCashInOutStatus.PENDING;

                        ORM.MCashOpAccount.InsertAccountCashOperation(request);

                        //通过事件中继触发事件
                        TLCtxHelper.EventSystem.FireCashOperation(this, QSEnumCashOpEventType.Request, request);

                        session.OperationSuccess("提交交易帐户出入金成功");

                        //如果自动确认该代理的出入金 则执行确认操作
                        //if (access.r_cashop_auto_confirm)
                        //{
                        //    //调用清算中心出入金确认操作
                        //    //TLCtxHelper..ConfirmCashOperation(request.Ref);

                        //    //重新从数据库加载数据 返回当前记录的数据
                        //    request = ORM.MCashOpAccount.GetAccountCashOperation(request.Ref);
                        //    session.ReplyMgr(request);
                        //    //通过事件中继触发事件
                        //    TLCtxHelper.EventSystem.FireCashOperation(this, QSEnumCashOpEventType.Confirm, request);

                        //}
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
