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
            Manager manger = session.GetManager();
            if (manger != null)
            {
                JsonWrapperCashOperation[] ops = ORM.MCashOpAccount.GetAccountLatestCashOperationTotal().ToArray();
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

    }
}
