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
            if (manager.IsRoot())
            {
                ExStrategyTemplate[] templates = manager.Domain.GetExStrategyTemplate().ToArray();
                session.ReplyMgr(templates);
            }
            else
            {
                throw new FutsRspError("无权查询计算策略模板");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateExStrategyTemplate", "UpdateExStrategyTemplate - update exstrategy template", "更新计算策略模板", QSEnumArgParseType.Json)]
        public void CTE_UpdateExStrategyTemplate(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                ExStrategyTemplateSetting t = Mixins.Json.JsonMapper.ToObject<ExStrategyTemplateSetting>(json);
                t.Domain_ID = manager.domain_id;
                bool isaddd = t.ID == 0;
                BasicTracker.ExStrategyTemplateTracker.UpdateExStrategyTemplate(t);

                //如果是添加手续费模板 则需要预先将数据写入到数据库
                if (isaddd)
                {

                }
                session.NotifyMgr("NotifyExStrategyTemplate", BasicTracker.ExStrategyTemplateTracker[t.ID]);
                session.OperationSuccess("更新计算策略模板成功");
            }
            else
            {
                throw new FutsRspError("无权修改计算策略模板");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryExStrategyTemplateItem", "QryExStrategyTemplateItem - qry commission template item", "查询手续费模板项目")]
        public void CTE_QryExStrategyTemplateItem(ISession session, int templateid)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[templateid];
                session.ReplyMgr(template.ExStrategy);
            }
            else
            {
                throw new FutsRspError("无权查询计算策略模板项目");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateExStrategyTemplateItem", "UpdateExStrategyTemplateItem - update exstrategy template item", "更新交易参数模板项目", QSEnumArgParseType.Json)]
        public void CTE_UpdateExStrategyTemplateItem(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
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
