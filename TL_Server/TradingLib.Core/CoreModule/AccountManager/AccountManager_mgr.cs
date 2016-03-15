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
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AddAccount", "AddAccount - add account", "添加交易帐户", QSEnumArgParseType.Json)]
        public void CTE_AddAccount(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 请求添加交易帐号:{1}", session.AuthorizedID, json));
            
            Manager manager = session.GetManager();
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var category = Util.ParseEnum<QSEnumAccountCategory>(req["category"].ToString());
            var password = req["password"].ToString();
            var routergroup_id = int.Parse(req["routergroup_id"].ToString());
            var user_id = int.Parse(req["user_id"].ToString());
            var manager_id = int.Parse(req["manager_id"].ToString());

            //域帐户数目检查
            if (manager.Domain.GetAccounts().Count() >= manager.Domain.AccLimit)
            {
                throw new FutsRspError("帐户数目达到上限:" + manager.Domain.AccLimit.ToString());
            }

            //如果不是Root权限的Manager需要进行执行权限检查
            if (!manager.IsInRoot())
            {
                //如果不是为该主域添加帐户,则我们需要判断当前Manager的主域是否拥有请求主域的权限
                if (manager.BaseMgrID != manager_id)
                {
                    if (!manager.IsParentOf(manager_id))
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

            Manager targetmgr = BasicTracker.ManagerTracker[manager_id];
            if (targetmgr == null)
            {
                //如果指定的管理域不存在，则默认为当前操作的manager的域
                targetmgr = manager.BaseManager;
            }

            //指定的manager域和当前管理域不一致 则判断当前管理域是否有权在指定的域内开户
            if (targetmgr.BaseMgrID != manager.BaseMgrID)
            {
                if (!manager.RightAccessManager(targetmgr))
                {
                    throw new FutsRspError("无权为该管理员添加帐户");
                }
            }

            AccountCreation create = new AccountCreation();
            create.Account = account;
            create.Category = category;
            create.Password = password;
            //create.RouteGroup = BasicTracker.RouterGroupTracker[routergroup_id];
            create.RouterType = QSEnumOrderTransferType.LIVE;
            create.UserID = user_id;
            //create.Domain = manager.Domain;
            create.BaseManagerID = targetmgr.BaseMgrID;


            //执行操作 并捕获异常 产生异常则给出错误回报
            this.AddAccount(ref create);//将交易帐户加入到主域
            session.OperationSuccess("新增帐户:" + create.Account + "成功");
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AddFinServiceAccount", "AddFinServiceAccount - add finservice account", "添加配资客户", QSEnumArgParseType.Json)]
        public void CTE_AddFinServiceAccount(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 请求添加配资客户帐号:{1}", session.AuthorizedID, json));

            Manager manager = session.GetManager();
            var profile = Mixins.Json.JsonMapper.ToObject<AccountProfile>(json);
            var account = profile.Account;
            QSEnumAccountCategory category = QSEnumAccountCategory.MONITERACCOUNT;
            var password = string.Empty;
            var routergroup_id = 0;
            var user_id = 0;
            var manager_id = manager.BaseManager.ID;

            //域帐户数目检查
            if (manager.Domain.GetAccounts().Count() >= manager.Domain.AccLimit)
            {
                throw new FutsRspError("帐户数目达到上限:" + manager.Domain.AccLimit.ToString());
            }

            //如果不是Root权限的Manager需要进行执行权限检查
            if (!manager.IsInRoot())
            {
                //如果不是为该主域添加帐户,则我们需要判断当前Manager的主域是否拥有请求主域的权限
                if (manager.BaseMgrID != manager_id)
                {
                    if (!manager.IsParentOf(manager_id))
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
            create.Account = account;
            create.Category = category;
            create.Password = password;
            //create.RouteGroup = BasicTracker.RouterGroupTracker[routergroup_id];
            create.RouterType = QSEnumOrderTransferType.SIM;
            create.UserID = user_id;
            //create.Domain = manager.Domain;
            //create.BaseManager = manager.BaseManager;


            //执行操作 并捕获异常 产生异常则给出错误回报
            this.AddAccount(ref create);//将交易帐户加入到主域

            profile.Account = create.Account;//获得添加的交易帐户
            //插入新的profile
            BasicTracker.AccountProfileTracker.UpdateAccountProfile(profile);

            session.OperationSuccess("新增配资客户:" + create.Account + "成功");

        }


        /// <summary>
        /// 统一使用AccountCreation对象创建交易帐户
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AddAccountFacde", "AddAccountFacde - add  account", "添加交易帐号", QSEnumArgParseType.Json)]
        public void CTE_AddAccountFacde(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 请求添加帐号:{1}", session.AuthorizedID, json));

            Manager manager = session.GetManager();
            var creation = Mixins.Json.JsonMapper.ToObject<AccountCreation>(json);
            var account = creation.Account;

            //域帐户数目检查
            if (manager.Domain.GetAccounts().Count() >= manager.Domain.AccLimit)
            {
                throw new FutsRspError("帐户数目达到上限:" + manager.Domain.AccLimit.ToString());
            }

            if (creation.BaseManagerID == 0)
            {
                creation.BaseManagerID = manager.BaseMgrID;
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

            int cnt = manager.GetVisibleAccount().Count();//获得该manger下属的所有帐户数目
            if (cnt >= limit)
            {
                throw new FutsRspError("可开帐户数量超过限制:" + limit.ToString());
            }


            //执行操作 并捕获异常 产生异常则给出错误回报
            this.AddAccount(ref creation);//将交易帐户加入到主域

            //帐户添加完毕后同步添加profile信息
            creation.Profile.Account = creation.Account;

            //插入新的profile
            BasicTracker.AccountProfileTracker.UpdateAccountProfile(creation.Profile);

            //对外触发交易帐号添加事件
            TLCtxHelper.EventAccount.FireAccountAddEvent(this[creation.Account]);

            session.OperationSuccess("新增交易帐号:" + creation.Account + "成功");

        }


        /// <summary>
        /// 查询交易帐户的Profile
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountProfile", "QryAccountProfile - qry profile account", "查询交易帐户个人信息")]
        public void CTE_QryAccountProfile(ISession session, string account)
        {
            Manager mgr = session.GetManager();
            if (mgr == null) throw new FutsRspError("管理员不存在");

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

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountProfile", "UpdateAccountProfile - update account profile", "更新交易帐户个人信息",QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountProfile(ISession session, string json)
        {
            Manager mgr = session.GetManager();
            if (mgr == null) throw new FutsRspError("管理员不存在");

            var profile = Mixins.Json.JsonMapper.ToObject<AccountProfile>(json);
            IAccount account = TLCtxHelper.ModuleAccountManager[profile.Account];

            if (account != null)
            {
                BasicTracker.AccountProfileTracker.UpdateAccountProfile(profile);
            }
            //触发交易帐户变动事件
            TLCtxHelper.EventAccount.FireAccountChangeEent(account);

            session.OperationSuccess("更新个人信息成功");

        }

        /// <summary>
        /// 请求删除交易帐户
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DelAccount", "DelAccount - del account", "删除交易帐户", QSEnumArgParseType.Json)]
        public void CTE_DelAccount(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 请求删除帐户:{1}", session.AuthorizedID, json));

            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            IAccount acc = this[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            //检查交易帐户资金
            if (_deleteAccountCheckEquity && (acc.NowEquity > 1 || acc.Credit > 1))
            {
                throw new FutsRspError(string.Format(string.Format("交易帐户:{0} 权益:{1} 信用额度:{2}未出金 无法删除", account, acc.NowEquity, acc.Credit)));
            }

            this.DelAccount(account);

            session.OperationSuccess("交易帐户:" + account + " 删除成功");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountCategory", "UpdateAccountCategory - change account category", "修改帐户类别", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountCategory(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 请求更新帐户类别:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var category = Util.ParseEnum<QSEnumAccountCategory>(req["category"].ToString());
            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountCategory(account, category);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountExecute", "UpdateAccountExecute - change account execute", "修改帐户交易权限状态", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountExecute(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 请求交易权限类别:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var execute = bool.Parse(req["execute"].ToString());
            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
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

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountIntraday", "UpdateAccountIntraday - change account intraday setting", "修改帐户日内交易属性", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountIntraday(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 请求更新日内交易:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var intraday = bool.Parse(req["intraday"].ToString());

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountIntradyType(account, intraday);
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateRouteType", "UpdateRouteType - change account route type", "修改帐户日路由属性", QSEnumArgParseType.Json)]
        public void CTE_UpdateRouteType(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 请求更新路由类被:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var routetype = Util.ParseEnum<QSEnumOrderTransferType>(req["routertrype"].ToString());

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountRouterTransferType(account, routetype);
            }
        }

        //[ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountInvestor", "UpdateAccountInvestor - update account investor info", "修改交易帐号投资者信息", QSEnumArgParseType.Json)]
        //public void CTE_UpdateAccountInvestor(ISession session, string json)
        //{
        //    logger.Info(string.Format("管理员:{0} 请求修改投资者信息:{1}", session.AuthorizedID, json));
        //    var req = Mixins.Json.JsonMapper.ToObject(json);
        //    var account = req["account"].ToString();
        //    var name = req["name"].ToString();
        //    var broker = req["broker"].ToString();
        //    var bank_id = int.Parse(req["bank_id"].ToString());
        //    var bank_ac = req["bank_ac"].ToString();

        //    IAccount acct = TLCtxHelper.ModuleAccountManager[account];
        //    if (acct != null)
        //    {
        //        this.UpdateInvestorInfo(account, name, broker, bank_id, bank_ac);
        //    }
        //}

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountCurrency", "UpdateAccountCurrency - update account currency", "更新交易帐户货币类别", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountCurrency(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 请求修改投资者信息:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var currency = (CurrencyType)Enum.Parse(typeof(CurrencyType),req["currency"].ToString());

            Manager mgr = session.GetManager();
            
            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            if (mgr == null || (!mgr.RightAccessAccount(acct)))
            {
                throw new FutsRspError("无权操作交易帐户");
            }

            this.UpdateAccountCurrency(account, currency);

            session.OperationSuccess("交易帐户更新货币类型成功");
        }



        /// <summary>
        /// 交易账户 出入金
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AccountCashOperation", "AccountCashOperation - account cash operation", "给交易帐户出入去金", QSEnumArgParseType.Json)]
        public void CTE_CashOperation(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 请求出入金操作:{1}", session.AuthorizedID, json));
            Manager manager = session.GetManager();

            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var amount = decimal.Parse(req["amount"].ToString());
            var txnref = req["txnref"].ToString();
            var comment = req["comment"].ToString();
            var equity_type = Util.ParseEnum<QSEnumEquityType>(req["equity_type"].ToString());
            var sync_mainacct = bool.Parse(req["sync_mainacct"].ToString());

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            HandlerMixins.Valid_ObjectNotNull(acct);

            Manager manger = session.GetManager();

            if (!manager.RightAccessAccount(acct))
            {
                throw new FutsRspError("无权操作该帐户");
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

            ////主帐户监控
            //if (TLCtxHelper.Version.ProductType == QSEnumProductType.VendorMoniter)
            //{
            //    //同步出入金操作到主帐户
            //    if (sync_mainacct)
            //    {
            //        IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(account);

            //        if (broker == null)
            //        {
            //            throw new FutsRspError("未绑定主帐户,无法同步出入金操作到底层帐户");
            //        }

            //        if (broker is TLBroker)
            //        {
            //            TLBroker b = broker as TLBroker;
            //            if (amount > 0)
            //            {
            //                //入金
            //                b.Deposit((double)Math.Abs(amount), "");
            //            }
            //            else
            //            {
            //                //出金
            //                b.Withdraw((double)Math.Abs(amount), "");
            //            }
            //            //session.OperationSuccess("出金操作已提交,请查询主帐户信息");
            //        }
            //    }
            //}
            //出入金操作后返回帐户信息更新
            session.NotifyMgr("NotifyAccountFinInfo", acct.GenAccountInfo());
            session.OperationSuccess("出入金操作成功");
        }






        /// <summary>
        /// @修改交易帐户密码
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manger"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountPass", "UpdateAccountPass - update account password", "更新交易帐户交易密码", QSEnumArgParseType.Json)]
        public void CTE_ChangePassword(ISession session, string json)
        {
            logger.Info(string.Format("管理员:{0} 请求修改交易密码:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var newpass = req["newpass"].ToString();

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountPass(account, newpass);
                session.OperationSuccess("修改密码成功");
            }
            else
            {
                throw new FutsRspError("交易帐户不存在");
            }
        }












        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountCommissionTemplate", "UpdateAccountCommissionTemplate - update account commission template set", "更新帐户手续费模板")]
        public void CTE_UpdateAccountCommissionTemplate(ISession session, string account, int templateid)
        {
            Manager manager = session.GetManager();
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("无权修改该交易帐户");
            }

            //更新路由组
            this.UpdateAccountCommissionTemplate(account, templateid);
            session.OperationSuccess("更新帐户手续费模板成功");
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountMarginTemplate", "UpdateAccountMarginTemplate - update account margin template set", "更新帐户保证金模板")]
        public void CTE_UpdateAccountMarginTemplate(ISession session, string account, int templateid)
        {
            Manager manager = session.GetManager();
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("无权修改该交易帐户");
            }

            //更新路由组
            this.UpdateAccountMarginTemplate(account, templateid);
            session.OperationSuccess("更新帐户保证金模板成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountExStrategyTemplate", "UpdateAccountExStrategyTemplate - update account exstrategy template set", "更新帐户交易参数模板")]
        public void CTE_UpdateAccountExStrategyTemplate(ISession session, string account, int templateid)
        {
            Manager manager = session.GetManager();
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("无权修改该交易帐户");
            }

            //更新路由组
            this.UpdateAccountExStrategyTemplate(account, templateid);
            session.OperationSuccess("更新帐户交易参数模板成功");
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
            session.OperationSuccess("更新帐户路由组成功");
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountFinInfo", "QryAccountFinInfo - query account", "查询帐户信息")]
        public void CTE_QryAccountFinInfo(ISession session, string account)
        {
            Manager manager = session.GetManager();
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (manager.RightAccessAccount(acc))
            {
                session.ReplyMgr(acc.GenAccountInfo());
            }
            else
            {
                throw new FutsRspError("无权查看该帐户信息");
            }
        }

        /// <summary>
        /// 查询分区管理员信息
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountLoginInfo", "QryAccountLoginInfo - query account logininfo", "查看交易帐户密码")]
        public void CTE_QryAccountLoginInfo(ISession session, string account)
        {
            Manager manager = session.GetManager();

            
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }
            if (manager.RightAccessAccount(acc))
            {
                Protocol.LoginInfo logininfo = new Protocol.LoginInfo();
                logininfo.LoginID = account;
                logininfo.Pass = ORM.MAccount.GetAccountPass(account);
                session.ReplyMgr(logininfo);
            }
            else
            {
                throw new FutsRspError("无权查看帐户");
            }
            
        }

    }
}
