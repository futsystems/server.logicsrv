using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {

        #region 客户端暴露的操作

        /// <summary>
        /// 请求登入
        /// </summary>
        /// <param name="loginid"></param>
        /// <param name="pass"></param>
        public void ReqLogin(string loginid, string pass)
        {
            MGRLoginRequest request = RequestTemplate<MGRLoginRequest>.CliSendRequest(requestid++);
            request.LoginID = loginid;
            request.Passwd = pass;

            SendPacket(request);
        }



        
        /*
        /// <summary>
        /// 请求注销行情数据
        /// </summary>
        public void ReqUnRegisterSymbols()
        {
            UnregisterSymbolsRequest request = RequestTemplate<UnregisterSymbolsRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }



        /// <summary>
        /// 请求帐户信息
        /// </summary>
        public void ReqQryAccountInfo()
        {
            QryAccountInfoRequest request = RequestTemplate<QryAccountInfoRequest>.CliSendRequest(requestid++);
            request.Account = _account;

            SendPacket(request);
        }
        /// <summary>
        /// 请求查询可开手数
        /// </summary>
        public void ReqQryMaxOrderVol(string symbol)
        {
            QryMaxOrderVolRequest request = RequestTemplate<QryMaxOrderVolRequest>.CliSendRequest(requestid++);
            request.Symbol = symbol;
            request.PostFlag = QSEnumOrderPosFlag.UNKNOWN;

            SendPacket(request);
        }

        /// <summary>
        /// 查询交易者信息
        /// </summary>
        public void ReqQryInvestor()
        {
            QryInvestorRequest request = RequestTemplate<QryInvestorRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            SendPacket(request);
        }

        /// <summary>
        /// 查询合约信息
        /// </summary>
        public void ReqQrySymbol()
        {
            QrySymbolRequest request = RequestTemplate<QrySymbolRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }

        /// <summary>
        /// 查询结算信息
        /// </summary>
        public void ReqQrySettleInfo()
        {
            QrySettleInfoRequest request = RequestTemplate<QrySettleInfoRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            request.Tradingday = 20140809;
            SendPacket(request);
        }

        /// <summary>
        /// 查询结算确认
        /// </summary>
        public void ReqQrySettleInfoConfirm()
        {
            QrySettleInfoConfirmRequest request = RequestTemplate<QrySettleInfoConfirmRequest>.CliSendRequest(requestid++);
            request.Account = _account;

            SendPacket(request);
        }

        /// <summary>
        /// 确认结算单
        /// </summary>
        public void ReqConfirmSettlement()
        {
            ConfirmSettlementRequest request = RequestTemplate<ConfirmSettlementRequest>.CliSendRequest(requestid++);
            request.Account = _account;

            SendPacket(request);
        }
        /// <summary>
        /// 查询委托
        /// </summary>
        public void ReqQryOrder(string symbol = "", long orderid = 0)
        {
            QryOrderRequest request = RequestTemplate<QryOrderRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            request.Symbol = symbol;
            request.OrderID = orderid;
            SendPacket(request);
        }

        /// <summary>
        /// 查询成交
        /// </summary>
        public void ReqQryTrade(string symbol = "")
        {
            QryTradeRequest request = RequestTemplate<QryTradeRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            request.Symbol = symbol;

            SendPacket(request);
        }

        /// <summary>
        /// 查询持仓
        /// </summary>
        public void ReqQryPosition(string symbol = "")
        {
            QryPositionRequest request = RequestTemplate<QryPositionRequest>.CliSendRequest(requestid++);
            request.Account = _account;
            request.Symbol = symbol;

            SendPacket(request);
        }

        /// <summary>
        /// 查询Bar数据
        /// </summary>
        public void ReqBar()
        {
            QryBarRequest request = RequestTemplate<QryBarRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }

        /// <summary>
        /// 请求注册行情数据
        /// </summary>
        public void ReqRegisterSymbols(string[] symbols)
        {
            RegisterSymbolsRequest request = RequestTemplate<RegisterSymbolsRequest>.CliSendRequest(requestid++);
            request.SetSymbols(symbols);

            SendPacket(request);
            connecton.Subscribe(symbols);

        }**/

        #endregion


        #region 交易帐号类操作
        /// <summary>
        /// 查询交易帐户列表
        /// </summary>
        public void ReqQryAccountList()
        {
            debug("查询交易帐户列表",QSEnumDebugLevel.INFO);
            MGRQryAccountRequest request = RequestTemplate<MGRQryAccountRequest>.CliSendRequest(requestid++);

            SendPacket(request);

        }

        /// <summary>
        /// 设定观察帐户列表
        /// </summary>
        /// <param name="list"></param>
        public void ReqWatchAccount(List<string> list)
        {
            debug("请求设置观察帐户列表:" + string.Join(",", list.ToArray()),QSEnumDebugLevel.INFO);
            MGRWatchAccountRequest request = RequestTemplate<MGRWatchAccountRequest>.CliSendRequest(requestid++);
            request.Add(list);

            SendPacket(request);

        }
        
        /// <summary>
        /// 恢复某个交易帐号的日内交易数据
        /// </summary>
        /// <param name="account"></param>
        public void ReqResumeAccount(string account)
        {
            debug("请求恢复日内交易数据 Account:" + account, QSEnumDebugLevel.INFO);
            MGRResumeAccountRequest request = RequestTemplate<MGRResumeAccountRequest>.CliSendRequest(requestid++);
            request.ResumeAccount = account;

            SendPacket(request);
        }

        public void ReqQryAccountInfo(string account)
        {
            debug("请求查询交易帐号 Account:" + account + " 信息", QSEnumDebugLevel.INFO);
            MGRQryAccountInfoRequest request = RequestTemplate<MGRQryAccountInfoRequest>.CliSendRequest(requestid++);
            request.Account = account;

            SendPacket(request);
        }

        public void ReqCashOperation(string account, decimal amount, string transref, string comment)
        {
            debug("请求出入金操作:" + account + " amount:" + amount.ToString() + " transref:" + transref + " comment:" + comment,QSEnumDebugLevel.INFO);
            MGRCashOperationRequest request = RequestTemplate<MGRCashOperationRequest>.CliSendRequest(requestid++);
            request.Account = account;
            request.Amount = amount;
            request.TransRef = transref;
            request.Comment = comment;

            SendPacket(request);
        }

        public void ReqUpdateAccountIntraday(string account, bool intraday)
        {
            debug("请求更新帐户日内属性:" + account + " intraday:" + intraday.ToString(), QSEnumDebugLevel.INFO);
            MGRUpdateIntradayRequest request = RequestTemplate<MGRUpdateIntradayRequest>.CliSendRequest(requestid++);
            request.Account = account;
            request.Intraday = intraday;

            SendPacket(request);
        }

        public void ReqUpdateAccountCategory(string account, QSEnumAccountCategory category)
        {
            debug("请求更新帐户类别:" + account + " category:" + category.ToString(), QSEnumDebugLevel.INFO);
            MGRUpdateCategoryRequest request = RequestTemplate<MGRUpdateCategoryRequest>.CliSendRequest(requestid++);
            request.Account = account;
            request.Category = category;

            SendPacket(request);
        }

        public void ReqUpdateRouteType(string account, QSEnumOrderTransferType routetype)
        {
            debug("请求更新路由类别:" + account + " category:" + routetype.ToString(), QSEnumDebugLevel.INFO);
            MGRUpdateRouteTypeRequest request = RequestTemplate<MGRUpdateRouteTypeRequest>.CliSendRequest(requestid++);
            request.Account = account;
            request.RouteType = routetype;

            SendPacket(request);
        }

        public void ReqUpdateAccountExecute(string account, bool active)
        {
            debug("请求更新交易权限:" + account + " active:" + active.ToString(), QSEnumDebugLevel.INFO);
            MGRUpdateExecuteRequest request = RequestTemplate<MGRUpdateExecuteRequest>.CliSendRequest(requestid++);
            request.Account = account;
            request.Execute = active;

            SendPacket(request);
        }

        public void ReqAddAccount(QSEnumAccountCategory category, string account, string pass, int mgrid,int userid)
        {
            debug("请求添加交易帐号", QSEnumDebugLevel.INFO);
            MGRAddAccountRequest request = RequestTemplate<MGRAddAccountRequest>.CliSendRequest(requestid++);
            request.AccountID = account;
            request.Category = category;
            request.Password = pass;
            request.UserID = userid;
            request.MgrID = mgrid;

            SendPacket(request);
        }

        public void ReqChangeAccountPass(string account, string pass)
        {
            debug("请求修改交易帐号密码", QSEnumDebugLevel.INFO);
            MGRChangeAccountPassRequest request = RequestTemplate<MGRChangeAccountPassRequest>.CliSendRequest(requestid++);

            request.TradingAccount = account;
            request.NewPassword = pass;

            SendPacket(request);
        }

        public void ReqChangeInverstorInfo(string account, string name, string broker, int bankfk, string bankac)
        {
            debug("请求修改投资者信息", QSEnumDebugLevel.INFO);
            MGRReqChangeInvestorRequest request = RequestTemplate<MGRReqChangeInvestorRequest>.CliSendRequest(requestid++);

            request.TradingAccount = account;
            request.Name = name;
            request.Broker = broker;
            request.BankFK = bankfk;
            request.BankAC = bankac;

            SendPacket(request);
        }

        public void ReqUpdaetAccountPosLock(string account, bool poslock)
        {
            debug("请求更新帐户锁仓权限", QSEnumDebugLevel.INFO);
            MGRReqUpdatePosLockRequest request = RequestTemplate<MGRReqUpdatePosLockRequest>.CliSendRequest(requestid++);

            request.TradingAccount = account;
            request.PosLock = poslock;

            SendPacket(request);
        }
        #endregion


        #region 管理员管理

        public void ReqQryManager()
        {
            debug("请求查询管理员列表", QSEnumDebugLevel.INFO);
            MGRQryManagerRequest request = RequestTemplate<MGRQryManagerRequest>.CliSendRequest(requestid++);
            SendPacket(request);
            
        }

        public void ReqUpdateManager(Manager manger)
        {
            debug("请求更新管理员", QSEnumDebugLevel.INFO);
            MGRReqUpdateManagerRequest request = RequestTemplate<MGRReqUpdateManagerRequest>.CliSendRequest(requestid++);
            request.ManagerToSend = manger;

            SendPacket(request);
        }

        public void ReqUpdatePass(string oldpass, string pass)
        {
            debug("请求更改密码", QSEnumDebugLevel.INFO);
            MGRUpdatePassRequest request = RequestTemplate<MGRUpdatePassRequest>.CliSendRequest(requestid++);
            request.OldPass = oldpass;
            request.NewPass = pass;

            SendPacket(request);
        }

        #endregion


        #region 服务端管理

        public void ReqOpenClearCentre()
        {
            debug("请求开启交易中心", QSEnumDebugLevel.INFO);
            MGRReqOpenClearCentreRequest request = RequestTemplate<MGRReqOpenClearCentreRequest>.CliSendRequest(requestid++);
            SendPacket(request);
        }

        public void ReqCloseCentre()
        {
            debug("请求关闭清算中心", QSEnumDebugLevel.INFO);
            MGRReqCloseClearCentreRequest request = RequestTemplate<MGRReqCloseClearCentreRequest>.CliSendRequest(requestid++);
            SendPacket(request);
        }

        public void ReqQryConnector()
        {
            debug("请求查询通道列表", QSEnumDebugLevel.INFO);
            MGRQryConnectorRequest request = RequestTemplate<MGRQryConnectorRequest>.CliSendRequest(requestid++);
            SendPacket(request);
        }

        public void ReqStartBroker(string fullname)
        {
            debug("请求启动成交通道:"+fullname, QSEnumDebugLevel.INFO);
            MGRReqStartBrokerRequest request = RequestTemplate<MGRReqStartBrokerRequest>.CliSendRequest(requestid++);
            request.FullName = fullname;

            SendPacket(request);
        }

        public void ReqStopBroker(string fullname)
        {
            debug("请求停止成交通道:" + fullname, QSEnumDebugLevel.INFO);
            MGRReqStopBrokerRequest request = RequestTemplate<MGRReqStopBrokerRequest>.CliSendRequest(requestid++);
            request.FullName = fullname;

            SendPacket(request);
        }

        public void ReqStartDataFeed(string fullname)
        {
            debug("请求启动行情通道:" + fullname, QSEnumDebugLevel.INFO);
            MGRReqStartDataFeedRequest request = RequestTemplate<MGRReqStartDataFeedRequest>.CliSendRequest(requestid++);
            request.FullName = fullname;

            SendPacket(request);
        }

        public void ReqStopDataFeed(string fullname)
        {
            debug("请求停止行情通道:" + fullname, QSEnumDebugLevel.INFO);
            MGRReqStopDataFeedRequest request = RequestTemplate<MGRReqStopDataFeedRequest>.CliSendRequest(requestid++);
            request.FullName = fullname;

            SendPacket(request);
        }
        #endregion

        #region 基础数据维护
        public void ReqQryExchange()
        {
            debug("请求查询交易所列表", QSEnumDebugLevel.INFO);
            MGRQryExchangeRequuest request = RequestTemplate<MGRQryExchangeRequuest>.CliSendRequest(requestid++);

            SendPacket(request);
        }

        public void ReqQryMarketTime()
        {
            debug("请求查询市场时间列表", QSEnumDebugLevel.INFO);
            MGRQryMarketTimeRequest request = RequestTemplate<MGRQryMarketTimeRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }
        public void ReqQrySecurity()
        {
            debug("请求查询品种列表", QSEnumDebugLevel.INFO);
            MGRQrySecurityRequest request = RequestTemplate<MGRQrySecurityRequest>.CliSendRequest(requestid ++);

            SendPacket(request);
        }
        public void ReqUpdateSecurity(SecurityFamilyImpl sec)
        {
            debug("请求更新品种信息", QSEnumDebugLevel.INFO);
            MGRUpdateSecurityRequest request = RequestTemplate<MGRUpdateSecurityRequest>.CliSendRequest(requestid++);
            request.SecurityFaimly = sec;

            SendPacket(request);
        }

        public void ReqAddSecurity(SecurityFamilyImpl sec)
        {
            debug("请求添加品种信息", QSEnumDebugLevel.INFO);
            MGRReqAddSecurityRequest request = RequestTemplate<MGRReqAddSecurityRequest>.CliSendRequest(requestid++);
            request.SecurityFaimly = sec;

            SendPacket(request);
        }

        public void ReqQrySymbol()
        {
            debug("请求查询合约列表", QSEnumDebugLevel.INFO);
            MGRQrySymbolRequest request = RequestTemplate<MGRQrySymbolRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }

        public void ReqUpdateSymbol(SymbolImpl sym)
        {
            debug("请求更新合约", QSEnumDebugLevel.INFO);
            MGRUpdateSymbolRequest request = RequestTemplate<MGRUpdateSymbolRequest>.CliSendRequest(requestid++);
            request.Symbol = sym;

            SendPacket(request);
        }

        public void ReqAddSymbol(SymbolImpl sym)
        {
            debug("请求添加合约", QSEnumDebugLevel.INFO);
            MGRReqAddSymbolRequest request = RequestTemplate<MGRReqAddSymbolRequest>.CliSendRequest(requestid++);
            request.Symbol = sym;

            SendPacket(request);
        }
        #endregion


        #region 风控规则操作
        public void ReqQryRuleSet()
        {
            debug("请求查询风控规则列表", QSEnumDebugLevel.INFO);
            MGRQryRuleSetRequest request = RequestTemplate<MGRQryRuleSetRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }

        public void ReqQryRuleItem(string account,QSEnumRuleType type)
        {
            debug("请求查询帐户风控规则列表");
            MGRQryRuleItemRequest request = RequestTemplate<MGRQryRuleItemRequest>.CliSendRequest(requestid++);
            request.Account = account;
            request.RuleType = type;

            SendPacket(request);
        }
        public void ReqUpdateRuleSet(RuleItem item)
        {
            debug("请求更新风控规则", QSEnumDebugLevel.INFO);
            MGRUpdateRuleRequest request = RequestTemplate<MGRUpdateRuleRequest>.CliSendRequest(requestid++);
            request.RuleItem = item;

            SendPacket(request);
        }

        public void ReqDelRuleItem(RuleItem item)
        {
            debug("请求删除风控规则", QSEnumDebugLevel.INFO);
            MGRDelRuleItemRequest request = RequestTemplate<MGRDelRuleItemRequest>.CliSendRequest(requestid++);
            request.RuleItem = item;

            SendPacket(request);
        }
        #endregion

        #region 交易类操作
        /// <summary>
        /// 发送委托
        /// </summary>
        /// <param name="order"></param>
        public void ReqOrderInsert(Order order)
        {
            OrderInsertRequest request = RequestTemplate<OrderInsertRequest>.CliSendRequest(requestid++);
            request.Order = order;

            SendPacket(request);
        }

        /// <summary>
        /// 提交委托操作
        /// </summary>
        /// <param name="action"></param>
        public void ReqOrderAction(OrderAction action)
        {
            OrderActionRequest requets = RequestTemplate<OrderActionRequest>.CliSendRequest(requestid++);
            requets.OrderAction = action;

            SendPacket(requets);
        }

        #endregion


        #region 系统状态

        public void ReqQrySystemStatus()
        {
            MGRQrySystemStatusRequest request = RequestTemplate<MGRQrySystemStatusRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }
        #endregion



        #region 历史记录查询
        public void ReqQryHistOrders(string account,int settleday)
        {
            MGRQryOrderRequest request = RequestTemplate<MGRQryOrderRequest>.CliSendRequest(requestid++);
            request.TradingAccount = account;
            request.Settleday = settleday;

            SendPacket(request);

        }

        public void ReqQryHistTrades(string account, int settleday)
        {
            MGRQryTradeRequest request = RequestTemplate<MGRQryTradeRequest>.CliSendRequest(requestid++);
            request.TradingAccount = account;
            request.Settleday = settleday;

            SendPacket(request);
        }

        public void ReqQryHistPosition(string account, int settleday)
        {
            MGRQryPositionRequest request = RequestTemplate<MGRQryPositionRequest>.CliSendRequest(requestid++);
            request.TradingAccount = account;
            request.Settleday = settleday;

            SendPacket(request);
        }

        public void ReqQryHistCashTransaction(string account, int settleday)
        {
            MGRQryCashRequest request = RequestTemplate<MGRQryCashRequest>.CliSendRequest(requestid++);
            request.TradingAccount = account;
            request.Settleday = settleday;

            SendPacket(request);
        }

        public void ReqQryHistSettlement(string account, int settleday)
        {
            MGRQrySettleRequest request = RequestTemplate<MGRQrySettleRequest>.CliSendRequest(requestid++);
            request.TradingAccount = account;
            request.Settleday = settleday;

            SendPacket(request);
            
        }
        #endregion


        #region 扩展请求

        /// <summary>
        /// 查询银行列表
        /// </summary>
        public void ReqQryBank()
        {
            this.ReqContribRequest("MgrExchServer", "QryBank", "");
        }

        /// <summary>
        /// 查询代理对应的支付信息
        /// </summary>
        /// <param name="agentfk"></param>
        public void ReqQryAgentPaymentInfo(int agentfk)
        {
            this.ReqContribRequest("MgrExchServer", "QryAgentPaymentInfo",agentfk.ToString());
        }

        /// <summary>
        /// 查询交易帐户对应的支付信息
        /// </summary>
        /// <param name="account"></param>
        public void ReqQryAccountPaymentInfo(string account)
        {
            this.ReqContribRequest("MgrExchServer", "QryAccountPaymentInfo",account);
        }
        /// <summary>
        /// 查询代理的财务信息
        /// </summary>
        public void ReqQryAgentFinanceInfo()
        {
            this.ReqContribRequest("MgrExchServer", "QryFinanceInfo", "");
        }

        /// <summary>
        /// 查询代理精简财务信息
        /// </summary>
        public void ReqQryAgentFinanceInfoLite()
        {
            this.ReqContribRequest("MgrExchServer", "QryFinanceInfoLite", "");
        }

        /// <summary>
        /// 更新代理主域的银行卡信息
        /// </summary>
        /// <param name="playload"></param>
        public void ReqUpdateAgentBankInfo(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateAgentBankAccount", playload);
        }

        #region 代理出入金操作
        /// <summary>
        /// 查询代理出入金记录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void ReqQryAgentCashTrans(int mgrfk, long start, long end)
        {
            this.ReqContribRequest("MgrExchServer", "QueryAgentCashTrans", mgrfk.ToString() + "," + start.ToString() + "," + end.ToString());
        }

        /// <summary>
        /// 请求出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqRequestCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "RequestCashOperation", playload);
        }

        /// <summary>
        /// 确认出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqConfirmCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "ConfirmCashOperation", playload);
        }

        /// <summary>
        /// 取消出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqCancelCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "CancelCashOperation", playload);
        }

        /// <summary>
        /// 拒绝出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqRejectCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "RejectCashOperation", playload);
        }
        #endregion

        #region 帐户出入金操作

        /// <summary>
        /// 查询交易帐户出入金记录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void ReqQryAccountCashTrans(string account, long start, long end)
        {
            this.ReqContribRequest("MgrExchServer", "QueryAccountCashTrans",account+","+start.ToString()+","+end.ToString());
        }
        /// <summary>
        /// 请求出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqRequestAccountCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "RequestAccountCashOperation", playload);
        }

        /// <summary>
        /// 确认出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqConfirmAccountCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "ConfirmAccountCashOperation", playload);
        }

        /// <summary>
        /// 取消出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqCancelAccountCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "CancelAccountCashOperation", playload);
        }

        /// <summary>
        /// 拒绝出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqRejectAccountCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "RejectAccountCashOperation", playload);
        }
        #endregion


        /// <summary>
        /// 查询所有代理的出入金操作
        /// </summary>
        public void ReqQryAgentCashopOperationTotal()
        {
            this.ReqContribRequest("MgrExchServer", "QryAgentCashOperationTotal", "");
        }

        /// <summary>
        /// 查询所有交易帐户出入金操作
        /// </summary>
        public void ReqQryAccountCashopOperationTotal()
        {
            this.ReqContribRequest("MgrExchServer", "QryAccountCashOperationTotal", "");
        }

        #region 查询报表

        /// <summary>
        /// 查询某日所有代理的利润报表
        /// </summary>
        /// <param name="settleday"></param>
        public void ReqQryTotalReport(int agentfk,int settleday)
        {
            this.ReqContribRequest("FinServiceCentre", "QryTotalReport", agentfk.ToString()+","+settleday.ToString());
        }

        /// <summary>
        /// 查询某个代理的在一个时间段内的汇总
        /// </summary>
        /// <param name="settleday"></param>
        public void ReqQrySummaryReport(int agentfk, int start, int end)
        {
            this.ReqContribRequest("FinServiceCentre", "QrySummaryReport", agentfk.ToString() + "," + start.ToString() + "," + end.ToString());
        }


        /// <summary>
        /// 查询某个代理某个时间段内的所有利润流水
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void ReqQryTotalReportByDayRange(int agentfk, int start, int end)
        {
            this.ReqContribRequest("FinServiceCentre", "QryTotalReportDayRange", agentfk.ToString() + "," + start.ToString() + "," + end.ToString());
        }

        /// <summary>
        /// 查询某个代理某个交易日的按帐户汇总的利润报表
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="settleday"></param>
        public void ReqQryDetailReportByAccount(int agentfk, int settleday)
        {
            this.ReqContribRequest("FinServiceCentre", "QryDetailReportByAccount", agentfk.ToString() + "," + settleday.ToString());
        }
        #endregion

        /// <summary>
        /// 查询某个代理 某个服务计划的成本参数
        /// 如果没有设置则返回默认参数
        /// 
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="spfk"></param>
        public void ReqQrySPAgentArg(int agentfk, int spfk)
        {
            this.ReqContribRequest("FinServiceCentre", "QryAgentSPArg", agentfk.ToString()+","+spfk.ToString());
        }

        /// <summary>
        /// 更新某个代理的某个服务计划的参数
        /// </summary>
        /// <param name="playload"></param>
        public void ReqUpdateSPAgentArg(string playload)
        {
            this.ReqContribRequest("FinServiceCentre", "UpdateAgentSPArg", playload);
        }
        /// <summary>
        /// 查询某个交易帐户的配资参数
        /// </summary>
        /// <param name="account"></param>
        public void ReqQryFinService(string account)
        {
            this.ReqContribRequest("FinServiceCentre", "QryFinService", account);
        }

        /// <summary>
        /// 更新配资服务参数
        /// </summary>
        /// <param name="playload"></param>
        public void ReqUpdateFinServiceArgument(string playload)
        {
            this.ReqContribRequest("FinServiceCentre", "UpdateArguments", playload);
        }

        /// <summary>
        /// 查询服务计划
        /// </summary>
        public void ReqQryServicePlan()
        {
            this.ReqContribRequest("FinServiceCentre", "QryFinServicePlan", "");
        }

        /// <summary>
        /// 修改某个帐户的配资服务
        /// </summary>
        /// <param name="playload"></param>
        public void ReqChangeFinService(string playload)
        {
            this.ReqContribRequest("FinServiceCentre", "ChangeServicePlane", playload);
        }

        /// <summary>
        /// 删除某个交易帐号的配资服务
        /// </summary>
        /// <param name="account"></param>
        public void ReqDeleteFinService(string account)
        {
            this.ReqContribRequest("FinServiceCentre", "DeleteServicePlane", account);
        }



        #region 权限类操作

        /// <summary>
        /// 查询所有权限模板
        /// </summary>
        public void ReqQryPermmissionTemplateList()
        {
            debug("请求查询权限模板列表", QSEnumDebugLevel.INFO);

            this.ReqContribRequest("MgrExchServer", "QueryPermmissionTemplateList", "");
        }


        /// <summary>
        /// 更新某个权限模板
        /// </summary>
        /// <param name="jsonstr"></param>
        public void ReqUpdatePermissionTemplate(string jsonstr)
        {
            debug("请求更新权限模板", QSEnumDebugLevel.INFO);

            this.ReqContribRequest("MgrExchServer", "UpdatePermission", jsonstr);
        }
        #endregion


        /// <summary>
        /// 调用某个模块 某个命令 某个参数 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        public void ReqContribRequest(string module, string cmd, string args)
        {
            debug("请求扩展命令,module:" + module + " cmd:" + cmd + " args:" + args, QSEnumDebugLevel.INFO);
            MGRContribRequest request = RequestTemplate<MGRContribRequest>.CliSendRequest(requestid++);
            request.ModuleID = module;
            request.CMDStr = cmd;
            request.Parameters = args;

            SendPacket(request);
        
        }


        public void ReqQryAcctService(string account, string servicename)
        {
            debug("请求查询帐户服务", QSEnumDebugLevel.INFO);
            MGRQryAcctServiceRequest request = RequestTemplate<MGRQryAcctServiceRequest>.CliSendRequest(requestid++);
            request.TradingAccount = account;
            request.ServiceName = servicename;

            SendPacket(request);
        }

        

        #endregion

        #region 插入成交
        public void ReqInsertTrade(Trade f)
        {
            debug("请求插入成交", QSEnumDebugLevel.INFO);
            MGRReqInsertTradeRequest request = RequestTemplate<MGRReqInsertTradeRequest>.CliSendRequest(requestid++);
            request.TradeToSend = f;
            SendPacket(request);

        }
        #endregion
    }
}
