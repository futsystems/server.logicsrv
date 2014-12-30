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
        /// 查询柜员
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryManager", "QryManager - query manger", "查询柜员列表")]
        public void CTE_QryManager(ISession session)
        {
            Manager manager = session.GetManager();
            //获得当前管理员可以查看的柜员列表
            Manager[] mgrs = manager.GetVisibleManager().ToArray();// BasicTracker.ManagerTracker.GetManagers(manager).ToArray();
            session.ReplyMgr(mgrs);
        }

        /// <summary>
        /// 更新柜员
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateManager", "UpdateManager - update manger", "更新或添加柜员",true)]
        public void CTE_UpdateManger(ISession session, string json)
        {
            Manager manager = session.GetManager();

            ManagerSetting m = Mixins.LitJson.JsonMapper.ToObject<ManagerSetting>(json);
            bool isadd = m.ID == 0;
            if (isadd)
            {
                //开启代理模块 并且 是管理员 或者 是代理同时可以开设下级代理
                if (!(manager.Domain.Module_Agent && (manager.IsRoot() || (manager.IsAgent() && manager.Domain.Module_SubAgent))))
                {
                    throw new FutsRspError("无权开设下级代理");
                }
                //父MangerID 柜员的父管理域是当前柜员管理域 Root除外,Root的父柜员为自己
                m.parent_fk = manager.BaseMgrID;
                //管理域ID 默认添加的管理员的管理域ID与当前管理员管理域ID一致(风控员,财务人员等) 代理与Root除外 他们有独立的管理域 
                m.mgr_fk = manager.BaseMgrID;
                //分区ID
                m.domain_id = manager.domain_id;

                if (!manager.RightAddManager(m))
                {
                    throw new FutsRspError("无权添加管理员类型:" + Util.GetEnumDescription(m.Type));
                }
                if (BasicTracker.ManagerTracker[m.Login] != null)
                {
                    throw new FutsRspError("柜员登入ID不能重复:" + m.Login);
                }
                if (m.Login.StartsWith("root"))
                {
                    throw new FutsRspError("系统保留字段root,不能用柜员登入名");
                }
                BasicTracker.ManagerTracker.UpdateManager(m);

                session.OperationSuccess("添加管理员成功");
                //通知管理员信息变更
                NotifyManagerUpdate(BasicTracker.ManagerTracker[m.ID]);
            }
            else
            {
                Manager target = BasicTracker.ManagerTracker[m.ID];
                if (target == null)
                {
                    throw new FutsRspError("管理员不存在");
                }

                BasicTracker.ManagerTracker.UpdateManager(m);
                //通知管理员信息变更
                NotifyManagerUpdate(target);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateManagerPass", "UpdateManagerPass - update manager password", "修改柜员密码")]
        public void CTE_UpdateMangerPassword(ISession session,string oldpass,string newpass)
        {
            Manager manager = session.GetManager();
           
            if (ORM.MManager.ValidManager(manager.Login, oldpass))
            {
                ORM.MManager.UpdateManagerPass(manager.ID, newpass);
                session.OperationSuccess("密码修改成功");
            }
            else
            {
                throw new FutsRspError("旧密码错误");
            }

        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ActiveManager", "ActiveManager - query bank", "查询银行列表")]
        public void CTE_ActiveManger(ISession session,int mgrid)
        {
            Manager mgr = session.GetManager();
            if (mgr.IsRoot() || mgr.IsAgent())
            {
                Manager tomanger = BasicTracker.ManagerTracker[mgrid];
                if (tomanger == null)
                {
                    throw new FutsRspError("指定管理员不存在");
                }

                //
                if (!mgr.RightAgentParent(tomanger))
                {
                    throw new FutsRspError("无权操作管理员");
                }

                tomanger.Active = true;
                ORM.MManager.UpdateManagerActive(mgrid, true);

                //通知管理员信息变更
                NotifyManagerUpdate(tomanger);

            }
            else
            {
                throw new FutsRspError("无权操作管理员");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "InactiveManager", "InactiveManager - query bank", "查询银行列表")]
        public void CTE_InactiveManger(ISession session, int mgrid)
        {
            Manager mgr = session.GetManager();
            if (mgr.IsRoot() || mgr.IsAgent())
            {
                Manager tomanger = BasicTracker.ManagerTracker[mgrid];
                if (tomanger == null)
                {
                    throw new FutsRspError("指定管理员不存在");
                }

                //
                if (!mgr.RightAgentParent(tomanger))
                {
                    throw new FutsRspError("无权操作管理员");
                }

                tomanger.Active = false;
                ORM.MManager.UpdateManagerActive(mgrid, false);

                //通知管理员信息变更
                NotifyManagerUpdate(tomanger);

            }
            else
            {
                throw new FutsRspError("无权操作管理员");
            }
        }
    }
}
