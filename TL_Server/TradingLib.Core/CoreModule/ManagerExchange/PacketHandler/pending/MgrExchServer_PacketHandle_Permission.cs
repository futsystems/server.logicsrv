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
        /// 查询模板列表
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryPermmissionTemplate", "QueryPermmissionTemplate - Query Permmission lsit", "查询权限模板列表")]
        public void CTE_QueryPermissionTemplateList(ISession session)
        {

            Manager manager = session.GetManager();
            session.ReplyMgr(manager.Domain.GetPermissions().Where(p => p.manager_id == manager.BaseMgrID).ToArray());
        }

        /// <summary>
        /// 更新权限模板
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdatePermission", "UpdatePermission - update permission config", "更新权限模板",QSEnumArgParseType.Json)]
        public void CTE_UpdatePermissionTemplateList(ISession session, string json)
        {
            Manager manger = session.GetManager();
            if (manger.IsRoot() || manger.IsAgent())
            {
                Permission permission = json.DeserializeObject<Permission>();// Mixins.Json.JsonMapper.ToObject<UIAccess>(playload);
                //更新域信息
                permission.domain_id = manger.domain_id;
                permission.manager_id = manger.BaseMgrID;


                BasicTracker.UIAccessTracker.UpdatePermission(permission);
                session.NotifyMgr("NotifyPermissionTemplate", BasicTracker.UIAccessTracker[permission.id]);
                session.RspMessage("权限模板更新成功");
            }
            else
            {
                throw new FutsRspError("无权查询权限模板列表");
            }
        }

        /// <summary>
        /// 删除权限模板
        /// </summary>
        /// <param name="session"></param>
        /// <param name="template_id"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeletePermissionTemplate", "DeletePermissionTemplate - delete permission template", "删除权限模板")]
        public void CTE_DelPermissionTemplate(ISession session, int template_id)
        { 
             Manager manager = session.GetManager();
            logger.Info(string.Format("管理员:{0} 删除权限模板 request:{1}", manager.Login, template_id));
            if (manager.IsRoot() || manager.IsAgent())
            {
                Permission access = BasicTracker.UIAccessTracker[template_id];
                if (access == null)
                {
                    throw new FutsRspError("指定权限模板不存在");
                }
                if (manager.domain_id != access.domain_id)
                {
                    throw new FutsRspError("权限模板与管理员不属于同一域");
                }

                //删除权限模板 权限模板中Manager与权限模板的map在tracker中维护
                BasicTracker.UIAccessTracker.DeletePermissionTemplate(template_id);

                session.NotifyMgr("NotifyDeletePermissionTemplate", access);
                session.RspMessage("删除续费模板成功");
            }
            else
            {
                throw new FutsRspError("无权删除权限模板");
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAgentPermission", "QueryAgentPermission - query agent permission", "查询某个代理的权限设置")]
        public void CTE_QueryAgentPermission(ISession session, int managerid)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot() || manager.IsAgent())
            {
                Manager target = BasicTracker.ManagerTracker[managerid];
                if (target == null)
                {
                    throw new FutsRspError("指定的管理员不存在");
                }

                session.ReplyMgr(target.Permission);
            }
            else
            {
                throw new FutsRspError("无权查询代理权限设置");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAgentPermission", "UpdateAgentPermission - updaet agent permission", "更新管理员的权限设置")]
        public void CTE_UpdateAgentPermission(ISession session, int managerid, int permission_id)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot() || manager.IsAgent())
            {
                Manager m = BasicTracker.ManagerTracker[managerid];
                if (m == null)
                {
                    throw new FutsRspError("指定的管理员不存在");
                }
                BasicTracker.UIAccessTracker.UpdateManagerPermission(m, permission_id);
                session.NotifyMgr("NotifyAgentPermission",m.Permission);
                session.RspMessage("更新柜员权限设置成功");
            }
            else
            {
                throw new FutsRspError("无权更新代理权限设置");
            }
        }
    }
}
