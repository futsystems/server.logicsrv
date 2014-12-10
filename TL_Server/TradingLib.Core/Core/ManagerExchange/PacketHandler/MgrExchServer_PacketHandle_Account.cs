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

        #region Account


        /// <summary>
        /// @请求添加交易帐户
        /// 服务端操作采用如下方式进行
        /// 1.权限常规检查
        /// 2.执行操作时内部通过FutsRspErro抛出异常的方式 外层通过捕获异常来将异常信息回报给客户端
        /// 
        /// 添加帐户的操作最终会触发新增帐号操作，新增帐号事件会将帐户通知给所有有权限查看的管理端
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnMGRAddAccount(MGRAddAccountRequest request, ISession session, Manager manager)
        {
            try
            {
                debug(string.Format("管理员:{0} 请求添加交易帐号:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
                
                //域帐户数目检查
                if (manager.Domain.GetAccounts().Count() >= manager.Domain.AccLimit)
                {
                    throw new FutsRspError("帐户数目达到上限:" + manager.Domain.AccLimit.ToString());
                }

                //如果不是Root权限的Manager需要进行执行权限检查
                if (!manager.RightRootDomain())
                {
                    //如果不是为该主域添加帐户,则我们需要判断当前Manager的主域是否拥有请求主域的权限
                    if (manager.GetBaseMGR() != request.MgrID)
                    {
                        if (!manager.RightAgentParent(request.MgrID))
                        {
                            throw new FutsRspError("无权在该管理域开设帐户");
                        }
                    }
                }

                //Manager帐户数量限制 如果是在自己的主域中添加交易帐户 则需要检查帐户数量
                int limit = manager.BaseManager.AccLimit;

                int cnt = manager.GetVisibleAccount().Count();//获得该manger下属的所有帐户数目
                if (cnt >= limit)
                {
                    throw new FutsRspError("可开帐户数量超过限制:" + limit.ToString());
                }


                AccountCreation create = new AccountCreation();
                create.Account = request.AccountID;
                create.Category = request.Category;
                create.Password = request.Password;
                create.RouteGroup = BasicTracker.RouterGroupTracker[request.RouterGroup_ID];
                create.RouterType = QSEnumOrderTransferType.SIM;
                create.UserID = request.UserID;
                create.Domain = manager.Domain;
                create.BaseManager = manager.BaseManager;


                //执行操作 并捕获异常 产生异常则给出错误回报
                clearcentre.AddAccount(ref create);//将交易帐户加入到主域
                session.OperationSuccess("新增帐户:" + create.Account + "成功");
            }
            catch (FutsRspError ex)//捕获到FutsRspError则向管理端发送对应回报
            {
                session.OperationError(ex);
            }
        }

        /// <summary>
        /// 请求删除交易帐户
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnDelAccount(MGRReqDelAccountRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求删除帐户:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            try
            {
                clearcentre.DelAccount(request.AccountToDelete);

                session.OperationSuccess("交易帐户:" + request.AccountToDelete + " 删除成功");
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }

        }

        /// <summary>
        /// 查询交易帐号列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnMGRQryAccount(MGRQryAccountRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求下载交易帐户列表:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IAccount[] list = manager.GetVisibleAccount().ToArray();
            if (list.Length > 0)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    RspMGRQryAccountResponse response = ResponseTemplate<RspMGRQryAccountResponse>.SrvSendRspResponse(request);
                    response.oAccount = list[i].ToAccountLite();
                    CacheRspResponse(response, i == list.Length - 1);


                    TrdClientInfo client = exchsrv.FirstClientInfoForAccount(list[i].ID);
                    if (client != null)
                    {
                        NotifyMGRSessionUpdateNotify notify = ResponseTemplate<NotifyMGRSessionUpdateNotify>.SrvSendRspResponse(request);
                        notify.TradingAccount = client.Account;
                        notify.IsLogin = client.Authorized;
                        notify.IPAddress = client.IPAddress;
                        notify.HardwarCode = client.HardWareCode;
                        notify.ProductInfo = client.ProductInfo;
                        notify.FrontID = client.Location.FrontID;
                        notify.ClientID = client.Location.ClientID;

                        CachePacket(notify);
                    }
                }
            }
            else
            {
                RspMGRQryAccountResponse response = ResponseTemplate<RspMGRQryAccountResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);

            }
        }


        /// <summary>
        /// 设定观察交易帐号列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnMGRWatchAccount(MGRWatchAccountRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求设定观察列表:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            CustInfoEx c = customerExInfoMap[request.ClientID];
            c.Watch(request.AccountList);
        }

        void SrvOnMGRResumeAccount(MGRResumeAccountRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求恢复交易数据,帐号:{1}", session.MGRLoginName, request.ResumeAccount), QSEnumDebugLevel.INFO);
            //判断权限

            //将请求放入队列等待处理
            CustInfoEx c = customerExInfoMap[request.ClientID];
            c.Selected(request.ResumeAccount);//保存管理客户端选中的交易帐号
            _resumecache.Write(request);
        }

        /// <summary>
        /// 处理管理端的帐户查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnMGRQryAccountInfo(MGRQryAccountInfoRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询交易帐户信息,帐号:{1}", session.MGRLoginName, request.Account), QSEnumDebugLevel.INFO);

            IAccount account = clearcentre[request.Account];

            if (account != null)
            {
                RspMGRQryAccountInfoResponse response = ResponseTemplate<RspMGRQryAccountInfoResponse>.SrvSendRspResponse(request);
                response.AccountInfoToSend = account.ToAccountInfo();
                CachePacket(response);
            }
        }

        void SrvOnMGRCashOperation(MGRCashOperationRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求出入金操作:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IAccount account = clearcentre[request.Account];
            if (account != null)
            {
                try
                {
                    clearcentre.CashOperation(request.Account, request.Amount, request.TransRef, request.Comment);

                }
                catch (FutsRspError ex)
                {
                    RspMGROperationResponse response = ResponseTemplate<RspMGROperationResponse>.SrvSendRspResponse(request);
                    response.RspInfo.ErrorID = ex.ErrorID;
                    response.RspInfo.ErrorMessage = ex.ErrorMessage;

                    CachePacket(response);
                    return;
                }

                //出入金操作后返回帐户信息更新
                RspMGRQryAccountInfoResponse notify = ResponseTemplate<RspMGRQryAccountInfoResponse>.SrvSendRspResponse(request);
                notify.AccountInfoToSend = account.ToAccountInfo();
                CachePacket(notify);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnMGRUpdateAccountCategory(MGRUpdateCategoryRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求更新帐户类别:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IAccount account = clearcentre[request.Account];
            if (account != null)
            {
                clearcentre.UpdateAccountCategory(request.Account, request.Category);
            }
        }

        void SrvOnMGRUpdateAccountExecute(MGRUpdateExecuteRequest request, ISession session, Manager manger)
        {
            debug(string.Format("管理员:{0} 请求交易权限类别:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IAccount account = clearcentre[request.Account];
            if (account != null)
            {
                if (request.Execute && !account.Execute)
                {
                    clearcentre.ActiveAccount(request.Account);
                }
                if (!request.Execute && account.Execute)
                {
                    clearcentre.InactiveAccount(request.Account);
                }

            }
        }

        void SrvOnMGRUpdateAccountIntraday(MGRUpdateIntradayRequest request, ISession session, Manager manger)
        {
            debug(string.Format("管理员:{0} 请求更新日内交易:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IAccount account = clearcentre[request.Account];
            if (account != null)
            {
                clearcentre.UpdateAccountIntradyType(request.Account, request.Intraday);
            }
        }

        void SrvOnMGRUpdateRouteType(MGRUpdateRouteTypeRequest request, ISession session, Manager manger)
        {
            debug(string.Format("管理员:{0} 请求更新路由类被:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IAccount account = clearcentre[request.Account];
            if (account != null)
            {
                clearcentre.UpdateAccountRouterTransferType(request.Account, request.RouteType);
            }
        }


        /// <summary>
        /// @修改交易帐户密码
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manger"></param>
        void SrvOnMGRChangeAccountPassword(MGRChangeAccountPassRequest request, ISession session, Manager manger)
        {
            try
            {
                debug(string.Format("管理员:{0} 请求修改交易密码:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

                IAccount account = clearcentre[request.TradingAccount];
                if (account != null)
                {
                    clearcentre.ChangeAccountPass(request.TradingAccount, request.NewPassword);
                    session.OperationSuccess("修改密码成功");
                }
                else
                {
                    throw new FutsRspError("交易帐户不存在");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }



        void SrvOnMGRReqChangeInvestor(MGRReqChangeInvestorRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求修改投资者信息:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IAccount account = clearcentre[request.TradingAccount];
            if (account != null)
            {
                clearcentre.UpdateInvestorInfo(request.TradingAccount, request.Name, request.Broker, request.BankFK, request.BankAC);
            }
        }

        void SrvOnMGRReqUpdateAccountPosLock(MGRReqUpdatePosLockRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求修改帐户锁仓权限:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IAccount account = clearcentre[request.TradingAccount];
            if (account != null)
            {
                clearcentre.UpdateAccountPosLock(request.TradingAccount, request.PosLock);
            }
        }


        #endregion

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountInfo", "QryAccountInfo - query account", "查询帐户信息")]
        public void CTE_QryDomain(ISession session,string account)
        {
            try
            {
                Manager manager = session.GetManager();
                IAccount acc = clearcentre[account];
                if (manager.RightAccessAccount(acc))
                {
                    session.SendJsonReplyMgr(acc.ToAccountInfo());
                }
                else
                {
                    throw new FutsRspError("无权查看该帐户信息");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountRouterGroup", "UpdateAccountRouterGroup - update account router group", "更新帐户路由组信息")]
        public void CTE_QryDomain(ISession session,string account,int gid)
        {
            try
            {
                Manager manager = session.GetManager();
                if (!manager.RightRootDomain())
                {
                    throw new FutsRspError("无权修改帐户路由组设置");
                }

                IAccount acc = clearcentre[account];
                if (acc == null)
                {
                    throw new FutsRspError("交易帐户不存在");
                }

                if (!manager.RightAccessAccount(acc))
                {
                    throw new FutsRspError("无权修改该交易帐户");
                }

                RouterGroup rg = manager.Domain.GetRouterGroup(gid);
                if (rg == null)
                {
                    throw new FutsRspError("指定路由组不存在");
                }

                //更新路由组
                clearcentre.UpdateRouterGroup(account, rg);
                session.OperationSuccess("更新帐户路由组成功");



            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

    }
}
