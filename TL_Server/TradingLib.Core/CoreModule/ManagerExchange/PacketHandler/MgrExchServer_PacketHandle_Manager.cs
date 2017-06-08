//Copyright 2013 by FutSystems,Inc.
//20170112 整理无用操作

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 管理员
    /// </summary>
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
            Manager[] mgrs = manager.GetVisibleManager().ToArray();
            session.ReplyMgr(mgrs);
        }

        /// <summary>
        /// 查询分区管理员信息
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryManagerLoginInfo", "QryManagerLoginInfo - query manager logininfo", "查看管理员密码")]
        public void CTE_QryDomainRootLoginInfo(ISession session, int mgrid)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {

                Manager mgr = BasicTracker.ManagerTracker[mgrid];
                if (manager.RightAccessManager(mgr))
                {
                    Protocol.LoginInfo logininfo = new Protocol.LoginInfo();
                    logininfo.LoginID = mgr.Login;
                    logininfo.Pass = ORM.MManager.GetManagerPass(mgr.Login);
                    session.ReplyMgr(logininfo);
                }
                else
                {
                    throw new FutsRspError("无权查看柜员信息");
                }
            }
        }

        /// <summary>
        /// 更新柜员
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateManager", "UpdateManager - update manger", "更新或添加柜员", QSEnumArgParseType.Json)]
        public void CTE_UpdateManger(ISession session, string json)
        {
            Manager manager = session.GetManager();

            var data = json.DeserializeObject();

            var id=int.Parse(data["id"].ToString());
            var login = data["login"].ToString();
            var mgr_type = data["mgr_type"].ToString().ParseEnum<QSEnumManagerType>();
            var name = data["name"].ToString();
            var mobile = data["mobile"].ToString();
            var qq = data["qq"].ToString();
         

            ManagerSetting m = new ManagerSetting();
            m.ID = id;
            m.Login = login;
            m.Type = mgr_type;
            m.Name = name;
            m.Mobile = mobile;
            m.QQ = qq;

            if (mgr_type == QSEnumManagerType.AGENT)
            {
                var acc_limit = int.Parse(data["acc_limit"].ToString());
                var agent_limit = int.Parse(data["agent_limit"].ToString());
                
                m.AccLimit = acc_limit;
                m.AgentLimit = agent_limit;
            }

            if (id == 0)
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
                
                //验证添加柜员帐户权限
                manager.ValidRightAddManager(m);

                int maxcnt = Math.Min(manager.Domain.AgentLimit, manager.AgentLimit);
                int cnt = manager.GetVisibleManager().Count();
                if (cnt >= maxcnt)
                {
                    throw new FutsRspError("可开柜员数量超过限制:" + maxcnt.ToString());
                }

                if (BasicTracker.ManagerTracker[m.Login] != null)
                {
                    throw new FutsRspError("柜员登入ID不能重复:" + m.Login);
                }
                if (m.Login.Contains("root"))
                {
                    throw new FutsRspError("系统保留字段root,不能用柜员登入名");
                }
                if (m.Login.Contains("admin"))
                {
                    throw new FutsRspError("系统保留字段admin,不能用柜员登入名");
                }

                BasicTracker.ManagerTracker.UpdateManager(m);
                var newManager = BasicTracker.ManagerTracker[m.ID];

                if (m.Type == QSEnumManagerType.AGENT)
                {
                    var agent_type = data["agent_type"].ToString().ParseEnum<EnumAgentType>();

                    AgentSetting agent = new AgentSetting();
                    agent.Account = m.Login;
                    agent.Currency = CurrencyType.RMB;
                    agent.AgentType = agent_type;

                    BasicTracker.AgentTracker.UpdateAgent(agent);
                    var a = BasicTracker.AgentTracker[agent.ID];
                    if (a != null)
                    {
                        a.BindManager(newManager);
                    }
                }
                session.RspMessage("添加管理员成功");
                //通知管理员信息变更
                NotifyManagerUpdate(newManager);

            }
            else
            {
                Manager target = BasicTracker.ManagerTracker[id];
                if (target == null)
                {
                    throw new FutsRspError("管理员不存在");
                }

                BasicTracker.ManagerTracker.UpdateManager(m);

                session.RspMessage("更新管理员成功");
                //通知管理员信息变更
                NotifyManagerUpdate(target);
            }
        }


        /// <summary>
        /// 删除管理员
        /// 删除管理员 删除管理员下面所有代理 以及代理下面的所有交易帐户
        /// </summary>
        /// <param name="session"></param>
        /// <param name="mgr_id"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeleteManager", "DeleteManager - delete manger", "删除柜员", QSEnumArgParseType.CommaSeparated)]
        public void CTE_DelManager(ISession session, int mgr_id)
        {
            Manager manager = session.GetManager();
            logger.Info(string.Format("管理员:{0} 删除管理员 id:{1}", manager.Login, mgr_id));

            Manager remove = BasicTracker.ManagerTracker[mgr_id];
            if (remove == null)
            {
                throw new FutsRspError(string.Format("管理员:{0}不存在", mgr_id));
            }

            if (!manager.RightAccessManager(remove))
            {
                throw new FutsRspError(string.Format("无权删除管理员:{0}", mgr_id));
            }

            //查看该manger下的所有代理
            List<Manager> mgrlist = remove.GetVisibleManager().ToList();

            foreach (var mgr in mgrlist)
            {
                //删除管理员下的帐户
                List<IAccount> acclist = TLCtxHelper.ModuleAccountManager.Accounts.Where(acc => acc.Mgr_fk == mgr.ID).ToList();
                foreach (var acc in acclist)
                {
                    TLCtxHelper.ModuleAccountManager.DelAccount(acc.ID);
                }

                Util.sleep(500);
                //删除管理员 如果不执行等待 则超级管理员会无法获得帐户删除通知 等待1秒后帐户删除通知会正常的送达所有管理员
                BasicTracker.ManagerTracker.DeleteManager(mgr);
                NotifyManagerDelete(mgr);
                Util.sleep(500);
                //注销管理端登入会话
                tl.ClearTerminalsForManager(mgr.Login);
            }
            
            session.RspMessage("删除管理员成功");

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateManagerPass", "UpdateManagerPass - update manager password", "修改柜员密码")]
        public void CTE_UpdateMangerPassword(ISession session,string oldpass,string newpass)
        {
            Manager manager = session.GetManager();
            if(manager.Pass.Equals(oldpass))
            {
                ORM.MManager.UpdateManagerPass(manager.ID, newpass);
                manager.Pass = newpass;
                session.RspMessage("密码修改成功");
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
                if (!mgr.IsParentOf(tomanger))
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
                if (!mgr.IsParentOf(tomanger))
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
