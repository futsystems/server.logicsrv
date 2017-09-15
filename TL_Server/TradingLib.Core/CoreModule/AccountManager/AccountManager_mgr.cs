//Copyright 2013 by FutSystems,Inc.
//20170112 整理无用操作
//20170711 整理操作权限

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    public partial class AccountManager
    {

        
        /// <summary>
        /// 统一使用AccountCreation对象创建交易帐户
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [PermissionRequiredAttr("r_account_add")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AddAccountFacde", "AddAccountFacde - add  account", "添加交易帐号", QSEnumArgParseType.Json)]
        public void CTE_AddAccountFacde(ISession session, string json)
        {
            Manager manager = session.GetManager();
            var creation = json.DeserializeObject<AccountCreation>();// Mixins.Json.JsonMapper.ToObject<AccountCreation>(json);
            var account = creation.Account;

            //域帐户数目检查 排除已经删除账户
            if (manager.Domain.GetAccounts().Where(acc=>!acc.Deleted).Count() >= manager.Domain.AccLimit)
            {
                throw new FutsRspError("帐户数目达到上限:" + manager.Domain.AccLimit.ToString());
            }

            if (creation.BaseManagerID == 0)
            {
                creation.BaseManagerID = manager.BaseMgrID;
            }

            var baseMgr = BasicTracker.ManagerTracker[creation.BaseManagerID];
            if (baseMgr== null)
            {
                throw new FutsRspError("账户所属管理员无效");
            }

            //如果不是Root权限的Manager需要进行执行权限检查
            if (!manager.IsInRoot())
            {
                //如果不是为该主域添加帐户,则我们需要判断当前Manager的主域是否拥有请求主域的权限
                if (manager.BaseMgrID != creation.BaseManagerID)
                {
                    if (!manager.IsParentOf(creation.BaseManagerID))
                    {
                        throw new FutsRspError("无权在该管理域开设帐户");
                    }
                }
            }

            //Manager帐户数量限制 如果是在自己的主域中添加交易帐户 则需要检查帐户数量
            int limit = manager.BaseManager.AccLimit;

            int cnt = manager.GetVisibleAccount().Where(acc => !acc.Deleted).Count();//获得该manger下属的所有帐户数目
            if (cnt >= limit)
            {
                throw new FutsRspError("可开帐户数量超过限制:" + limit.ToString());
            }


            //执行操作 并捕获异常 产生异常则给出错误回报
            this.AddAccount(ref creation);//将交易帐户加入到主域

            //优先使用创建账户时提供的configID
            int config_id = creation.Config_ID;
            //没有指定ConfigID 则使用所属管理员的默认配置模板
            if (config_id == 0)
            {
                config_id = baseMgr.AgentAccount.Default_Config_ID;
            }
            //有效配置模板 则更新该账户的配置模板
            if (config_id > 0)
            {
                this.UpdateAccountConfigTemplate(creation.Account, config_id);
            }

            //帐户添加完毕后同步添加profile信息
            creation.Profile.Account = creation.Account;

            //插入新的profile
            BasicTracker.AccountProfileTracker.UpdateAccountProfile(creation.Profile);

            //对外触发交易帐号添加事件
            TLCtxHelper.EventAccount.FireAccountAddEvent(this[creation.Account]);

            session.RspMessage("新增交易帐号:" + creation.Account + "成功");

        }

        /// <summary>
        /// 为某个User创建交易账户
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public bool CreateAccountForUser(int userID, int agentID,CurrencyType currency,out string account)
        {
            account = string.Empty;
            try
            {
                AccountCreation creation = new AccountCreation();
                creation.BaseManagerID = agentID;
                creation.Category = QSEnumAccountCategory.SUBACCOUNT;
                creation.RouterType = QSEnumOrderTransferType.SIM;
                creation.UserID = userID;
                creation.Currency = currency;

                this.AddAccount(ref creation);
                //帐户添加完毕后同步添加profile信息
                creation.Profile.Account = creation.Account;

                //插入新的profile
                BasicTracker.AccountProfileTracker.UpdateAccountProfile(creation.Profile);

                //对外触发交易帐号添加事件
                TLCtxHelper.EventAccount.FireAccountAddEvent(this[creation.Account]);

                account = creation.Account;
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Create Account Error:" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 请求删除交易帐户
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        [PermissionRequiredAttr("r_account_del")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DelAccount", "DelAccount - del account", "删除交易帐户", QSEnumArgParseType.Json)]
        public void CTE_DelAccount(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var accounts = req["accounts"].ToObject<string[]>(); 
            foreach (var account in accounts)
            {
                session.GetManager().PermissionCheckAccount(account);

                IAccount acc = this[account];
                if (acc.GetPositionsHold().Count() > 0)
                {
                    throw new FutsRspError(string.Format("交易帐户:{0} 有持仓 无法删除", acc.ID));
                }

                //检查交易帐户资金
                if (_deleteAccountCheckEquity && (acc.NowEquity > 1 || acc.Credit > 1))
                {
                    throw new FutsRspError(string.Format(string.Format("交易帐户:{0} 权益:{1} 信用额度:{2}未出金 无法删除", account, acc.NowEquity, acc.Credit)));
                }

                this.DelAccount(account);
            }
            session.RspMessage("交易帐户:" + accounts + " 删除成功");
        }


        [PermissionRequiredAttr("r_account_edit_profile")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountProfile", "UpdateAccountProfile - update account profile", "更新交易帐户个人信息",QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountProfile(ISession session, string json)
        {
            var profile = json.DeserializeObject<AccountProfile>();
            session.GetManager().PermissionCheckAccount(profile.Account);

            IAccount account = TLCtxHelper.ModuleAccountManager[profile.Account];
            
            BasicTracker.AccountProfileTracker.UpdateAccountProfile(profile);
            
            //触发交易帐户变动事件
            TLCtxHelper.EventAccount.FireAccountChangeEent(account);

            session.RspMessage("更新个人信息成功");

        }


        /// <summary>
        /// 更新账户类别
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        [PermissionRequiredAttr("r_account_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountCategory", "UpdateAccountCategory - change account category", "修改帐户类别", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountCategory(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var category = Util.ParseEnum<QSEnumAccountCategory>(req["category"].ToString());
            session.GetManager().PermissionCheckAccount(account);

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            this.UpdateAccountCategory(account, category);
            
        }

        /// <summary>
        /// 更新账户冻结/激活状态
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [PermissionRequiredAttr("r_account_edit_execution")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountExecute", "UpdateAccountExecute - change account execute", "修改帐户交易权限状态", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountExecute(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var accounts = req["accounts"].ToObject<string[]>(); 
            var execute = bool.Parse(req["execute"].ToString());
            foreach (var account in accounts)
            {

                session.GetManager().PermissionCheckAccount(account);

                IAccount acct = TLCtxHelper.ModuleAccountManager[account];
                if (execute && !acct.Execute)
                {
                    this.ActiveAccount(account);
                }
                if (!execute && acct.Execute)
                {
                    this.InactiveAccount(account);
                }
            }
            
        }


        /// <summary>
        /// 更新账户日内属性
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [PermissionRequiredAttr("r_account_edit_interday")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountIntraday", "UpdateAccountIntraday - change account intraday setting", "修改帐户日内交易属性", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountIntraday(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var intraday = bool.Parse(req["intraday"].ToString());
            session.GetManager().PermissionCheckAccount(account);

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            this.UpdateAccountIntradyType(account, intraday);
            
        }

        /// <summary>
        /// 更新账户路由类别
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [PermissionRequiredAttr("r_account_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateRouteType", "UpdateRouteType - change account route type", "修改帐户日路由属性", QSEnumArgParseType.Json)]
        public void CTE_UpdateRouteType(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var routetype = Util.ParseEnum<QSEnumOrderTransferType>(req["routertrype"].ToString());
            session.GetManager().PermissionCheckAccount(account);

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            this.UpdateAccountRouterTransferType(account, routetype);
            
        }

        /// <summary>
        /// 更新账户货币类别
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [PermissionRequiredAttr("r_account_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountCurrency", "UpdateAccountCurrency - update account currency", "更新交易帐户货币类别", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountCurrency(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var currency = (CurrencyType)Enum.Parse(typeof(CurrencyType),req["currency"].ToString());
            session.GetManager().PermissionCheckAccount(account);

            this.UpdateAccountCurrency(account, currency);

            session.RspMessage("交易帐户更新货币类型成功");
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountRouterGroup", "UpdateAccountRouterGroup - update account router group", "更新帐户路由组信息")]
        public void CTE_UpdateAccountRouterGroup(ISession session, string account, int gid)
        {
            Manager manager = session.GetManager();
            if (!manager.IsInRoot())
            {
                throw new FutsRspError("无权修改帐户路由组设置");
            }

            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
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
            this.UpdateRouterGroup(account, rg);
            session.RspMessage("更新帐户路由组成功");
        }


        /// <summary>
        /// 修改交易帐户密码
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manger"></param>
        [PermissionRequiredAttr("r_account_edit_pass")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountPass", "UpdateAccountPass - update account password", "更新交易帐户交易密码", QSEnumArgParseType.Json)]
        public void CTE_ChangePassword(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var newpass = req["newpass"].ToString();

            session.GetManager().PermissionCheckAccount(account);

            this.UpdateAccountPass(account, newpass);
            session.RspMessage("修改密码成功");

        }




        /// <summary>
        /// 交易账户 出入金
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [PermissionRequiredAttr("r_account_cashop")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AccountCashOperation", "AccountCashOperation - account cash operation", "给交易帐户出入去金", QSEnumArgParseType.Json)]
        public void CTE_CashOperation(ISession session, string json)
        {
            Manager manager = session.GetManager();

            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var amount = decimal.Parse(req["amount"].ToString());
            var txnref = req["txnref"].ToString();
            var comment = req["comment"].ToString();
            var equity_type = Util.ParseEnum<QSEnumEquityType>(req["equity_type"].ToString());
            var sync_mainacct = bool.Parse(req["sync_mainacct"].ToString());

            session.GetManager().PermissionCheckAccount(account);




            var baseMgr = manager.BaseManager;//获得对应的管理域主管理员
            if (baseMgr.IsAgent())
            {
                if (baseMgr.AgentAccount == null)
                {
                    throw new FutsRspError("代理账户不存在 无法执行出入金操作");
                }

                //普通代理 且没有设置老版代理 则无法执行出入金
                if (baseMgr.AgentAccount.AgentType == EnumAgentType.Normal && (!baseMgr.Permission.r_tradition))
                {
                    throw new FutsRspError("普通代理无权执行出入金操作");
                }
                if (baseMgr.AgentAccount.AgentType == EnumAgentType.SelfOperated)
                {
                    decimal canuse = baseMgr.AgentAccount.StaticEquity - baseMgr.AgentAccount.SubStaticEquity;
                    if (( canuse < Math.Abs(amount)) && amount > 0)//入金且可分配小于入金额 则拒绝
                    {
                        throw new FutsRspError("自营代理可分配权益不足");
                    }
                }
            }


            CashTransaction txn = new CashTransactionImpl();
            txn.Account = account;
            txn.Amount = Math.Abs(amount);
            txn.Comment = comment;
            txn.DateTime = Util.ToTLDateTime();
            txn.EquityType = equity_type;
            txn.Operator = manager.Login;
            txn.Settled = false;
            txn.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            txn.TxnRef = txnref;
            txn.TxnType = amount > 0 ? QSEnumCashOperation.Deposit : QSEnumCashOperation.WithDraw;


            //执行出入金操作
            this.CashOperation(txn);

            //出入金操作后返回帐户信息更新
            session.NotifyMgr("NotifyAccountFinInfo", this[account].GenAccountInfo());
            session.RspMessage("出入金操作成功");
        }




        #region 更新账户模板
        [PermissionRequiredAttr("r_account_edit_template")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountCommissionTemplate", "UpdateAccountCommissionTemplate - update account commission template set", "更新帐户手续费模板")]
        public void CTE_UpdateAccountCommissionTemplate(ISession session, string account, int templateid)
        {
            session.GetManager().PermissionCheckAccount(account);
            this.UpdateAccountCommissionTemplate(account, templateid);
            session.RspMessage("更新帐户手续费模板成功");
        }

        [PermissionRequiredAttr("r_account_edit_template")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountMarginTemplate", "UpdateAccountMarginTemplate - update account margin template set", "更新帐户保证金模板")]
        public void CTE_UpdateAccountMarginTemplate(ISession session, string account, int templateid)
        {
            session.GetManager().PermissionCheckAccount(account);
            this.UpdateAccountMarginTemplate(account, templateid);
            session.RspMessage("更新帐户保证金模板成功");
        }

        [PermissionRequiredAttr("r_account_edit_template")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountExStrategyTemplate", "UpdateAccountExStrategyTemplate - update account exstrategy template set", "更新帐户交易参数模板")]
        public void CTE_UpdateAccountExStrategyTemplate(ISession session, string account, int templateid)
        {
            session.GetManager().PermissionCheckAccount(account);
            this.UpdateAccountExStrategyTemplate(account, templateid);
            session.RspMessage("更新帐户交易参数模板成功");
        }

        [PermissionRequiredAttr("r_account_edit_template")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountConfigTemplate", "UpdateAccountConfigTemplate - update account config template set", "更新帐户配置模板", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountConfigTemplate(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var accounts = req["accounts"].ToObject<string[]>();
            var templateid = int.Parse(req["template_id"].ToString());
            var force = bool.Parse(req["force"].ToString());

            foreach (var account in accounts)
            {
                session.GetManager().PermissionCheckAccount(account);
                this.UpdateAccountConfigTemplate(account, templateid, force);
                System.Threading.Thread.Sleep(100);
            }
            session.RspMessage("更新帐户配置模板成功");
        }

        public void UpdateAccountConfigTemplate(string account, int template_id,bool force)
        {
            this.UpdateAccountConfigTemplate(account, template_id);

            if (force)
            {
                //重置模板
                this.UpdateAccountCommissionTemplate(account, 0);
                this.UpdateAccountMarginTemplate(account, 0);
                this.UpdateAccountExStrategyTemplate(account, 0);
                //删除风控规则
                TLCtxHelper.ModuleRiskCentre.DeleteRiskRule(this[account]);
            }
            //重置风控规则
            TLCtxHelper.ModuleRiskCentre.LoadRiskRule(this[account]);
        }

        #endregion




        


        /// <summary>
        /// 查询交易帐户的Profile
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountProfile", "QryAccountProfile - qry profile account", "查询交易帐户个人信息")]
        public void CTE_QryAccountProfile(ISession session, string account)
        {
            session.GetManager().PermissionCheckAccount(account);

            AccountProfile profile = BasicTracker.AccountProfileTracker[account];

            //如果个人信息不存在 则添加个人信息
            if (profile == null)
            {
                profile = new AccountProfile();
                profile.Account = account;

                BasicTracker.AccountProfileTracker.UpdateAccountProfile(profile);
            }
            session.ReplyMgr(profile);

        }


        /// <summary>
        /// 查询账户财务信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountFinInfo", "QryAccountFinInfo - query account", "查询帐户信息")]
        public void CTE_QryAccountFinInfo(ISession session, string account)
        {
            session.GetManager().PermissionCheckAccount(account);
            session.ReplyMgr(this[account].GenAccountInfo());
            
        }

        /// <summary>
        /// 查询账户登入信息
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountLoginInfo", "QryAccountLoginInfo - query account logininfo", "查看交易帐户密码")]
        public void CTE_QryAccountLoginInfo(ISession session, string account)
        {
            session.GetManager().PermissionCheckAccount(account);
            Protocol.LoginInfo logininfo = new Protocol.LoginInfo();
            logininfo.LoginID = account;
            logininfo.Pass = ORM.MAccount.GetAccountPass(account);
            session.ReplyMgr(logininfo);
           
        }


        #region 历史记录查询
        /// <summary>
        /// 查询交易帐户的出入金记录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountCashTxn", "QueryAccountCashTxn -query account cashtrans", "查询交易帐户出入金记录", QSEnumArgParseType.Json)]
        public void CTE_QueryAccountCashTrans(ISession session, string json)
        {
            var data = json.DeserializeObject();
            string account = data["account"].ToString();
            long start = long.Parse(data["start"].ToString());
            long end = long.Parse(data["end"].ToString());
            session.GetManager().PermissionCheckAccount(account);

            CashTransactionImpl[] trans = ORM.MCashTransaction.SelectHistCashTransactions(account, start, end).ToArray();
            session.ReplyMgrArray(trans);
            
        }

        /// <summary>
        /// 查询交易账户结算单
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountSettlementDetail", "QueryAccountSettlement -query account settlement", "查询交易帐户结算单", QSEnumArgParseType.Json)]
        public void CTE_QueryAccountSettlement(ISession session, string json)
        {
            var data = json.DeserializeObject();
            string account = data["account"].ToString();
            int tradingday = int.Parse(data["tradingday"].ToString());
            session.GetManager().PermissionCheckAccount(account);

            AccountSettlement settlement = ORM.MSettlement.SelectSettlement(account, tradingday);
            if (settlement != null)
            {
                List<string> settlelist = SettlementFactory.GenSettlementFile(settlement, this[account]);
                for (int i = 0; i < settlelist.Count; i++)
                {
                    session.ReplyMgr(settlelist[i].Replace('|', '*'), i == settlelist.Count - 1);
                }
            }
            
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountSettlements", "QueryAccountSettlements -query account settlement ", "查询交易帐户结算单", QSEnumArgParseType.Json)]
        public void CTE_QueryAgentSettlements(ISession session, string json)
        {

            var data = json.DeserializeObject();
            string account = data["account"].ToString();
            int start = int.Parse(data["start"].ToString());
            int end = int.Parse(data["end"].ToString());

            session.GetManager().PermissionCheckAccount(account);

            AccountSettlementImpl[] trans = ORM.MSettlement.SelectSettlements(account, start, end).ToArray();
            session.ReplyMgrArray(trans);

        }


        /// <summary>
        /// 查询交易账户委托记录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountOrder", "QueryAccountOrder -query account order", "查询交易帐户委托", QSEnumArgParseType.Json)]
        public void CTE_QueryAccountOrder(ISession session, string json)
        {

            var data = json.DeserializeObject();
            string account = data["account"].ToString();
            int start = int.Parse(data["start"].ToString());
            int end = int.Parse(data["end"].ToString());
            session.GetManager().PermissionCheckAccount(account);

            var orders = ORM.MTradingInfo.SelectOrders(account, start, end);

            int totalnum = orders.Count();
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    session.ReplyMgr(OrderImpl.Serialize(orders.ElementAt(i)), i == totalnum - 1);
                }
            }
            else
            {
                session.ReplyMgr("");
            }

        }
        /// <summary>
        /// 查询交易账户成交记录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountTrade", "QueryAccountTrade -query account trade", "查询交易帐户成交", QSEnumArgParseType.Json)]
        public void CTE_QueryAccountTrade(ISession session, string json)
        {
            var data = json.DeserializeObject();
            string account = data["account"].ToString();
            int start = int.Parse(data["start"].ToString());
            int end = int.Parse(data["end"].ToString());
            session.GetManager().PermissionCheckAccount(account);

            var trades = ORM.MTradingInfo.SelectTrades(account, start, end);

            int totalnum = trades.Count();
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    session.ReplyMgr(TradeImpl.Serialize(trades.ElementAt(i)), i == totalnum - 1);
                }
            }
            else
            {
                session.ReplyMgr("");
            }
            
        }

        /// <summary>
        /// 查询交易账户结算持仓
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountPosition", "QueryAccountPosition -query account position", "查询交易帐户结算持仓", QSEnumArgParseType.Json)]
        public void CTE_QueryAccountPosition(ISession session, string json)
        {

            var data = json.DeserializeObject();
            string account = data["account"].ToString();
            int tradingday = int.Parse(data["tradingday"].ToString());
            session.GetManager().PermissionCheckAccount(account);

            List<PositionDetail> positions = ORM.MSettlement.SelectAccountPositionDetails(account, tradingday).ToList();
            int totalnum = positions.Count();
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    session.ReplyMgr(PositionDetailImpl.Serialize(positions[i]), i == totalnum - 1);
                }
            }
            else
            {
                session.ReplyMgr("");
            }
            
        }
        #endregion

        #region 其他


        [ContribCommandAttr(QSEnumCommandSource.MessageExchange, "UpdateAccountProfile", "UpdateAccountProfile - update account profile", "更新交易帐户个人信息", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountProfileEx(ISession session, string json)
        {
            var profile = json.DeserializeObject<AccountProfile>();
            IAccount account = TLCtxHelper.ModuleAccountManager[profile.Account];

            if (account != null)
            {
                BasicTracker.AccountProfileTracker.UpdateAccountProfile(profile);
            }
            //触发交易帐户变动事件
            TLCtxHelper.EventAccount.FireAccountChangeEent(account);

            RspContribResponse response = ResponseTemplate<RspContribResponse>.SrvSendRspResponse(session);
            response.ModuleID = session.ContirbID;
            response.CMDStr = session.CMDStr;
            response.IsLast = true;
            response.Result = profile.SerializeObject();

            TLCtxHelper.ModuleExCore.Send(response);
        }

        /// <summary>
        /// 查询交易帐户的Profile
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageExchange, "QryAccountProfile", "QryAccountProfile - qry profile account", "查询交易帐户个人信息")]
        public void CTE_QryAccountProfileEx(ISession session, string account)
        {
            AccountProfile profile = BasicTracker.AccountProfileTracker[account];

            //如果个人信息不存在 则添加个人信息
            if (profile == null)
            {
                profile = new AccountProfile();
                profile.Account = account;

                BasicTracker.AccountProfileTracker.UpdateAccountProfile(profile);
            }

            RspContribResponse response = ResponseTemplate<RspContribResponse>.SrvSendRspResponse(session);
            response.ModuleID = session.ContirbID;
            response.CMDStr = session.CMDStr;
            response.IsLast = true;
            response.Result = profile.SerializeObject();

            TLCtxHelper.ModuleExCore.Send(response);
        }


        #endregion
    }
}
