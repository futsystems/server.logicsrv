using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {
        /// <summary>
        /// 查询交易帐户列表
        /// </summary>
        public void ReqQryAccountList()
        {
            debug("查询交易帐户列表", QSEnumDebugLevel.INFO);
            MGRQryAccountRequest request = RequestTemplate<MGRQryAccountRequest>.CliSendRequest(requestid++);

            SendPacket(request);

        }

        /// <summary>
        /// 设定观察帐户列表
        /// </summary>
        /// <param name="list"></param>
        public void ReqWatchAccount(List<string> list)
        {
            debug("请求设置观察帐户列表:" + string.Join(",", list.ToArray()), QSEnumDebugLevel.INFO);
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

        public void ReqCashOperation(string account, decimal amount,QSEnumEquityType eq_type, string transref, string comment)
        {
            debug("请求出入金操作:" + account + " amount:" + amount.ToString() + " transref:" + transref + " comment:" + comment, QSEnumDebugLevel.INFO);
            //MGRCashOperationRequest request = RequestTemplate<MGRCashOperationRequest>.CliSendRequest(requestid++);
            //request.Account = account;
            //request.Amount = amount;
            //request.TransRef = transref;
            //request.Comment = comment;

            //SendPacket(request);
            this.ReqContribRequest("AccountManager", "AccountCashOperation", Mixins.Json.JsonMapper.ToJson(new { account = account, amount = amount, txnref = transref, comment = comment,equity_type=eq_type }));
     
        }

        public void ReqUpdateAccountIntraday(string account, bool intraday)
        {
            debug("请求更新帐户日内属性:" + account + " intraday:" + intraday.ToString(), QSEnumDebugLevel.INFO);
            //MGRUpdateIntradayRequest request = RequestTemplate<MGRUpdateIntradayRequest>.CliSendRequest(requestid++);
            //request.Account = account;
            //request.Intraday = intraday;

            //SendPacket(request);
            this.ReqContribRequest("AccountManager", "UpdateAccountIntraday", Mixins.Json.JsonMapper.ToJson(new { account = account, intraday = intraday }));
     
        }

        public void ReqUpdateAccountCategory(string account, QSEnumAccountCategory category)
        {
            debug("请求更新帐户类别:" + account + " category:" + category.ToString(), QSEnumDebugLevel.INFO);
            //MGRUpdateCategoryRequest request = RequestTemplate<MGRUpdateCategoryRequest>.CliSendRequest(requestid++);
            //request.Account = account;
            //request.Category = category;

            this.ReqContribRequest("AccountManager", "UpdateAccountCategory", Mixins.Json.JsonMapper.ToJson(new { account = account, category = category }));
        }

        public void ReqUpdateRouteType(string account, QSEnumOrderTransferType routertrype)
        {
            debug("请求更新路由类别:" + account + " category:" + routertrype.ToString(), QSEnumDebugLevel.INFO);
            //MGRUpdateRouteTypeRequest request = RequestTemplate<MGRUpdateRouteTypeRequest>.CliSendRequest(requestid++);
            //request.Account = account;
            //request.RouteType = routetype;

            //SendPacket(request);

            this.ReqContribRequest("AccountManager", "UpdateRouteType", Mixins.Json.JsonMapper.ToJson(new { account = account, routertrype = routertrype }));
       
        }

        public void ReqUpdateAccountExecute(string account, bool active)
        {
            debug("请求更新交易权限:" + account + " active:" + active.ToString(), QSEnumDebugLevel.INFO);
            //MGRUpdateExecuteRequest request = RequestTemplate<MGRUpdateExecuteRequest>.CliSendRequest(requestid++);
            //request.Account = account;
            //request.Execute = active;

            //SendPacket(request);
            this.ReqContribRequest("AccountManager", "UpdateAccountExecute", Mixins.Json.JsonMapper.ToJson(new { account = account, execute = active }));
       
        }

        public void ReqAddAccount(QSEnumAccountCategory category, string account, string pass, int mgrid, int userid, int routergroupid)
        {
            debug("请求添加交易帐号", QSEnumDebugLevel.INFO);
            //MGRAddAccountRequest request = RequestTemplate<MGRAddAccountRequest>.CliSendRequest(requestid++);
            //request.AccountID = account;
            //request.Category = category;
            //request.Password = pass;
            //request.UserID = userid;
            //request.MgrID = mgrid;
            //request.RouterGroup_ID = routergroupid;
            //SendPacket(request);
            this.ReqContribRequest("AccountManager", "AddAccount", Mixins.Json.JsonMapper.ToJson(new { account = account, category = category, password = pass, user_id = userid, manager_id = mgrid, routergroup_id = routergroupid }));
        }

        public void ReqDelAccount(string account)
        {
            debug("请求删除交易帐号", QSEnumDebugLevel.INFO);
            //MGRReqDelAccountRequest request = RequestTemplate<MGRReqDelAccountRequest>.CliSendRequest(requestid++);
            //request.AccountToDelete = account;

            this.ReqContribRequest("AccountManager", "DelAccount", Mixins.Json.JsonMapper.ToJson(new { account = account }));
        
        }

        public void ReqChangeAccountPass(string account, string pass)
        {
            debug("请求修改交易帐号密码", QSEnumDebugLevel.INFO);
            //MGRChangeAccountPassRequest request = RequestTemplate<MGRChangeAccountPassRequest>.CliSendRequest(requestid++);

            //request.TradingAccount = account;
            //request.NewPassword = pass;

            this.ReqContribRequest("AccountManager", "UpdateAccountPass", Mixins.Json.JsonMapper.ToJson(new { account = account, newpass = pass }));
        
        }

        public void ReqChangeInverstorInfo(string account, string name, string broker, int bankfk, string bankac)
        {
            debug("请求修改投资者信息", QSEnumDebugLevel.INFO);
            //MGRReqChangeInvestorRequest request = RequestTemplate<MGRReqChangeInvestorRequest>.CliSendRequest(requestid++);

            //request.TradingAccount = account;
            //request.Name = name;
            //request.Broker = broker;
            //request.BankFK = bankfk;
            //request.BankAC = bankac;

            //SendPacket(request);
            this.ReqContribRequest("AccountManager", "UpdateAccountInvestor", Mixins.Json.JsonMapper.ToJson(new { account = account, name = name, broker = broker, bank_id = bankfk, bank_ac = bankac }));
       
        }

        //public void ReqUpdaetAccountPosLock(string account, bool poslock)
        //{
        //    debug("请求更新帐户锁仓权限", QSEnumDebugLevel.INFO);
        //    MGRReqUpdatePosLockRequest request = RequestTemplate<MGRReqUpdatePosLockRequest>.CliSendRequest(requestid++);

        //    request.TradingAccount = account;
        //    request.PosLock = poslock;

        //    SendPacket(request);
        //}

        //public void ReqUpdateAccountSideMargin(string account, bool sidemargin)
        //{
        //    debug("请求更新帐户单向大边", QSEnumDebugLevel.INFO);
        //    this.ReqContribRequest("MgrExchServer", "UpdateAccountSideMargin", account + "," + sidemargin.ToString());
        //}

        public void ReqUpdateAccountCommissionTemplate(string account,int templateid)
        {
            debug("请求更新帐户手续费模板", QSEnumDebugLevel.INFO);
            this.ReqContribRequest("AccountManager", "UpdateAccountCommissionTemplate", account + "," + templateid.ToString());
        }

        public void ReqUpdateAccountMarginTemplate(string account, int templateid)
        {
            debug("请求更新帐户保证金模板", QSEnumDebugLevel.INFO);
            this.ReqContribRequest("AccountManager", "UpdateAccountMarginTemplate", account + "," + templateid.ToString());
        }

        public void ReqUpdateAccountExStrategyTemplate(string account, int templateid)
        {
            debug("请求更交易参数模板", QSEnumDebugLevel.INFO);
            this.ReqContribRequest("AccountManager", "UpdateAccountExStrategyTemplate", account + "," + templateid.ToString());
        }


        public void ReqUpdateAccountCreditSeparate(string account, bool separate)
        {
            this.ReqContribRequest("AccountManager", "UpdateAccountCreditSeparate", account + "," + separate.ToString());
        }

        /// <summary>
        /// 查询分区管理员登入信息
        /// </summary>
        /// <param name="domainid"></param>
        public void ReqQryAccountLoginInfo(string account)
        {
            this.ReqContribRequest("AccountManager", "QryAccountLoginInfo", account);
        }


        /// <summary>
        /// 查询交易帐户信息
        /// </summary>
        /// <param name="account"></param>
        public void ReqQryAccountFinInfo(string account)
        {
            this.ReqContribRequest("AccountManager", "QryAccountFinInfo", account);
        }

        /// <summary>
        /// 修改交易帐户路由组
        /// </summary>
        /// <param name="account"></param>
        /// <param name="rgid"></param>
        public void ReqUpdateRouterGroup(string account,int rgid)
        {
            this.ReqContribRequest("AccountManager", "UpdateAccountRouterGroup", account + "," + rgid.ToString());
        }
    }
}
