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


        /// <summary>
        /// 查询配资服务
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
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


        /*
         * 更新配资服务参数
         * 修改服务计划
         * 删除配资服务
         * 均以当前配资stub作为回报
         * 
         * 
         * **/
        /// <summary>
        /// 更新配资参数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="playload"></param>
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

            FinServiceStub stub2 = FinTracker.FinServiceTracker[target.Account];
            if (stub2 == null)
            {
                SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "无有效配资服务"));
                return;
            }
            SendJsonReplyMgr(session, stub2.ToJsonWrapperFinServiceStub());
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ChangeServicePlane", "ChangeServicePlane - add or change finservice of account", "添加或者修改某个帐户的配资服务", true)]
        public void CTE_ChangeServicePlane(ISession session, string playload)
        {
            debug("request:" + playload, QSEnumDebugLevel.INFO);
            JsonWrapperChgServicePlaneRequest request = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperChgServicePlaneRequest>(playload);
            string account = request.Account;
            int serviceplan_fk = request.ServicePlaneFK;

            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null)
            {
                SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "交易帐号不存在"));
                return;
            }

            FinServiceStub stub = FinTracker.FinServiceTracker[account];
            //如果配资服务存在则先删除 然后再添加配资服务
            if(stub != null)
                FinTracker.FinServiceTracker.DeleteFinService(stub);
            //添加配资服务
            FinTracker.FinServiceTracker.AddFinService(account, serviceplan_fk);


            //通知当前服务
            FinServiceStub stub2 = FinTracker.FinServiceTracker[account];
            if (stub2 == null)
            {
                SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "无有效配资服务"));
                return;
            }
            SendJsonReplyMgr(session, stub2.ToJsonWrapperFinServiceStub());

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeleteServicePlane", "DeleteServicePlane - delete of account", "添加或者修改某个帐户的配资服务", true)]
        public void CTE_DeleteServicePlane(ISession session, string account)
        {
            debug("删除帐户:"+account+"的配资服务", QSEnumDebugLevel.INFO);
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null)
            {
                SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "交易帐号不存在"));
                return;
            }

            FinServiceStub stub = FinTracker.FinServiceTracker[account];
            //如果配资服务不存在 则添加配资服务
            if (stub != null)
            {
                FinTracker.FinServiceTracker.DeleteFinService(stub);
            }


            //通知当前服务
            FinServiceStub stub2 = FinTracker.FinServiceTracker[account];
            if (stub2 == null)
            {
                SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "无有效配资服务"));
                return;
            }
            SendJsonReplyMgr(session, stub2.ToJsonWrapperFinServiceStub());
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFinServicePlan", "QryFinServicePlan - query serviceplane", "查询所有服务计划列表")]
        public void CTE_QryFinServicePlan(ISession session)
        {
            JsonWrapperServicePlane[] splist = FinTracker.ServicePlaneTracker.ServicePlans.Select(sp => sp.ToJsonWrapperServicePlane()).ToArray();
            SendJsonReplyMgr(session, splist);
        }

    }


}
