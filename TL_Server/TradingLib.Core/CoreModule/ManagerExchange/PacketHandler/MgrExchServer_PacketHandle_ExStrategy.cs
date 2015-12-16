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

            if (manager.IsRoot())
            {
                ExStrategyTemplate[] templates = manager.Domain.GetExStrategyTemplate().ToArray();
                session.ReplyMgr(templates);
            }
            else if (manager.BaseManager.IsAgent())
            {
                ExStrategyTemplate[] templates = manager.Domain.GetExStrategyTemplate().Where(item => item.Manager_ID == manager.BaseMgrID).ToArray();
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
            UIAccess access = manager.GetAccess();
            if (!access.r_exstrategy)
            {
                throw new FutsRspError("无权更新交易参数模板");
            }

            ExStrategyTemplateSetting t = Mixins.Json.JsonMapper.ToObject<ExStrategyTemplateSetting>(json);
            t.Domain_ID = manager.domain_id;
            bool isaddd = t.ID == 0;

            //如果是添加手续费模板 则需要预先将数据写入到数据库
            if (isaddd)
            {
                t.Manager_ID = manager.BaseMgrID;//如果是新添加 则设定管理主域ID    
            }
            else
            {
                if (!manager.IsRoot())
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
            }

            BasicTracker.ExStrategyTemplateTracker.UpdateExStrategyTemplate(t);
            session.NotifyMgr("NotifyExStrategyTemplate", BasicTracker.ExStrategyTemplateTracker[t.ID]);
            session.OperationSuccess("更新计算策略模板成功");

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeleteExStrategyTemplate", "DeleteExStrategyTemplate - delete ex strategy template", "删除交易参数模板")]
        public void CTE_DelExStrategyTemplate(ISession session, int template_id)
        { 
            Manager manager = session.GetManager();
            logger.Info(string.Format("管理员:{0} 删除交易参数模板 request:{1}", manager.Login, template_id));
            UIAccess access = manager.GetAccess();
            if (!access.r_exstrategy)
            {
                throw new FutsRspError("无权更新交易参数模板");
            }

            ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[template_id];
            if (template == null)
            {
                throw new FutsRspError("指定交易参数模板不存在");
            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("交易参数模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权删除交易参数模板[{0}]", template.Name));
                }
            }


            //调用维护器 删除该模板
            BasicTracker.ExStrategyTemplateTracker.DeleteExStrategyTemplate(template_id);
            IAccount[] accounts = manager.Domain.GetAccounts().ToArray();

            //遍历所有交易帐户 如果交易参数模板为删掉的模板则将模板id设置为0
            for (int i = 0; i < accounts.Length; i++)
            {
                IAccount acc = accounts[i];
                if (acc.ExStrategy_ID == template_id)
                {
                    TLCtxHelper.ModuleAccountManager.UpdateAccountExStrategyTemplate(acc.ID, 0);
                }
            }
            session.NotifyMgr("NotifyDeleteExStrategyTemplate", template);
            session.OperationSuccess("删除交易参数模板成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryExStrategyTemplateItem", "QryExStrategyTemplateItem - qry commission template item", "查询手续费模板项目")]
        public void CTE_QryExStrategyTemplateItem(ISession session, int templateid)
        {
            Manager manager = session.GetManager();
            UIAccess access = manager.GetAccess();
            if (!access.r_exstrategy)
            {
                throw new FutsRspError("无权更新交易参数模板");
            }

            ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[templateid];
            if (template == null)
            {
                throw new FutsRspError("指定交易参数模板不存在");
            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("交易参数模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权查询交易参数模板[{0}]", template.Name));
                }
            }
            session.ReplyMgr(template.ExStrategy);
            
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateExStrategyTemplateItem", "UpdateExStrategyTemplateItem - update exstrategy template item", "更新交易参数模板项目", QSEnumArgParseType.Json)]
        public void CTE_UpdateExStrategyTemplateItem(ISession session, string json)
        {
            Manager manager = session.GetManager();
            UIAccess access = manager.GetAccess();
            if (!access.r_exstrategy)
            {
                throw new FutsRspError("无权更新交易参数模板");
            }

            ExStrategy item = Mixins.Json.JsonMapper.ToObject<ExStrategy>(json);
            ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[item.Template_ID];
            if (template == null)
            {
                throw new FutsRspError("指定交易参数模板不存在");
            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("交易参数模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权查询交易参数模板[{0}]", template.Name));
                }
            }

            BasicTracker.ExStrategyTemplateTracker.UpdateExStrategy(item);
            session.NotifyMgr("NotifyExStrategyTemplateItem", BasicTracker.ExStrategyTemplateTracker[item.Template_ID].ExStrategy);
            session.OperationSuccess("更新交易参数模板成功");

        }
    }
}
