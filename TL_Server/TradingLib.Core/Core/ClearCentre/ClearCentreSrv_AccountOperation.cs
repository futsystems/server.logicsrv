using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 帐户的操作 设置 修改密码等
    /// </summary>
    public partial class ClearCentre
    {
        #region *【IAccountOperation abstract覆写】 修改账户相应设置,查询可开,修改密码,验证交易账户,出入金,激活禁止交易账户等
        /// <summary>
        /// 更改账户密码
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public override void ChangeAccountPass(string account, string pass)
        {
            if (!HaveAccount(account)) return;
            ORM.MAccount.UpdateAccountPass(account, pass);
        }
        /// <summary>
        /// 更新账户类别
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ca"></param>
        public override void UpdateAccountCategory(string account, QSEnumAccountCategory ca)
        {
            if (!HaveAccount(account)) return;
            this[account].Category = ca;
            ORM.MAccount.UpdateAccountCategory(account, ca);
            AccountChanged(this[account]);
        }
        /// <summary>
        /// 修改账户类型
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="type"></param>
        public override void UpdateAccountRouterTransferType(string account, QSEnumOrderTransferType type)
        {
            IAccount acct = this[account];
            if(acct == null) return;
            
            //记录原来路由类别
            QSEnumOrderTransferType oldrouter = acct.OrderRouteType;

            //修改交易帐户路由类别
            //原来路由是实盘 需要将原来实盘上的挂单撤掉 同时平掉实盘上的持仓 然后将挂单挂到模拟上
            if (oldrouter == QSEnumOrderTransferType.LIVE)
            {
                //遍历所有持仓
                foreach (Position pos in acct.Positions.Where(p => !p.isFlat))
                {
                    
                }
            }


            acct.OrderRouteType = type;
            ORM.MAccount.UpdateAccountRouterTransferType(account, type);
            AccountChanged(acct);

            

        }

        public void UpdateInvestorInfo(string account, string name,string broker,int  bankfk,string bankac)
        {
            if (!HaveAccount(account)) return;
            IAccount acc = this[account];
            acc.Name = name;
            acc.Broker = broker;
            acc.BankAC = bankac;
            acc.BankID = bankfk;
            ORM.MAccount.UpdateInvestorInfo(account, name, broker, bankfk, bankac);
            AccountChanged(this[account]);
        }

        /// <summary>
        /// 更新交易帐户的ManagerID
        /// </summary>
        /// <param name="account"></param>
        /// <param name="id"></param>
        public void UpdateManagerID(string account, int id)
        {
            if (!HaveAccount(account)) return;
            IAccount acc = this[account];
            acc.Mgr_fk = id;
            ORM.MAccount.UpdateManagerID(account, id);
            AccountChanged(this[account]);
        }

        /// <summary>
        /// 更新交易账户交易参数模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="id"></param>
        public void UpdateAccountExStrategyTemplate(string account, int id)
        {
            if (!HaveAccount(account)) return;
            IAccount acc = this[account];
            acc.ExStrategy_ID = id;
            ORM.MAccount.UpdateAccountExStrategyTemplate(account, id);
            AccountChanged(this[account]);
        }
        //public void UpdateAccountPosLock(string account, bool poslock)
        //{
        //    if (!HaveAccount(account)) return;
        //    this[account].PosLock = poslock;
        //    ORM.MAccount.UpdateAccountPosLock(account, poslock);
        //    AccountChanged(this[account]);
        //}

        /// <summary>
        /// 更新手续费模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public void UpdateAccountCommissionTemplate(string account, int templateid)
        {
            if (!HaveAccount(account)) return;
            this[account].Commission_ID = templateid;
            ORM.MAccount.UpdateAccountCommissionTemplate(account, templateid);
            AccountChanged(this[account]);
        }

        /// <summary>
        /// 更新保证金模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public void UpdateAccountMarginTemplate(string account, int templateid)
        {
            if (!HaveAccount(account)) return;
            this[account].Margin_ID = templateid;
            ORM.MAccount.UpdateAccountMarginTemplate(account, templateid);
            AccountChanged(this[account]);
        }

        /// <summary>
        /// 更新帐户是否分开显示信用额度
        /// </summary>
        /// <param name="account"></param>
        /// <param name="creditseparate"></param>
        //public void UpdateAccountCreditSeparate(string account, bool creditseparate)
        //{
        //    if (!HaveAccount(account)) return;
        //    this[account].CreditSeparate = creditseparate;
        //    ORM.MAccount.UpdateAccountCreditSeparate(account, creditseparate);
        //    AccountChanged(this[account]);
        //}

        //public void UpdateAccountSideMargin(string account, bool sidemargin)
        //{
        //    if (!HaveAccount(account)) return;
        //    this[account].SideMargin = sidemargin;
        //    ORM.MAccount.UpdateAccountSideMargin(account, sidemargin);
        //    AccountChanged(this[account]);
        //}

        /// <summary>
        /// 更新账户日内交易设置
        /// </summary>
        /// <param name="account"></param>
        /// <param name="intraday"></param>
        public override void UpdateAccountIntradyType(string account, bool intraday)
        {
            if (!HaveAccount(account)) return;
            this[account].IntraDay = intraday;
            ORM.MAccount.UpdateAccountInterday(account, intraday);
            AccountChanged(this[account]);
        }

        public void UpdateRouterGroup(string account, int gid)
        {
            if (!HaveAccount(account)) return;
            this[account].RG_FK = gid;
            ORM.MAccount.UpdateRouterGroup(account, gid);
            AccountChanged(this[account]);

        }
        /// <summary>
        /// 激活某个交易账户 允许其进行交易
        /// 某个账户激活后需要调用风控中心重新加载该账户的风控规则，使得风控规则复位
        /// </summary>
        /// <param name="id"></param>
        public override void ActiveAccount(string id)
        {
            debug("激活帐户:" + id, QSEnumDebugLevel.INFO);
            if (!HaveAccount(id)) return;
            this[id].Execute = true;
            if (AccountActiveEvent != null)
                AccountActiveEvent(id);
            AccountChanged(this[id]);
        }
        /// <summary>
        /// 禁止某个账户进行交易
        /// </summary>
        /// <param name="id"></param>
        public override void InactiveAccount(string id)
        {
            debug("冻结账户:" + id, QSEnumDebugLevel.INFO);
            if (!HaveAccount(id)) return;
            this[id].Execute = false;
            if (AccountInActiveEvent != null)
                AccountInActiveEvent(id);
            AccountChanged(this[id]);
        }

        /// <summary>
        /// 更新帐户路由组
        /// </summary>
        /// <param name="account"></param>
        /// <param name="rg"></param>
        public void UpdateRouterGroup(string account,RouterGroup rg)
        {
            debug("修改帐户路由组为:" + rg.Name,QSEnumDebugLevel.INFO);
            if (!HaveAccount(account)) return;
            this[account].RG_FK = rg.ID;
            ORM.MAccount.UpdateRouterGroup(account, rg.ID);
            AccountChanged(this[account]);
            
        }


        /// <summary>
        /// 交易账户的资金操作
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        public override void CashOperation(string account, decimal amount, QSEnumEquityType equitytype, string transref, string comment)
        {

            debug("CashOperation ID:" + account + " Amount:" + amount.ToString() + " Comment:" + comment, QSEnumDebugLevel.INFO);
            IAccount acc = this[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            //帐户自有资金的出入金操作
            if (equitytype == QSEnumEquityType.OwnEquity)
            {
                //金额检查
                if (amount < 0)
                {
                    if (acc.NowEquity < Math.Abs(amount))
                    {
                        throw new FutsRspError("出金额度大于帐户权益");
                    }
                }

                //执行时间检查 
                if (TLCtxHelper.Ctx.SettleCentre.IsInSettle)
                {
                    throw new FutsRspError("系统正在结算,禁止出入金操作");
                }

                if (amount > 0)
                {
                    acc.Deposit(amount);
                }
                else
                {
                    acc.Withdraw(Math.Abs(amount));
                }
                ORM.MAccount.CashOperation(account, amount, QSEnumEquityType.OwnEquity, transref, comment);
            }

            if (equitytype == QSEnumEquityType.CreditEquity)
            {
                if (amount < 0)
                {
                    if (acc.Credit < Math.Abs(amount))
                    {
                        throw new FutsRspError("出金额度大于优先资金权益");
                    }
                }

                //执行时间检查 
                if (TLCtxHelper.Ctx.SettleCentre.IsInSettle)
                {
                    throw new FutsRspError("系统正在结算,禁止出入金操作");
                }

                if (amount > 0)
                {
                    acc.CreditDeposit(Math.Abs(amount));
                }
                else
                {
                    acc.CreditWithdraw(Math.Abs(amount));
                }
                ORM.MAccount.CashOperation(account, amount, QSEnumEquityType.CreditEquity, transref, comment);

                TLCtxHelper.EventAccount.FireAccountCashOperationEvent(acc.ID, amount > 0 ? QSEnumCashOperation.Deposit : QSEnumCashOperation.WithDraw, Math.Abs(amount));
            }

        }
        ///// <summary>
        ///// web管理所用到的出入金操作 并返回对应信息
        ///// </summary>
        ///// <param name="accid"></param>
        ///// <param name="amount"></param>
        ///// <param name="comment"></param>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //public bool CashOperationSafe(string accid, decimal amount, string comment, out string msg)
        //{
        //    try
        //    {
        //        if (TLCtxHelper.Ctx.SettleCentre.IsInSettle)
        //        {
        //            msg = "下午15:30-16:15 无法出入金";
        //            return false;
        //        }

        //        IAccount acc = this[accid];
        //        if (acc == null)
        //        {
        //            msg = "无该交易帐号";
        //            return false;

        //        }

        //        if (ORM.MAccount.IsTransRefExist(accid,comment))
        //        {
        //            msg = "该资金操作的ref_ID已经提交,请勿重复提交";
        //            return false;
        //        }

        //        if (amount > 0)
        //        {
        //            acc.Deposit(amount);
        //        }
        //        else
        //        {
        //            acc.Withdraw(amount);
        //        }


        //        if (ORM.MAccount.CashOperation(acc.ID, amount,"",comment))
        //        {
        //            msg = "";
        //            return true;
        //        }
        //        else
        //        {
        //            msg = "数据库操作异常";
        //            if (amount > 0)
        //            {
        //                acc.Deposit(amount);
        //            }
        //            else
        //            {
        //                acc.Withdraw(amount);
        //            }
        //            return false;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        debug("资金操作异常:" + ex.ToString(), QSEnumDebugLevel.ERROR);
        //        msg = "资金操作异常";
        //        return false;
        //    }

        //}
        /// <summary>
        /// 验证某个交易账户是否有效
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public bool VaildAccount(string account, string pass)
        {
            bool v = ORM.MAccount.ValidAccount(account, pass);
            v = v && HaveAccount(account);//检查风控中心是否记录该账号
            return v;
        }


        public void BindAccountMAC(string account, string mac)
        {
            if (!HaveAccount(account)) return;
            this[account].MAC= mac;
            bool v = ORM.MAccount.UpdateAccountMAC(account, mac);
        }

        /// <summary>
        /// 通过硬件号码验证帐户 如果帐户不存在则添加帐号并绑定MAC
        /// 返回Account
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        public string ValidAccountViaMAC(string mac)
        {
            string account = string.Empty;
            IAccount acc = this.Accounts.Where(a => a.MAC.Equals(mac)).SingleOrDefault();
            if (acc == null)
            {
                debug("与MAC地址绑定的帐号不存在", QSEnumDebugLevel.INFO);
                AccountCreation create = new AccountCreation();
                this.AddAccount(ref create);
                this.BindAccountMAC(account, mac);
                debug("创建帐号成功 account:" + account + " mac:" + mac, QSEnumDebugLevel.INFO);
                return account;
            }
            else
            {
                return acc.ID;
            }
        }

        /// <summary>
        /// 重置资金
        /// 将某个账户的资金重置到多少数额
        /// </summary>
        /// <param name="account"></param>
        /// <param name="value"></param>
        public override void ResetEquity(string account, decimal value)
        {
            IAccount a;
            if (!HaveAccount(account, out a)) return;
            decimal nowequity = a.NowEquity;
            decimal netchange = value - nowequity;
            CashOperation(account, netchange,QSEnumEquityType.OwnEquity,"", "System Reset");
        }
        #endregion

        #region 【加载账户,添加账户,删除账户】

        /// <summary>
        ///获得某个账户的user_id(网站pk)
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public int GetAccountUserID(string account)
        {
            return ORM.MAccount.GetAccountUserID(account);
        }


        /// <summary>
        /// 从数据库加载某accID的交易账户
        /// </summary>
        /// <param name="accID"></param>
        private void LoadAccount(string account = null)
        {
            debug("Loading accounts form database.....", QSEnumDebugLevel.INFO);
            try
            {
                IList<IAccount> accountlist = new List<IAccount>();
                if (string.IsNullOrEmpty(account))
                {
                    accountlist = ORM.MAccount.SelectAccounts();
                }
                else
                {
                    IAccount acc = ORM.MAccount.SelectAccount(account);
                    if (acc != null)
                    {
                        accountlist.Add(acc);
                    }
                }
                foreach (IAccount acc in accountlist)
                {
                    //1.检查该帐户当前是否可以交易
                    //CheckAccountExecute(acc);
                    //2.如果缓存中没有该账户,则加入该账户
                    if (!HaveAccount(acc.ID))
                        CacheAccount(acc);
                }
            }
            catch (Exception ex)
            {
                Util.Debug(Util.GlobalPrefix + ex.ToString());
                throw (new QSClearCentreLoadAccountError(ex, "ClearCentre加载账户:" + account + "异常"));
            }
        }

        /// <summary>
        /// 交易帐号只能是数字或字母
        /// </summary>
        System.Text.RegularExpressions.Regex regaccount = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9-]+$");
        

        /// <summary>
        /// 为某个user_id添加某个类型的帐号 密码为pass
        /// 默认mgr_fk为0 如果为0则通过ManagerTracker获得Root的mgr_fk 将默认帐户统一挂在Root用户下
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="type"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public void AddAccount(ref AccountCreation create)
        {
            debug("清算中心为user:" + create.UserID.ToString() + " 添加交易帐号到主柜员ID:" + create.BaseManager.ID.ToString(), QSEnumDebugLevel.INFO);

            if (create.Domain == null)
            {
                throw new FutsRspError("帐户所属域参数不正确");
            }
            if (create.BaseManager == null)
            {
                throw new FutsRspError("帐户所属柜员参数不正确");
            }

            //如果给定了交易帐号 则我们需要检查交易帐号是否是字母或数字
            if (!string.IsNullOrEmpty(create.Account))
            {
                if (!regaccount.IsMatch(create.Account))
                {
                    throw new FutsRspError("交易帐号只能包含数字,字母,-");
                }
                if (create.Account.Length > 20)
                {
                    throw new FutsRspError("交易帐号长度不能超过20位");
                }
            }


            ORM.MAccount.AddAccount(ref create);

            //如果添加成功则将该账户加载到内存
            LoadAccount(create.Account);

            //对外触发交易帐号添加事件
            if (this.AccountAddEvent != null)
                this.AccountAddEvent(create.Account);
        }


        public void DelAccount(string account)
        {
            debug("清算中心删除交易帐户:" + account, QSEnumDebugLevel.INFO);
            IAccount acc = this[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐号不存在");
            }

            try
            {
                //删除数据库
                ORM.MAccount.DelAccount(account);//删除数据库记录
                DropAccount(acc);//删除内存记录
                //对外触发交易帐户删除事件
                if (AccountDelEvent != null)
                    AccountDelEvent(account);
                acc.Deleted = true;
                AccountChanged(acc);

                
            }
            catch (Exception ex)
            {
                debug("删除交易帐户错误:" + ex.ToString());
                throw new FutsRspError("删除交易帐户异常，请手工删除相关信息");
            }
        }

        
        #endregion
    }
}
