using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;

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
            debug(string.Format("管理员:{0} 请求添加交易帐号:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);

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


            AccountCreation create = new AccountCreation();
            create.Account = account;
            create.Category = category;
            create.Password = password;
            create.RouteGroup = BasicTracker.RouterGroupTracker[routergroup_id];
            create.RouterType = create.Category == QSEnumAccountCategory.SIMULATION ? QSEnumOrderTransferType.SIM : QSEnumOrderTransferType.LIVE;
            create.UserID = user_id;
            create.Domain = manager.Domain;
            create.BaseManager = manager.BaseManager;


            //执行操作 并捕获异常 产生异常则给出错误回报
            this.AddAccount(ref create);//将交易帐户加入到主域
            session.OperationSuccess("新增帐户:" + create.Account + "成功");
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
            debug(string.Format("管理员:{0} 请求删除帐户:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);

            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
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
            debug(string.Format("管理员:{0} 请求更新帐户类别:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
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
            debug(string.Format("管理员:{0} 请求交易权限类别:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
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
            debug(string.Format("管理员:{0} 请求更新日内交易:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
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
            debug(string.Format("管理员:{0} 请求更新路由类被:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var routetype = Util.ParseEnum<QSEnumOrderTransferType>(req["routertrype"].ToString());

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountRouterTransferType(account, routetype);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountInvestor", "UpdateAccountInvestor - update account investor info", "修改交易帐号投资者信息", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountInvestor(ISession session, string json)
        {
            debug(string.Format("管理员:{0} 请求修改投资者信息:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var name = req["name"].ToString();
            var broker = req["broker"].ToString();
            var bank_id = int.Parse(req["bank_id"].ToString());
            var bank_ac = req["bank_ac"].ToString();

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateInvestorInfo(account, name, broker, bank_id, bank_ac);
            }
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AccountCashOperation", "AccountCashOperation - account cash operation", "给交易帐户出入去金", QSEnumArgParseType.Json)]
        public void CTE_CashOperation(ISession session, string json)
        {
            debug(string.Format("管理员:{0} 请求出入金操作:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
            Manager manager = session.GetManager();

            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var amount = decimal.Parse(req["amount"].ToString());
            var txnref = req["txnref"].ToString();
            var comment = req["comment"].ToString();

            //var routetype = Util.ParseEnum<QSEnumOrderTransferType>(req["routertrype"].ToString());

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            HandlerMixins.Valid_ObjectNotNull(acct);

            Manager manger = session.GetManager();

            if (!manager.RightAccessAccount(acct))
            {
                throw new FutsRspError("无权操作该帐户");
            }

            //执行出入金操作
            this.CashOperation(account, amount, txnref, comment);

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
            debug(string.Format("管理员:{0} 请求修改交易密码:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
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


        /// <summary>
        /// 查询分区管理员信息
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountLoginInfo", "QryAccountLoginInfo - query account logininfo", "查看交易帐户密码")]
        public void CTE_QryAccountLoginInfo(ISession session, string account)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                IAccount acc = TLCtxHelper.ModuleAccountManager[account];
                if (acc == null)
                {
                    throw new FutsRspError("交易帐户不存在");
                }
                Protocol.LoginInfo logininfo = new Protocol.LoginInfo();
                logininfo.LoginID = account;
                logininfo.Pass = ORM.MAccount.GetAccountPass(account);
                session.ReplyMgr(logininfo);
            }
        }
















        //#endregion

    }
}
