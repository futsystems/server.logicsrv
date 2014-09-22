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

        public void ReqChangeInverstorInfo(string account, string name,string broker,string bank,string bankac)
        {
            debug("请求修改投资者信息", QSEnumDebugLevel.INFO);
            MGRReqChangeInvestorRequest request = RequestTemplate<MGRReqChangeInvestorRequest>.CliSendRequest(requestid++);

            request.TradingAccount = account;
            request.Name = name;
            request.Broker = broker;
            request.Bank = bank;
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

    }
}
