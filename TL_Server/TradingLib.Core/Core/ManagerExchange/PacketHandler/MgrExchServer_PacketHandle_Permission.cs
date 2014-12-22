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
        /// <summary>
        /// 查询模板列表
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryPermmissionTemplateList", "QueryPermmissionTemplateList - Query Permmission lsit", "查询权限模板列表")]
        public void CTE_QueryPermissionTemplateList(ISession session)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
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
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdatePermission", "UpdatePermission - update permission config", "更新权限模板", true)]
        public void CTE_UpdatePermissionTemplateList(ISession session, string playload)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    UIAccess access = Mixins.LitJson.JsonMapper.ToObject<UIAccess>(playload);
                    //更新域信息
                    access.domain_id = manger.domain_id;

                    BasicTracker.UIAccessTracker.UpdateUIAccess(access);//更新
                    session.OperationSuccess("权限模板更新成功");
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

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAgentPermission", "QueryAgentPermission - query agent permission", "查询某个代理的权限设置")]
        public void CTE_QueryAgentPermission(ISession session, int managerid)
        {
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
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
            try
            {
                Manager manger = session.GetManager();
                if (manger.RightRootDomain())
                {
                    Manager m = BasicTracker.ManagerTracker[managerid];
                    if (m == null)
                    {
                        throw new FutsRspError("指定的管理员不存在");
                    }
                    BasicTracker.UIAccessTracker.UpdateAgentPermission(managerid, accessid);

                }
                else
                {
                    throw new FutsRspError("无权更新代理权限设置");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }
    }
}
