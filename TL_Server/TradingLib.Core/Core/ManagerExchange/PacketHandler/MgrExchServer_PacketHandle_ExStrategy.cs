using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        /// <summary>
        /// 查询计算策略模板
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryExStrategyTemplate", "QryExStrategyTemplate - qry exstrategy template", "查询计算策略模板")]
        public void CTE_QryExStrategyTemplate(ISession session)
        {
            Manager manager = session.GetManager();
            UIAccess access = manager.GetAccess();
            if (!access.r_exstrategy)
            {
                throw new FutsRspError("无权查询交易参数模板");
            }

            if (manager.BaseManager.IsRoot())
            {
                ExStrategyTemplate[] templates = manager.Domain.GetExStrategyTemplate().ToArray();
                session.ReplyMgr(templates);
            }
            else if(manager.BaseManager.IsAgent())
            {
                ExStrategyTemplate[] templates = manager.Domain.GetExStrategyTemplate().Where(item => item.Manager_ID == manager.BaseMgrID).ToArray();
                session.ReplyMgr(templates);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateExStrategyTemplate", "UpdateExStrategyTemplate - update exstrategy template", "更新计算策略模板", QSEnumArgParseType.Json)]
        public void CTE_UpdateExStrategyTemplate(ISession session, string json)
        {
            Manager manager = session.GetManager();
            UIAccess access = manager.GetAccess();
            if (!access.r_exstrategy)
            {
                throw new FutsRspError("无权更新交易参数模板");
            }
            //if (manager.IsRoot())
            {
                ExStrategyTemplateSetting t = Mixins.Json.JsonMapper.ToObject<ExStrategyTemplateSetting>(json);
                t.Domain_ID = manager.domain_id;
                bool isaddd = t.ID == 0;
                if (isaddd)
                {
                    t.Manager_ID = manager.BaseMgrID;//如果是新添加 则设定管理主域ID
                }
                else
                {
                    ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[t.ID];
                    if (template != null)
                    {
                        if (template.Manager_ID != manager.BaseMgrID)
                        {
                            throw new FutsRspError(string.Format("无权修改交易参数模板[{0}]", template.Name));
                        }
                    }
                }
                BasicTracker.ExStrategyTemplateTracker.UpdateExStrategyTemplate(t);

                //如果是添加手续费模板 则需要预先将数据写入到数据库

                session.NotifyMgr("NotifyExStrategyTemplate", BasicTracker.ExStrategyTemplateTracker[t.ID]);
                session.OperationSuccess("更新计算策略模板成功");
            }
            //else
            //{
            //    throw new FutsRspError("无权修改计算策略模板");
            //}
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryExStrategyTemplateItem", "QryExStrategyTemplateItem - qry commission template item", "查询手续费模板项目")]
        public void CTE_QryExStrategyTemplateItem(ISession session, int templateid)
        {
            Manager manager = session.GetManager();
            UIAccess access = manager.GetAccess();
            if (!access.r_exstrategy)
            {
                throw new FutsRspError("无权查询交易参数模板项目");
            }
            //if (manager.IsRoot())
            {
                ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[templateid];
                session.ReplyMgr(template.ExStrategy);
            }
            //else
            //{
            //    throw new FutsRspError("无权查询计算策略模板项目");
            //}
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateExStrategyTemplateItem", "UpdateExStrategyTemplateItem - update exstrategy template item", "更新交易参数模板项目", QSEnumArgParseType.Json)]
        public void CTE_UpdateExStrategyTemplateItem(ISession session, string json)
        {
            Manager manager = session.GetManager();
            UIAccess access = manager.GetAccess();
            if (!access.r_exstrategy)
            {
                throw new FutsRspError("无权更新交易参数模板项目");
            }
            //if (manager.IsRoot())
            {
                ExStrategy item = Mixins.Json.JsonMapper.ToObject<ExStrategy>(json);
                ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[item.Template_ID];
                if (template == null)
                {
                    throw new FutsRspError("指定交易参数模板不存在");
                }

                BasicTracker.ExStrategyTemplateTracker.UpdateExStrategy(item);
                session.NotifyMgr("NotifyExStrategyTemplateItem", BasicTracker.ExStrategyTemplateTracker[item.Template_ID].ExStrategy);
                session.OperationSuccess("更新交易参数模板成功");

            }
        }
    }
}
