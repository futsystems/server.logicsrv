﻿using System;
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
            try
            {
                Manager manger = session.GetManager();
                if (manger.IsInRoot())
                {
                    session.ReplyMgr(manger.Domain.GetUIAccesses().ToArray());
                }
                else
                {
                    throw new FutsRspError("无权查询权限模板列表");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        /// <summary>
        /// 更新权限模板
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdatePermission", "UpdatePermission - update permission config", "更新权限模板",QSEnumArgParseType.Json)]
        public void CTE_UpdatePermissionTemplateList(ISession session, string playload)
        {
            Manager manger = session.GetManager();
            if (manger.IsInRoot())
            {
                UIAccess access = playload.DeserializeObject<UIAccess>();// Mixins.Json.JsonMapper.ToObject<UIAccess>(playload);
                //更新域信息
                access.domain_id = manger.domain_id;

                BasicTracker.UIAccessTracker.UpdateUIAccess(access);//更新
                session.NotifyMgr("NotifyPermissionTemplate", BasicTracker.UIAccessTracker[access.id]);
                session.OperationSuccess("权限模板更新成功");
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
            if (manager.IsRoot())
            {
                UIAccess access = BasicTracker.UIAccessTracker[template_id];
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
                session.OperationSuccess("删除续费模板成功");
            }
            else
            {
                throw new FutsRspError("无权删除权限模板");
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAgentPermission", "QueryAgentPermission - query agent permission", "查询某个代理的权限设置")]
        public void CTE_QueryAgentPermission(ISession session, int managerid)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.IsInRoot())
                {
                    UIAccess access = BasicTracker.UIAccessTracker.GetAgentUIAccess(managerid);
                    session.ReplyMgr(access);
                }
                else
                {
                    throw new FutsRspError("无权查询代理权限设置");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAgentPermission", "UpdateAgentPermission - updaet agent permission", "更新管理员的权限设置")]
        public void CTE_UpdateAgentPermission(ISession session, int managerid, int accessid)
        {
            Manager manger = session.GetManager();
            if (manger.IsInRoot())
            {
                Manager m = BasicTracker.ManagerTracker[managerid];
                if (m == null)
                {
                    throw new FutsRspError("指定的管理员不存在");
                }
                BasicTracker.UIAccessTracker.UpdateAgentPermission(managerid, accessid);
                UIAccess access = BasicTracker.UIAccessTracker.GetAgentUIAccess(managerid);
                session.NotifyMgr("NotifyAgentPermission",access);
                session.OperationSuccess("更新柜员权限设置成功");
            }
            else
            {
                throw new FutsRspError("无权更新代理权限设置");
            }
        }
    }
}
