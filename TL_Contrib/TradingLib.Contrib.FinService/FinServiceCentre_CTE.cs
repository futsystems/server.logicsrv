using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Reflection;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Contrib.FinService
{
    public partial class FinServiceCentre
    {
        [TaskAttr("3秒执行配资帐户风控检查",1, "执行配资帐户风控检查")]
        public void CTE_CheckPosition()
        {
            //执行强平检查时,需要等待系统启动完毕后才正常进行，否则在没有正常价格或者启动不完备的情况下，会出现提前强平，但是强平时候通道没有打开或者没有行情，或者错误的触发了强平动作
            if (!TLCtxHelper.IsReady) return;
            foreach (FinServiceStub stub in FinTracker.FinServiceTracker)
            {
                stub.CheckAccount();
            }
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFinService", "QryFinService - query finservice of account", "查询某个帐户的配资服务")]
        public void CTE_QryFinService(ISession session, string account)
        {
            debug("查询帐户:" + account + "的配资服务", QSEnumDebugLevel.INFO);
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null)
            { 
                SendJsonReplyMgr(session,Mixins.JsonReply.GenericError(1,"交易帐号不存在"));
                return;
            }

            FinServiceStub stub = FinTracker.FinServiceTracker[account];
            if (stub == null)
            {
                SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "无有效配资服务"));
                return;
            }
            SendJsonReplyMgr(session, stub.ToJsonWrapperFinServiceStub());
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateArguments", "UpdateArguments - update argument of finservice", "更新某个帐户的配资参数", true)]
        public void CTE_UpdateArguments(ISession session,string playload)
        {
            debug("arg:" + playload, QSEnumDebugLevel.INFO);
            JsonWrapperFinServiceStub target = Mixins.LitJson.JsonMapper.ToObject<JsonWrapperFinServiceStub>(playload);

            debug("更新帐户:" + target.Account+ "的配资服务参数", QSEnumDebugLevel.INFO);
            IAccount acc = TLCtxHelper.CmdAccount[target.Account];
            if (acc == null)
            {
                SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "交易帐号不存在"));
                return;
            }
            FinServiceStub stub = FinTracker.FinServiceTracker[target.Account];
            if (stub == null)
            {
                SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "无有效配资服务"));
                return;
            }

            if (!stub.ID.Equals(target.ID))
            { 
                
            }

            //更新参数
            FinTracker.ArgumentTracker.UpdateArgumentAccount(target.ID, target.FinService.Arguments);

            //加载参数
            stub.LoadArgument();
        }
    }


}
