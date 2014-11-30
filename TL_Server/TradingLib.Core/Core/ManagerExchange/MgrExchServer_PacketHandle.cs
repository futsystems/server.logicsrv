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
        void TokenLast(int i, int len, RspResponsePacket response)
        {
            if (i == len - 1)
            {
                response.IsLast = true;
            }
            else
            {
                response.IsLast = false;
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
                    response.oAccount =list[i].ToAccountLite();
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

        void SrvOnMGROpenClearCentre(MGRReqOpenClearCentreRequest request, ISession session, Manager manger)
        {
            debug(string.Format("管理员:{0} 请求开启清算中心:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            clearcentre.OpenClearCentre();
            
        }

        void SrvOnMGRCloseClearCentre(MGRReqCloseClearCentreRequest request, ISession session, Manager manger)
        {
            debug(string.Format("管理员:{0} 请求关闭清算中心:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            clearcentre.CloseClearCentre();

        }

        void SrvOnMGRQryConnector(MGRQryConnectorRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询通道列表:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            List<RspMGRQryConnectorResponse> responselist = new List<RspMGRQryConnectorResponse>();
            foreach (IBroker b in TLCtxHelper.Ctx.RouterManager.Brokers)
            {
                RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
                response.Connector = new ConnectorInfo(b);
                responselist.Add(response);
            }

            foreach (IDataFeed d in TLCtxHelper.Ctx.RouterManager.DataFeeds)
            {
                RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
                response.Connector = new ConnectorInfo(d);
                responselist.Add(response);
            }

            int totalnum = responselist.Count;
            if (totalnum>0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    CacheRspResponse(responselist[i], i == totalnum - 1);
                }
            }
            else
            { 
                
            }
            
        }

        void SrvOnMGRStartBroker(MGRReqStartBrokerRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求启动成交通道:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IBroker b = TLCtxHelper.Ctx.RouterManager.FindBroker(request.FullName);
            if (b != null && !b.IsLive)
            {
                string msg = string.Empty;
                bool success = b.Start(out msg);
                if (success)
                {
                    session.OperationSuccess(string.Format("交易通道[{0}]已启动", b.Token));
                }
                else
                {
                    session.OperationError(new FutsRspError(msg));
                }
            }

            RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
            response.Connector = new ConnectorInfo(b);
            CachePacket(response);
        }

        void SrvOnMGRStopBroker(MGRReqStopBrokerRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求停止成交通道:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IBroker b = TLCtxHelper.Ctx.RouterManager.FindBroker(request.FullName);
            if (b != null && b.IsLive)
            {
                //string msg = string.Empty;
                //bool success = b.Start(out msg);
                //if (success)
                //{
                //    session.OperationSuccess(string.Format("交易通道[{0}]已启动", b.Token));
                //}
                //else
                //{
                //    session.OperationError(new FutsRspError(msg));
                //}

                b.Stop();
                session.OperationSuccess(string.Format("交易通道[{0}]已停止", b.Token));
            }

            RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
            response.Connector = new ConnectorInfo(b);
            CachePacket(response);
        }
        void SrvOnMGRStartDataFeed(MGRReqStartDataFeedRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求启动行情通道:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            IDataFeed d = TLCtxHelper.Ctx.RouterManager.FindDataFeed(request.FullName);

            if (d != null && !d.IsLive)
            {
                d.Start();
                session.OperationSuccess(string.Format("行情通道[{0}]已启动", d.Token));
            }

            RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
            response.Connector = new ConnectorInfo(d);
            CachePacket(response);
        }
        void SrvOnMGRStopDataFeed(MGRReqStopDataFeedRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求停止行情通道:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IDataFeed d = TLCtxHelper.Ctx.RouterManager.FindDataFeed(request.FullName);
            if (d != null && d.IsLive)
            {
                d.Stop();
                session.OperationSuccess(string.Format("行情通道[{0}]已停止", d.Token));
            }
            RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
            response.Connector = new ConnectorInfo(d);
            CachePacket(response);
        }

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
                    else
                    {
                        //如果是在自己的主域中添加交易帐户 则需要检查帐户数量
                        int limit = manager.BaseManager.AccLimit;
                        
                        int cnt = TLCtxHelper.CmdAccount.Accounts.Where(acc => acc.Mgr_fk == manager.GetBaseMGR()).Count();
                        if (cnt > limit)
                        {
                            throw new FutsRspError("可开帐户数量超过限制:" + limit.ToString());
                        }
                    }
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

        void SrvOnMGRQryExchange(MGRQryExchangeRequuest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询交易所列表:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IExchange[] exchs = BasicTracker.ExchagneTracker.Exchanges;

            int totalnum = exchs.Length;
            
            for (int i = 0; i < totalnum; i++)
            {
                RspMGRQryExchangeResponse response = ResponseTemplate<RspMGRQryExchangeResponse>.SrvSendRspResponse(request);
                response.Exchange = exchs[i] as Exchange;

                CacheRspResponse(response, i == totalnum - 1);
            }
        }

        void SrvOnMGRQryMarketTime(MGRQryMarketTimeRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询交易时间段:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            MarketTime[] mts = BasicTracker.MarketTimeTracker.MarketTimes;
            int totalnum = mts.Length;
            for (int i = 0; i < totalnum; i++)
            {
                RspMGRQryMarketTimeResponse response = ResponseTemplate<RspMGRQryMarketTimeResponse>.SrvSendRspResponse(request);
                response.MarketTime = mts[i];

                CacheRspResponse(response, i == totalnum - 1);
            }
        }

        void SrvOnMGRQrySecurity(MGRQrySecurityRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询品种:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            SecurityFamily[] seclist = BasicTracker.SecurityTracker.Securities;
            int totalnum = seclist.Length;
            //debug("security totalnum:" + totalnum, QSEnumDebugLevel.INFO);
            for (int i = 0; i < totalnum; i++)
            {
                
                RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
                response.SecurityFaimly = seclist[i] as SecurityFamilyImpl;
                //debug("sec:" + response.ToString(), QSEnumDebugLevel.INFO);
                CacheRspResponse(response, i == totalnum - 1);
            }
        }

        void SrvOnMGRUpdateSecurity(MGRUpdateSecurityRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求更新品种:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            SecurityFamilyImpl sec = request.SecurityFaimly;
            BasicTracker.SecurityTracker.UpdateSecurity(sec);
            RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
            response.SecurityFaimly = BasicTracker.SecurityTracker[sec.ID] as SecurityFamilyImpl;
            CacheRspResponse(response);

            if (sec.Tradeable)
            {
                exchsrv.RegisterSymbol(BasicTracker.SecurityTracker[sec.Code]);
            }
        }

        void SrvOnMGRQrySymbol(MGRQrySymbolRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询合约:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            Symbol[] symlis = BasicTracker.SymbolTracker.Symbols;
            int totalnum = symlis.Length;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspMGRQrySymbolResponse response = ResponseTemplate<RspMGRQrySymbolResponse>.SrvSendRspResponse(request);
                    response.Symbol = symlis[i] as SymbolImpl;

                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspMGRQrySymbolResponse response = ResponseTemplate<RspMGRQrySymbolResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }

        void SrvOnMGRUpdateSymbol(MGRUpdateSymbolRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求更新合约:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            SymbolImpl sym = request.Symbol;
            BasicTracker.SymbolTracker.UpdateSymbol(sym);
            RspMGRQrySymbolResponse response = ResponseTemplate<RspMGRQrySymbolResponse>.SrvSendRspResponse(request);
            response.Symbol = BasicTracker.SymbolTracker[sym.Symbol] as SymbolImpl;
            CacheRspResponse(response);
            if (sym.Tradeable)
            {
                exchsrv.RegisterSymbol(sym.Symbol);
            }
        }

        void SrvOnMGRQryRuleSet(MGRQryRuleSetRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询风控规则:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            RuleClassItem[] items = riskcentre.GetRuleClassItems();
            int totalnum = items.Length;

            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspMGRQryRuleSetResponse response = ResponseTemplate<RspMGRQryRuleSetResponse>.SrvSendRspResponse(request);
                    response.RuleClassItem = items[i];
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspMGRQryRuleSetResponse response = ResponseTemplate<RspMGRQryRuleSetResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }

        void SrvOnMGRUpdateRule(MGRUpdateRuleRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求更新风控规则:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            RuleItem item = request.RuleItem;
            riskcentre.UpdateRule(item);
            RspMGRUpdateRuleResponse response = ResponseTemplate<RspMGRUpdateRuleResponse>.SrvSendRspResponse(request);
            response.RuleItem = item;
            CacheRspResponse(response);
        }

        void SrvOnMGRQryRuleItem(MGRQryRuleItemRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询帐户分控规则列表:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            //RuleItem[] items = ORM.MRuleItem.SelectRuleItem(request.Account, QSEnumRuleType.OrderRule).ToArray();
            List<RuleItem> items = new List<RuleItem>();
            IAccount account = clearcentre[request.Account];
            if (account != null)
            {
                if (!account.RuleItemLoaded)
                {
                    riskcentre.LoadRuleItem(account);//风控规则延迟加载,如果帐户没有加载则先加载帐户风控规则
                }
                //从内存风控实例 生成ruleitem
                if (request.RuleType == QSEnumRuleType.OrderRule)
                {
                    foreach (IOrderCheck oc in account.OrderChecks)
                    {
                        items.Add(RuleItem.IRule2RuleItem(oc));
                    }
                }
                else if (request.RuleType == QSEnumRuleType.AccountRule)
                {
                    foreach (IAccountCheck ac in account.AccountChecks)
                    {
                        items.Add(RuleItem.IRule2RuleItem(ac));
                    }
                }

                int totalnum = items.Count;
                if (totalnum > 0)
                {
                    for (int i = 0; i < totalnum; i++)
                    {
                        RspMGRQryRuleItemResponse response = ResponseTemplate<RspMGRQryRuleItemResponse>.SrvSendRspResponse(request);
                        response.RuleItem = items[i];

                        CacheRspResponse(response, i == totalnum - 1);
                    }
                }
                else
                {
                    RspMGRQryRuleItemResponse response = ResponseTemplate<RspMGRQryRuleItemResponse>.SrvSendRspResponse(request);
                    CacheRspResponse(response);
                }
            }
        }

        void SrvOnMGRDelRuleItem(MGRDelRuleItemRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求删除风控项:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            riskcentre.DeleteRiskRule(request.RuleItem);
            RspMGRDelRuleItemResponse response = ResponseTemplate<RspMGRDelRuleItemResponse>.SrvSendRspResponse(request);
            response.RuleItem = request.RuleItem;

            CacheRspResponse(response);

        }

        void SrvOnMGRQrySystemStatus(MGRQrySystemStatusRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询系统状态:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            RspMGRQrySystemStatusResponse response = ResponseTemplate<RspMGRQrySystemStatusResponse>.SrvSendRspResponse(request);

            SystemStatus status = new SystemStatus();
            status.CurrentTradingday = TLCtxHelper.Ctx.SettleCentre.CurrentTradingday;
            status.IsClearCentreOpen = clearcentre.Status == QSEnumClearCentreStatus.CCOPEN;
            status.IsSettleNormal = TLCtxHelper.Ctx.SettleCentre.IsNormal;
            status.IsTradingday = TLCtxHelper.Ctx.SettleCentre.IsTradingday;
            status.LastSettleday = TLCtxHelper.Ctx.SettleCentre.LastSettleday;
            status.NextTradingday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
            status.TotalAccountNum = clearcentre.Accounts.Length;
            status.MarketOpenCheck = TLCtxHelper.Ctx.RiskCentre.MarketOpenTimeCheck;
            status.IsDevMode = GlobalConfig.IsDevelop;
            response.Status = status;

            CacheRspResponse(response);
        }


        void SrvOnMGRQryOrder(MGRQryOrderRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询历史委托:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            IList<Order> orders = ORM.MTradingInfo.SelectHistOrders(request.TradingAccount,request.Settleday, request.Settleday);

            int totalnum = orders.Count;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspMGRQryOrderResponse response = ResponseTemplate<RspMGRQryOrderResponse>.SrvSendRspResponse(request);
                    response.OrderToSend = orders[i];
                    response.OrderToSend.Side = response.OrderToSend.TotalSize > 0 ? true : false;
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                //返回空项目
                RspMGRQryOrderResponse response = ResponseTemplate<RspMGRQryOrderResponse>.SrvSendRspResponse(request);
                response.OrderToSend = new OrderImpl();
                CacheRspResponse(response);
            }
        }

        void SrvnMGRQryTrade(MGRQryTradeRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询历史成交:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            IList<Trade> trades = ORM.MTradingInfo.SelectHistTrades(request.TradingAccount, request.Settleday, request.Settleday);

            int totalnum = trades.Count;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspMGRQryTradeResponse response = ResponseTemplate<RspMGRQryTradeResponse>.SrvSendRspResponse(request);
                    response.TradeToSend = trades[i];
                    response.TradeToSend.Side = response.TradeToSend.xSize > 0 ? true : false;
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                //返回空项目
                RspMGRQryTradeResponse response = ResponseTemplate<RspMGRQryTradeResponse>.SrvSendRspResponse(request);
                response.TradeToSend = new TradeImpl();
                CacheRspResponse(response);
            }
        }

        void SrvOnMGRQryPosition(MGRQryPositionRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询历史持仓:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            //IList<SettlePosition> positions = ORM.MTradingInfo.SelectHistPositions(request.TradingAccount, request.Settleday, request.Settleday);

            //int totalnum = positions.Count;
            //if (totalnum > 0)
            //{
            //    for (int i = 0; i < totalnum; i++)
            //    {
            //        RspMGRQryPositionResponse response = ResponseTemplate<RspMGRQryPositionResponse>.SrvSendRspResponse(request);
            //        response.PostionToSend = positions[i];
            //        CacheRspResponse(response, i == totalnum - 1);
            //    }
            //}
            //else
            //{
            //    //返回空项目
            //    RspMGRQryPositionResponse response = ResponseTemplate<RspMGRQryPositionResponse>.SrvSendRspResponse(request);
            //    response.PostionToSend = new SettlePosition();
            //    CacheRspResponse(response);
            //}
        }



        void SrvOnMGRQryCash(MGRQryCashRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询出入金记录:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            IList<CashTransaction> cts = ORM.MAccount.SelectHistCashTransaction(request.TradingAccount, request.Settleday, request.Settleday);

            int totalnum = cts.Count;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspMGRQryCashResponse response = ResponseTemplate<RspMGRQryCashResponse>.SrvSendRspResponse(request);
                    response.CashTransToSend = cts[i];
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                //返回空项目
                RspMGRQryCashResponse response = ResponseTemplate<RspMGRQryCashResponse>.SrvSendRspResponse(request);
                response.CashTransToSend = new CashTransaction();
                CacheRspResponse(response);
            }
        }

        void SrvOnMGRQrySettlement(MGRQrySettleRequest request, ISession session, Manager manager)
        { 
            debug(string.Format("管理员:{0} 请求查询结算记录:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            IAccount account = clearcentre[request.TradingAccount];

            Settlement settlement = ORM.MSettlement.SelectSettlement(request.TradingAccount, request.Settleday);           
            if (settlement != null)
            {
                List<string> settlelist = SettlementFactory.GenSettlementFile(settlement, account);
                for (int i = 0; i < settlelist.Count; i++)
                {
                    RspMGRQrySettleResponse response = ResponseTemplate<RspMGRQrySettleResponse>.SrvSendRspResponse(request);
                    response.Tradingday = settlement.SettleDay;
                    response.TradingAccount = settlement.Account;
                    response.SettlementContent = settlelist[i]+"\n";
                    CacheRspResponse(response, i == settlelist.Count -1);
                }
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

        void SrvOnMGRReqAddSecurity(MGRReqAddSecurityRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求添加品种:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            SecurityFamilyImpl sec = request.SecurityFaimly;
            if (BasicTracker.SecurityTracker[sec.Code] == null)
            {
                BasicTracker.SecurityTracker.UpdateSecurity(sec);

                RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
                response.SecurityFaimly = BasicTracker.SecurityTracker[sec.Code] as SecurityFamilyImpl;
                CacheRspResponse(response);
            }
            else
            {

                RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
                response.RspInfo.Fill("SECURITY_EXIST");
                CacheRspResponse(response);
            }
            if (sec.Tradeable)
            {
                exchsrv.RegisterSymbol(BasicTracker.SecurityTracker[sec.Code]);
            }
        }

        void SrvOnMGRReqAddSymbol(MGRReqAddSymbolRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求添加合约:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            SymbolImpl symbol = request.Symbol;
            if (BasicTracker.SymbolTracker[symbol.Symbol] == null)
            {
                BasicTracker.SymbolTracker.UpdateSymbol(symbol);

                RspMGRReqAddSymbolResponse response = ResponseTemplate<RspMGRReqAddSymbolResponse>.SrvSendRspResponse(request);
                response.Symbol = BasicTracker.SymbolTracker[symbol.Symbol] as SymbolImpl;
                CacheRspResponse(response);
            }
            else
            {
                RspMGRReqAddSymbolResponse response = ResponseTemplate<RspMGRReqAddSymbolResponse>.SrvSendRspResponse(request);
                response.RspInfo.Fill("SYMBOL_EXIST");
                CacheRspResponse(response);
            }
            if (symbol.Tradeable)
            {
                exchsrv.RegisterSymbol(request.Symbol.Symbol);   
            }
        }

        void SrvOnMGRReqChangeInvestor(MGRReqChangeInvestorRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求修改投资者信息:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IAccount account = clearcentre[request.TradingAccount];
            if (account != null)
            {
                clearcentre.UpdateInvestorInfo(request.TradingAccount, request.Name,request.Broker,request.BankFK,request.BankAC);
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
                     throw new FutsRspError("柜员登入ID不能重复:"+m.Login);
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

        void SrvOnMGRUpdateManger(MGRReqUpdateManagerRequest request,ISession session,Manager manger)
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

        void SrvOnMGRQryAcctService(MGRQryAcctServiceRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询服务:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            //IAccount account = clearcentre[request.TradingAccount];
            //debug("account null:" + (account == null).ToString(), QSEnumDebugLevel.INFO);
            //AccountBase acct = account as AccountBase;
            //if (acct != null)
            //{
            //    debug("got account impl:"+request.TradingAccount, QSEnumDebugLevel.INFO);
            //    IAccountService service = null;
            //    if(acct.GetService(request.ServiceName,out service))
            //    {
            //        debug("got service:" + request.ServiceName,QSEnumDebugLevel.INFO);
            //        RspMGRQryAcctServiceResponse response = ResponseTemplate<RspMGRQryAcctServiceResponse>.SrvSendRspResponse(request);
            //        response.TradingAccount = request.TradingAccount;
            //        response.ServiceName = request.ServiceName;
            //        response.JsonRet = service.QryService();

            //        CacheRspResponse(response);
            //    }
            //    //服务不存在
            //}
            //帐号不存在
        }

        void SrvOnMGRContribRequest(MGRContribRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求扩展命令:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            debug("MGRContrib Request,ModuleID:" + request.ModuleID + " CMDStr:" + request.CMDStr + " Parameters:" + request.Parameters, QSEnumDebugLevel.INFO);
            
            session.ContirbID = request.ModuleID;
            session.CMDStr = request.CMDStr;
            session.RequestID = request.RequestID;

            TLCtxHelper.Ctx.MessageMgrHandler(session, request);
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

        void SrvOnInsertTrade(MGRReqInsertTradeRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求插入委托:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            RspMGROperationResponse response = ResponseTemplate<RspMGROperationResponse>.SrvSendRspResponse(request);

            Trade fill = request.TradeToSend;
            fill.oSymbol = BasicTracker.SymbolTracker[fill.Symbol];
            if (fill.oSymbol == null)
            {
                response.RspInfo.Fill("SYMBOL_NOT_EXISTED");
                CacheRspResponse(response);
                return;
            }
            IAccount account = clearcentre[fill.Account];
            if (account == null)
            {
                response.RspInfo.Fill("交易帐号不存在");
                CacheRspResponse(response);
                return;
            }

            fill.Broker = "Broker.SIM.SIMTrader";

            Order o = new MarketOrder(fill.Symbol, fill.Side, fill.UnsignedSize);

            o.oSymbol = fill.oSymbol;
            o.Account = fill.Account;
            o.Date = fill.xDate;
            o.Time = Util.ToTLTime(Util.ToDateTime(fill.xDate, fill.xTime) - new TimeSpan(0, 0, 1));
            o.Status = QSEnumOrderStatus.Filled;
            o.OffsetFlag = fill.OffsetFlag;
            o.Broker = fill.Broker;

            //委托成交之后
            o.TotalSize = o.Size;
            o.Size = 0;
            o.FilledSize = o.UnsignedSize;
            
            
            


            //注意这里需要获得可用的委托流水和成交流水号
            long ordid = exchsrv.futs_InsertOrderManual(o);
            o.OrderSysID = o.OrderSeq.ToString();
            //o.BrokerKey = 
            
            fill.id = ordid;
            fill.OrderSeq = o.OrderSeq;
            fill.TradeID = "xxxxx";//随机产生的成交编号

            Util.sleep(100);
            exchsrv.futs_InsertTradeManual(fill);

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
        void tl_newPacketRequest(IPacket packet,ISession session,Manager manager)
        {
            switch (packet.Type)
            {
                
                case MessageTypes.MGRQRYACCOUNTS://查询交易帐号列表
                    { 
                        SrvOnMGRQryAccount(packet as MGRQryAccountRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRWATCHACCOUNTS://设定观察交易帐号列表
                    {
                        SrvOnMGRWatchAccount(packet as MGRWatchAccountRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRRESUMEACCOUNT://恢复某个交易帐号日内交易数据
                    {
                        SrvOnMGRResumeAccount(packet as MGRResumeAccountRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYACCOUNTINFO://查询帐户信息
                    {
                        SrvOnMGRQryAccountInfo(packet as MGRQryAccountInfoRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRCASHOPERATION://出入金操作
                    {
                        SrvOnMGRCashOperation(packet as MGRCashOperationRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRUPDATEACCOUNTCATEGORY://更新帐户类别
                    {
                        SrvOnMGRUpdateAccountCategory(packet as MGRUpdateCategoryRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRUPDATEACCOUNTEXECUTE://更新帐户执行权限
                    {
                        SrvOnMGRUpdateAccountExecute(packet as MGRUpdateExecuteRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRUPDATEACCOUNTINTRADAY://更新日内交易权限
                    {
                        SrvOnMGRUpdateAccountIntraday(packet as MGRUpdateIntradayRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRUPDATEACCOUNTROUTETRANSFERTYPE://更新路由类别哦
                    {
                        SrvOnMGRUpdateRouteType(packet as MGRUpdateRouteTypeRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGROPENCLEARCENTRE://请求开启清算中心
                    {
                        SrvOnMGROpenClearCentre(packet as MGRReqOpenClearCentreRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRCLOSECLEARCENTRE://请求关闭清算中心
                    {
                        SrvOnMGRCloseClearCentre(packet as MGRReqCloseClearCentreRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYCONNECTOR://查询通道列表
                    {
                        SrvOnMGRQryConnector(packet as MGRQryConnectorRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRSTARTBROKER://请求启动成交通道
                    {
                        SrvOnMGRStartBroker(packet as MGRReqStartBrokerRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRSTOPBROKER://请求停止成交通道
                    {
                        SrvOnMGRStopBroker(packet as MGRReqStopBrokerRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRSTARTDATAFEED://请求启动行情通道
                    {
                        SrvOnMGRStartDataFeed(packet as MGRReqStartDataFeedRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRSTOPDATAFEED://请求停止行情通道
                    {
                        SrvOnMGRStopDataFeed(packet as MGRReqStopDataFeedRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRADDACCOUNT://请求添加交易帐号
                    {
                        SrvOnMGRAddAccount(packet as MGRAddAccountRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYEXCHANGE://请求查询交易所
                    {
                        SrvOnMGRQryExchange(packet as MGRQryExchangeRequuest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYMARKETTIME://请求查询市场时间段
                    {
                        SrvOnMGRQryMarketTime(packet as MGRQryMarketTimeRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYSECURITY://请求查询品种
                    {
                        SrvOnMGRQrySecurity(packet as MGRQrySecurityRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRUPDATESECURITY://请求更新品种
                    {
                        SrvOnMGRUpdateSecurity(packet as MGRUpdateSecurityRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYSYMBOL://请求查询合约
                    {
                        SrvOnMGRQrySymbol(packet as MGRQrySymbolRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRUPDATESYMBOL://请求更新合约
                    {
                        SrvOnMGRUpdateSymbol(packet as MGRUpdateSymbolRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYRULECLASS://请求查询风控规则列表
                    {
                        SrvOnMGRQryRuleSet(packet as MGRQryRuleSetRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRUPDATERULEITEM://请求更新风控规则
                    {
                        SrvOnMGRUpdateRule(packet as MGRUpdateRuleRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYRULEITEM://请求查询帐户风控项目列表
                    {
                        SrvOnMGRQryRuleItem(packet as MGRQryRuleItemRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRDELRULEITEM://请求删除风控规则
                    {
                        SrvOnMGRDelRuleItem(packet as MGRDelRuleItemRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYSYSTEMSTATUS://请求系统状态
                    {
                        SrvOnMGRQrySystemStatus(packet as MGRQrySystemStatusRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYORDER://请求查询历史委托
                    {
                        SrvOnMGRQryOrder(packet as MGRQryOrderRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYTRADE://请求查询历史成交
                    {
                        SrvnMGRQryTrade(packet as MGRQryTradeRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYPOSITION://请求查询历史持仓
                    {
                        SrvOnMGRQryPosition(packet as MGRQryPositionRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYCASH://请求查询出入金记录
                    {
                        SrvOnMGRQryCash(packet as MGRQryCashRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYSETTLEMENT://请求查询结算单
                    {
                        SrvOnMGRQrySettlement(packet as MGRQrySettleRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRCHANGEACCOUNTPASS://请求修改密码
                    {
                        SrvOnMGRChangeAccountPassword(packet as MGRChangeAccountPassRequest,session,manager);
                        break;
                    }
                case MessageTypes.MGRADDSECURITY://请求添加品种
                    {
                        SrvOnMGRReqAddSecurity(packet as MGRReqAddSecurityRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRADDSYMBOL://请求添加合约
                    {
                        SrvOnMGRReqAddSymbol(packet as MGRReqAddSymbolRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRCHANGEINVESTOR://请求修改投资者信息
                    {
                        SrvOnMGRReqChangeInvestor(packet as MGRReqChangeInvestorRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRUPDATEPOSLOCK://请求修改帐户锁仓权限
                    {
                        SrvOnMGRReqUpdateAccountPosLock(packet as MGRReqUpdatePosLockRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYMANAGER://请求查询管理员列表
                    {
                        SrvOnMGRQryManager(packet as MGRQryManagerRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRADDMANAGER://请求添加管理员
                    { 
                        SrvOnMGRAddManger(packet as MGRReqAddManagerRequest,session,manager);
                        break;
                    }
                case MessageTypes.MGRUPDATEMANAGER://请求更新管理员
                    {
                        SrvOnMGRUpdateManger(packet as MGRReqUpdateManagerRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRQRYACCTSERVICE://查询帐户服务
                    {
                        SrvOnMGRQryAcctService(packet as MGRQryAcctServiceRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRCONTRIBREQUEST://扩展请求
                    {
                        SrvOnMGRContribRequest(packet as MGRContribRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRUPDATEPASS://请求修改密码
                    {
                        SrvOnMGRUpdatePass(packet as MGRUpdatePassRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRINSERTTRADE://请求插入成交
                    {
                        SrvOnInsertTrade(packet as MGRReqInsertTradeRequest, session, manager);
                        break;
                    }
                case MessageTypes.MGRDELACCOUNT://请求删除交易帐户
                    {
                        SrvOnDelAccount(packet as MGRReqDelAccountRequest, session, manager);
                        break;
                    }
                default:
                    debug("packet type:" + packet.Type.ToString() + " not set handler", QSEnumDebugLevel.WARNING);
                    break;
            }
        }
    }
}
