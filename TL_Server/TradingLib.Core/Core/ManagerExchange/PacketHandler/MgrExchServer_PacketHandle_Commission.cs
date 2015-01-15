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
        /// 查询手续费模板
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryCommissionTemplate", "QryCommissionTemplate - qry commission template", "查询手续费模板")]
        public void CTE_QryCommissionTemplate(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                CommissionTemplateSetting[] templates = manager.Domain.GetCommissionTemplate().ToArray();
                session.ReplyMgr(templates);
            }
            else
            {
                throw new FutsRspError("无权查询手续费模板");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateCommissionTemplate", "UpdateCommissionTemplate - update commission template", "更新手续费模板",QSEnumArgParseType.Json)]
        public void CTE_UpdateCommissionTemplate(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                CommissionTemplateSetting t = Mixins.Json.JsonMapper.ToObject<CommissionTemplateSetting>(json);
                t.Domain_ID = manager.domain_id;
                bool isaddd = t.ID == 0;
                BasicTracker.CommissionTemplateTracker.UpdateCommissionTemplate(t);

                //如果是添加手续费模板 则需要预先将数据写入到数据库
                if (isaddd)
                { 
                    
                }

                session.NotifyMgr("NotifyCommissionTemplate",BasicTracker.CommissionTemplateTracker[t.ID]);
                session.OperationSuccess("更新手续费模板成功");
            }
            else
            {
                throw new FutsRspError("无权修改手续费模板");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryCommissionTemplateItem", "QryCommissionTemplateItem - qry commission template item", "查询手续费模板项目")]
        public void CTE_QryCommissionTemplate(ISession session,int templateid)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                CommissionTemplate template = BasicTracker.CommissionTemplateTracker[templateid];

                CommissionTemplateItemSetting[] items = template.CommissionItems.ToArray();
                session.ReplyMgr(items);
            }
            else
            {
                throw new FutsRspError("无权查询手续费模板项目");
            }
        }
    }
}
