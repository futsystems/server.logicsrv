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
            Manager[] mgrs = BasicTracker.ManagerTracker.GetManagers(manager).ToArray();
            session.ReplyMgr(mgrs);
        }

        /// <summary>
        /// 更新柜员
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateManager", "UpdateManager - update manger", "更新或添加柜员")]
        public void CTE_UpdateManger(ISession session, string json)
        { 
            
        }

        //void SrvOnMGRQryManager(MGRQryManagerRequest request, ISession session, Manager manager)
        //{
        //    debug(string.Format("管理员:{0} 请求查询管理员列表:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

        //    //获得当前管理员可以查看的柜员列表
        //    Manager[] mgrs = BasicTracker.ManagerTracker.GetManagers(manager).ToArray();
        //    if (mgrs.Length > 0)
        //    {
        //        for (int i = 0; i < mgrs.Length; i++)
        //        {
        //            RspMGRQryManagerResponse response = ResponseTemplate<RspMGRQryManagerResponse>.SrvSendRspResponse(request);
        //            response.ManagerToSend = mgrs[i];
        //            CacheRspResponse(response, i == mgrs.Length - 1);
        //        }
        //    }
        //    else
        //    {
        //        RspMGRQryManagerResponse response = ResponseTemplate<RspMGRQryManagerResponse>.SrvSendRspResponse(request);
        //        response.ManagerToSend = new Manager();
        //        CacheRspResponse(response);
        //    }
        //}

        /// <summary>
        /// Manager添加的代理的MgrFK为数据ID ParentFK为当前MgrFK
        /// Manager添加的其他角色MgrFK为当前MgrFK ParentFK为当前MgrFK
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnMGRAddManger(MGRReqAddManagerRequest request, ISession session, Manager manager)
        {
            try
            {
                debug(string.Format("管理员:{0} 请求添加管理员:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

                //开启代理模块 并且 是管理员 或者 是代理同时可以开设下级代理
                if (!(manager.Domain.Module_Agent && (manager.IsRoot() || (manager.IsAgent() && manager.Domain.Module_SubAgent))))
                {
                    throw new FutsRspError("无权开设下级代理");
                }

                Manager m = request.ManagerToSend;

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

                RspMGRQryManagerResponse response = ResponseTemplate<RspMGRQryManagerResponse>.SrvSendRspResponse(request);
                response.ManagerToSend = BasicTracker.ManagerTracker[m.ID];
                CacheRspResponse(response);
                session.OperationSuccess("添加管理员成功");
                //通知管理员信息变更
                NotifyManagerUpdate(m);

            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        void SrvOnMGRUpdateManger(MGRReqUpdateManagerRequest request, ISession session, Manager manger)
        {
            try
            {
                debug(string.Format("管理员:{0} 请求更新管理员:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

                

                //更新管理员 如果管理不存在则添加新的管理员帐户 如果存在则进行参数更新
                Manager m = request.ManagerToSend;
                if (!manger.RightAddManager(m))
                {
                    throw new FutsRspError("无权添加管理员类型:" + Util.GetEnumDescription(m.Type));
                }

                BasicTracker.ManagerTracker.UpdateManager(m);

                //通知直接请求的管理端
                RspMGRQryManagerResponse response = ResponseTemplate<RspMGRQryManagerResponse>.SrvSendRspResponse(request);
                response.ManagerToSend = m;
                CacheRspResponse(response);

                //通知管理员信息变更
                NotifyManagerUpdate(m);
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        void SrvOnMGRUpdatePass(MGRUpdatePassRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求修改密码:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            if (ORM.MManager.ValidManager(manager.Login, request.OldPass))
            {
                ORM.MManager.UpdateManagerPass(manager.ID, request.NewPass);
                RspMGROperationResponse response = ResponseTemplate<RspMGROperationResponse>.SrvSendRspResponse(request);
                response.RspInfo.ErrorMessage = "密码修改成功";
                CacheRspResponse(response);
            }
            else
            {
                RspMGROperationResponse response = ResponseTemplate<RspMGROperationResponse>.SrvSendRspResponse(request);
                response.RspInfo.Fill("MGR_PASS_ERROR");
                CacheRspResponse(response);
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ActiveManager", "ActiveManager - query bank", "查询银行列表")]
        public void CTE_ActiveManger(ISession session,int mgrid)
        {
            try
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
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "InactiveManager", "InactiveManager - query bank", "查询银行列表")]
        public void CTE_InactiveManger(ISession session, int mgrid)
        {
            try
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
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }
    }
}
