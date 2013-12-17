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
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAgentPaymentInfo", "QryAgentPaymentInfo - query payment Info", "查询代理支付信息")]
        public void CTE_QryPaymentInfo(ISession session, int agentfk)
        {
            debug("查询代理支付信息", QSEnumDebugLevel.INFO);
            Manager manger = BasicTracker.ManagerTracker[agentfk];
            JsonWrapperAgentPaymentInfo info = manger.GetPaymentInfo();
            session.ReplyMgr(info);
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAgentCashOperationTotal", "QryAgentCashOperationTotal - query agent pending cash operation", "查询所有代理待处理委托")]
        public void CTE_QryAgentCashOperationTotal(ISession session)
        {
            debug("查询代理所有出入金请求记录", QSEnumDebugLevel.INFO);
            //查询所有代理出入金记录
            Manager manger = session.GetManager();
            JsonWrapperCashOperation[] ops = manger.Domain.GetAgentCashOperation().ToArray();
            session.ReplyMgr(ops);
        }

        IdTracker cashopref = new IdTracker();
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RequestCashOperation", "RequestCashOperation -rquest deposit or withdraw", "请求出入金操作", true)]
        public void CTE_RequestCashOperation(ISession session, string playload)
        {
            Manager manger = session.GetManager();

            JsonWrapperCashOperation request = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
            if (request != null)
            {
                request.mgr_fk = manger.mgr_fk;

                request.DateTime = Util.ToTLDateTime();
                request.Ref = cashopref.AssignId.ToString();
                request.Source = QSEnumCashOPSource.Manual;
                request.Status = QSEnumCashInOutStatus.PENDING;


                ORM.MAgentFinance.InsertAgentCashOperation(request);

                //bankaccount.Bank = BasicTracker.ContractBankTracker[bankaccount.bank_id].ToJsonWrapperBank();
                //session.SendJsonReplyMgr(request);

                //通知出入金操作
                //NotifyCashOperation(request);
                //通过事件中继触发事件
                TLCtxHelper.CashOperationEvent.FireCashOperation(this, QSEnumCashOpEventType.Request, request);
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
            try
            {
                debug("确认出入金操作请求", QSEnumDebugLevel.INFO);
                Manager manger = session.GetManager();
                JsonWrapperCashOperation request = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(playload);
                
                HandlerMixins.Valid_ObjectNotNull(request);

                //if (request.Status != QSEnumCashInOutStatus.PENDING)
                //{
                //    throw new FutsRspError("出入金请求状态异常");
                //}

                ////出金执行金额检查
                //if (op.Operation == QSEnumCashOperation.WithDraw)
                //{
                //    if (account.NowEquity < op.Amount)
                //    {
                //        throw new FutsRspError("出入金额度大于帐户权益");
                //    }
                //}

                //执行时间检查 
                if (TLCtxHelper.Ctx.SettleCentre.IsInSettle)
                {
                    throw new FutsRspError("系统正在结算,禁止出入金操作");
                }

                ORM.MAgentFinance.ConfirmAgentCashOperation(request);
                session.ReplyMgr(request);
                //通过事件中继触发事件
                TLCtxHelper.CashOperationEvent.FireCashOperation(this, QSEnumCashOpEventType.Confirm, request);
                
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
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
                    session.ReplyMgr(request);
                    //通过事件中继触发事件
                    TLCtxHelper.CashOperationEvent.FireCashOperation(this, QSEnumCashOpEventType.Cancel, request);
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
                    session.ReplyMgr(request);
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
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAgentCashTrans", "QueryAgentCashTrans -query agents cashtrans", "查询代理出入金记录")]
        public void CTE_QueryAgentCashTrans(ISession session,int agentfk, long start, long end)
        {
            debug("查询出入金记录", QSEnumDebugLevel.INFO);
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCasnTrans[] trans = ORM.MAgentFinance.SelectAgentCashTrans(agentfk, start, end).ToArray();
                session.ReplyMgr(trans);
            }
        }
    }
}
