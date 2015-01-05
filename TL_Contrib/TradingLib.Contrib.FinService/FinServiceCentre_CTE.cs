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
        [TaskAttr("配资帐户风控检查",1, "执行配资帐户风控检查")]
        public void CTE_CheckPosition()
        {
            //执行强平检查时,需要等待系统启动完毕后才正常进行，否则在没有正常价格或者启动不完备的情况下，会出现提前强平，但是强平时候通道没有打开或者没有行情，或者错误的触发了强平动作
            if (!TLCtxHelper.IsReady) return;
            foreach (FinServiceStub stub in FinTracker.FinServiceTracker)
            {
                stub.CheckAccount();
            }
        }

        #region 代理参数查询与设置

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAgentSPArg", "QryAgentSPArg - qry agent sparg  of account", "查询代理某个服务计划的参数")]
        public void CTE_QryAgentSPArg(ISession session, int agentfk, int spfk)
        {
            debug("查询代理:" + agentfk.ToString() + " 的配资服务计划:" + spfk.ToString()+" 参数", QSEnumDebugLevel.INFO);
            Manager m = BasicTracker.ManagerTracker[agentfk];
            //session.ManagerID 对应的登入ID 这里需要用AgentFK标识
            if (m == null)
            { 
                
            }
            //获得对应的服务计划
            DBServicePlan sp = FinTracker.ServicePlaneTracker[spfk];
            if (sp == null)
            { 
            
            }
            Dictionary<string, Argument> argumap = FinTracker.ArgumentTracker.GetAgentArgument(agentfk, spfk);

            List<JsonWrapperArgument> list = new List<JsonWrapperArgument>();
            foreach (string key in argumap.Keys)
            {
                Argument arg = argumap[key];
                string name = arg.Name;
                string value = arg.Value;
                string type = arg.Type.ToString();
                IEnumerable<ArgumentAttribute> attrlist = FinTracker.ServicePlaneTracker.GetAttribute(spfk);
                ArgumentAttribute attr = attrlist.Where(a => a.Name.Equals(name)).SingleOrDefault();
                if (attr == null)
                {
                    continue;
                }

                string title = attr.Title;

                bool editable = attr.Editable;
                list.Add(new JsonWrapperArgument
                {
                    ArgName = name,
                    ArgTitle = title,
                    ArgValue = value,
                    ArgType = type,
                    Editable = editable,

                });
            }

            Type sptype = FinTracker.ServicePlaneTracker.GetFinServiceType(spfk);
            //if (sptype == null) return;//如果没有获得对应的类型 则直接返回

            //2.生成对应的IFinService
            IFinService fs = (IFinService)Activator.CreateInstance(sptype);

            JsonWrapperServicePlanAgentArgument ret = new JsonWrapperServicePlanAgentArgument
            {
                ClassName = sp.ClassName,
                Name = sp.Name,
                Title = sp.Title,
                serviceplan_fk = sp.ID,
                agent_fk = agentfk,
                ChargeType =Util.GetEnumDescription(fs.ChargeType),
                CollectType = Util.GetEnumDescription(fs.CollectType),
                Arguments = list.ToArray(),
            };
            session.ReplyMgr(ret);
            //SendJsonReplyMgr(session, ret);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAgentSPArg", "UpdateAgentSPArg - update agent service plan arg  of account", "更新代理配资服务计划参数",true)]
        public void CTE_UpdateAgentSPArg(ISession session, string playload)
        {
            debug("arg:" + playload, QSEnumDebugLevel.INFO);
            JsonWrapperServicePlanAgentArgument target = Mixins.Json.JsonMapper.ToObject<JsonWrapperServicePlanAgentArgument>(playload);
            
            //更新参数
            FinTracker.ArgumentTracker.UpdateArgumentAgent(target.agent_fk, target.serviceplan_fk, target.Arguments);

            //代理更新完毕参数后 需要将该代理下所有客户 对应的该服务计划的配资服务重新加载参数 
            foreach (FinServiceStub stub in FinTracker.FinServiceTracker.Where(s => s.Account.Mgr_fk == target.agent_fk && s.serviceplan_fk == target.serviceplan_fk))
            {
                stub.LoadArgument();
            }


        }
        #endregion


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
                throw new FutsRspError("交易帐号不存在");
                //SendJsonReplyMgr(session,Mixins.JsonReply.GenericError(1,"交易帐号不存在"));
                //return;
            }

            FinServiceStub stub = FinTracker.FinServiceTracker[account];
            if (stub == null)
            {
                throw new FutsRspError("无有效配资服务");
                //SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "无有效配资服务"));
                //return;
            }
            session.ReplyMgr(stub.ToJsonWrapperFinServiceStub());
            //SendJsonReplyMgr(session, stub.ToJsonWrapperFinServiceStub());
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
            JsonWrapperFinServiceStub target = Mixins.Json.JsonMapper.ToObject<JsonWrapperFinServiceStub>(playload);

            debug("更新帐户:" + target.Account+ "的配资服务参数", QSEnumDebugLevel.INFO);
            IAccount acc = TLCtxHelper.CmdAccount[target.Account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐号不存在");
                //SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "交易帐号不存在"));
                //return;
            }
            FinServiceStub stub = FinTracker.FinServiceTracker[target.Account];
            if (stub == null)
            {
                throw new FutsRspError("无有效配资服务");
                //SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "无有效配资服务"));
                //return;
            }

            if (!stub.ID.Equals(target.ID))
            { 
                
            }

            //更新参数
            FinTracker.ArgumentTracker.UpdateArgumentAccount(target.ID, target.FinService.Arguments);
            
            //加载参数
            stub.LoadArgument();

            //调用参数变动事件回调
            stub.FinService.OnArgumentChanged();

            FinServiceStub stub2 = FinTracker.FinServiceTracker[target.Account];
            if (stub2 == null)
            {
                throw new FutsRspError("无有效配资服务");
                //SendJsonReplyMgr(session, Mixins.JsonReply.GenericError(1, "无有效配资服务"));
                //return;
            }
            session.ReplyMgr(stub2.ToJsonWrapperFinServiceStub());
            //SendJsonReplyMgr(session, stub2.ToJsonWrapperFinServiceStub());
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ChangeServicePlane", "ChangeServicePlane - add or change finservice of account", "添加或者修改某个帐户的配资服务", true)]
        public void CTE_ChangeServicePlane(ISession session, string playload)
        {
            debug("request:" + playload, QSEnumDebugLevel.INFO);
            JsonWrapperChgServicePlaneRequest request = TradingLib.Mixins.Json.JsonMapper.ToObject<JsonWrapperChgServicePlaneRequest>(playload);
            string account = request.Account;
            int serviceplan_fk = request.ServicePlaneFK;

            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐号不存在");
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
                throw new FutsRspError("无有效配资服务");
            }
            session.ReplyMgr(stub2.ToJsonWrapperFinServiceStub());

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeleteServicePlane", "DeleteServicePlane - delete of account", "添加或者修改某个帐户的配资服务", true)]
        public void CTE_DeleteServicePlane(ISession session, string account)
        {
            debug("删除帐户:"+account+"的配资服务", QSEnumDebugLevel.INFO);
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐号不存在");
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
                session.ReplyMgr(null);
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFinServicePlan", "QryFinServicePlan - query serviceplane", "查询所有服务计划列表")]
        public void CTE_QryFinServicePlan(ISession session)
        {
            try
            {
                
                Manager manager = session.GetManager();
                IEnumerable<JsonWrapperServicePlane> splist = FinTracker.ServicePlaneTracker.ServicePlans.Where(sp => sp.Active).Select(sp => sp.ToJsonWrapperServicePlane());

                if (manager.Domain.Super)
                {
                    session.ReplyMgr(splist.ToArray());
                    //SendJsonReplyMgr(session, splist.ToArray());
                }
                else
                {
                    int[] idlist = (string.IsNullOrEmpty(manager.Domain.FinSPList)?new int[]{0}:manager.Domain.FinSPList.Split(',').Select(s => int.Parse(s)).ToArray());
                    session.ReplyMgr(splist.Where(sp => idlist.Contains(sp.ID)).ToArray());
                    //SendJsonReplyMgr(session, splist.Where(sp=>idlist.Contains(sp.ID)).ToArray());
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        /// <summary>
        /// 查询某个交易日 代理商汇总记录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="settleday"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryTotalReport", "QryTotalReport - query totalreport", "查询某日所有代理的汇总统计")]
        public void CTE_QryTotalReport(ISession session,int agent,int settleday)
        {
            JsonWrapperToalReport report = ORM.MServiceChargeReport.GenTotalReport(agent, settleday);
            session.ReplyMgr(report);
            //SendJsonReplyMgr(session, report);
        }

        /// <summary>
        /// 查询某个代理 在某个时间段内的流水汇总
        /// </summary>
        /// <param name="session"></param>
        /// <param name="agentfk"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QrySummaryReport", "QrySummaryReport - query summary report", "查询某个代理在一段时间内的汇总")]
        public void CTE_QrySummaryReport(ISession session, int agentfk,int start,int end)
        {
            JsonWrapperToalReport report = ORM.MServiceChargeReport.GenSummaryReportByDayRange(agentfk, start, end);
            session.ReplyMgr(report);
            //SendJsonReplyMgr(session, report);
        }

        /// <summary>
        /// 查询某个代理在某段时间的每日汇总流水
        /// </summary>
        /// <param name="session"></param>
        /// <param name="agentfk"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryTotalReportDayRange", "QryTotalReportDayRange - query totalreport", "查询某个代理某个时间段内利润流水")]
        public void CTE_QryTotalReport(ISession session, int agentfk,int start,int end)
        {
            JsonWrapperToalReport[] reports = ORM.MServiceChargeReport.GenTotalReportByDayRange(agentfk,start,end).Select((ret) => { return FillTotalReport(ret); }).ToArray();
            session.ReplyMgr(reports);
            //SendJsonReplyMgr(session, reports);
        }

        /// <summary>
        /// 查询某个代理在某个结算日的客户流水分组
        /// </summary>
        /// <param name="session"></param>
        /// <param name="agentfk"></param>
        /// <param name="settleday"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryDetailReportByAccount", "QryDetailReportByAccount - query totalreport", "查询某个代理某个时间段内利润流水")]
        public void CTE_QryDetailReportByAccount(ISession session, int agentfk,int settleday)
        {
            JsonWrapperToalReport[] reports = ORM.MServiceChargeReport.GenDetailReportByAccount(agentfk, settleday).Select((ret) => { return FillTotalReport(ret); }).ToArray();
            session.ReplyMgr(reports);
            //SendJsonReplyMgr(session, reports);
        }

        JsonWrapperToalReport FillTotalReport(JsonWrapperToalReport report)
        {
            Manager m = BasicTracker.ManagerTracker[report.Agent_FK];
            if (m != null)
            {
                report.AgentName = m.Name;
                report.Mobile = m.Mobile;
                report.QQ = m.QQ;
            }
            return report;
        }
    }


}
