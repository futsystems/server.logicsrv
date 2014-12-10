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



        void SrvOnMGRQryManager(MGRQryManagerRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询管理员列表:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            //获得当前管理员可以查看的柜员列表
            Manager[] mgrs = BasicTracker.ManagerTracker.GetManagers(manager).ToArray();
            if (mgrs.Length > 0)
            {
                for (int i = 0; i < mgrs.Length; i++)
                {
                    RspMGRQryManagerResponse response = ResponseTemplate<RspMGRQryManagerResponse>.SrvSendRspResponse(request);
                    response.ManagerToSend = mgrs[i];
                    CacheRspResponse(response, i == mgrs.Length - 1);
                }
            }
            else
            {
                RspMGRQryManagerResponse response = ResponseTemplate<RspMGRQryManagerResponse>.SrvSendRspResponse(request);
                response.ManagerToSend = new Manager();
                CacheRspResponse(response);
            }


        }

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

                Manager m = request.ManagerToSend;
                //1.添加的Manager的父代理为当前管理员的mgr_fk 
                m.parent_fk = manager.mgr_fk;
                m.domain_id = manager.domain_id;
                //只有添加代理用户时才从数据库创建主域ID MgrFK,其余员工角色和当前管理员的主域ID一致
                if (m.Type != QSEnumManagerType.AGENT && m.Type != QSEnumManagerType.ROOT)
                {
                    m.mgr_fk = manager.mgr_fk;
                }
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
                response.ManagerToSend = m;
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
            debug(string.Format("管理员:{0} 请求更新管理员:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            //更新管理员 如果管理不存在则添加新的管理员帐户 如果存在则进行参数更新
            Manager m = request.ManagerToSend;
            BasicTracker.ManagerTracker.UpdateManager(m);

            //通知直接请求的管理端
            RspMGRQryManagerResponse response = ResponseTemplate<RspMGRQryManagerResponse>.SrvSendRspResponse(request);
            response.ManagerToSend = m;
            CacheRspResponse(response);

            //通知管理员信息变更
            NotifyManagerUpdate(m);
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

    }
}
