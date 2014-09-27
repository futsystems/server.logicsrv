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
            if (!HaveAccount(account)) return;
            this[account].OrderRouteType = type;
            ORM.MAccount.UpdateAccountRouterTransferType(account, type);
            AccountChanged(this[account]);
        }

        public void UpdateInvestorInfo(string account, string name,string broker,string bank,string bankac)
        {
            if (!HaveAccount(account)) return;
            IAccount acc = this[account];
            acc.Name = name;
            acc.Broker = broker;
            acc.BankAC = bankac;
            acc.BankID = bank;
            ORM.MAccount.UpdateInvestorInfo(account, name,broker,bank,bankac);
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

        public void UpdateAccountPosLock(string account, bool poslock)
        {
            if (!HaveAccount(account)) return;
            this[account].PosLock = poslock;
            ORM.MAccount.UpdateAccountPosLock(account, poslock);
            AccountChanged(this[account]);
        }
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
        /// 交易账户的资金操作
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        public override void CashOperation(string account, decimal amount,string transref, string comment)
        {
            if (CoreUtil.IsSettle2Reset())
            {
                debug("Account:" + account + " 资金操作:" + amount.ToString() + " comment:" + comment + "被忽略", QSEnumDebugLevel.WARNING);
                FutsRspError error = new FutsRspError();
                error.FillError("CASHOPERATION_NOT_ALLOW_NOW");//当前时间不允许出入金
                throw error;
            }

            debug("CashOperation ID:" + account + " Amount:" + amount.ToString() + " Comment:" + comment, QSEnumDebugLevel.INFO);
            IAccount acc = this[account];
            {
                if (acc == null)
                {
                    FutsRspError error = new FutsRspError();
                    error.FillError("TRADING_ACCOUNT_NOT_FOUND");//当前时间不允许出入金
                    throw error;
                }
            }

            if (amount > 0)
            {
                acc.Deposit(amount);
            }
            else
            {
                acc.Withdraw(Math.Abs(amount));
            }
            ORM.MAccount.CashOperation(account, amount, transref, comment);
        }
        /// <summary>
        /// web管理所用到的出入金操作 并返回对应信息
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CashOperationSafe(string accid, decimal amount, string comment, out string msg)
        {
            try
            {
                if (CoreUtil.IsSettle2Reset())
                {
                    msg = "下午15:30-16:15 无法出入金";
                    return false;
                }

                IAccount acc = this[accid];
                if (acc == null)
                {
                    msg = "无该交易帐号";
                    return false;

                }

                if (ORM.MAccount.IsTransRefExist(accid,comment))
                {
                    msg = "该资金操作的ref_ID已经提交,请勿重复提交";
                    return false;
                }

                if (amount > 0)
                {
                    acc.Deposit(amount);
                }
                else
                {
                    acc.Withdraw(amount);
                }


                if (ORM.MAccount.CashOperation(acc.ID, amount,"",comment))
                {
                    msg = "";
                    return true;
                }
                else
                {
                    msg = "数据库操作异常";
                    if (amount > 0)
                    {
                        //acc.CashIn = acc.CashIn - ammount;
                        acc.Deposit(amount);
                    }
                    else
                    {
                        //acc.CashOut = acc.CashOut - Math.Abs(ammount);
                        acc.Withdraw(amount);
                    }
                    return false;
                }

            }
            catch (Exception ex)
            {
                debug("资金操作异常:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                msg = "资金操作异常";
                return false;
            }

        }
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
                bool re = this.AddAccount(out account, "0", "", "123456", QSEnumAccountCategory.SIMULATION);
                if (re)
                {
                    this.BindAccountMAC(account, mac);
                    debug("创建帐号成功 account:" + account + " mac:" + mac, QSEnumDebugLevel.INFO);
                    return account;
                }
                else
                {
                    debug("创建帐号失败", QSEnumDebugLevel.INFO);
                    return null;
                }
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
            CashOperation(account, netchange,"", "System Reset");
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
            debug("加载帐户数据...", QSEnumDebugLevel.INFO);
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
                    CheckAccountExecute(acc);
                    //2.如果缓存中没有该账户,则加入该账户
                    if (!HaveAccount(acc.ID) && needLoad(acc))
                        CacheAccount(acc);
                }
            }
            catch (Exception ex)
            {
                TLCtxHelper.Debug(Util.GlobalPrefix + ex.ToString());
                throw (new QSClearCentreLoadAccountError(ex, "ClearCentre加载账户:" + account + "异常"));
            }
        }


        bool needLoad(IAccount account)
        {
            switch (_loadmode)
            {
                case QSEnumAccountLoadMode.ALL:
                    return true;
                //实盘加载 只加载实盘帐户
                case QSEnumAccountLoadMode.REAL:
                    return ExUtil.IsRealAccount(account);

                //模拟盘加载 只加载模拟盘帐户
                case QSEnumAccountLoadMode.SIM:
                    return !ExUtil.IsRealAccount(account);
                default:
                    return false;
            }
        }

        /// <summary>
        /// 检查账户是否应该被冻结
        /// </summary>
        /// <param name="account"></param>
        void CheckAccountExecute(IAccount account)
        {
            //account.Execute = true;
            ////初赛账户，报名后
            //if (account.RaceStatus == QSEnumAccountRaceStatus.INPRERACE)
            //{
            //    //报名后账户就被冻结,当重行启动或者加载时，需要检测，需要一直冻结到结算。这样才可以保证结算时为25万。
            //    if (account.SettleDateTime <= account.RaceEntryTime)
            //        account.Execute = false;
            //}
        }

        /// <summary>
        /// 为某个user_id添加某个类型的帐号 密码为pass
        /// 默认mgr_fk为0 如果为0则通过ManagerTracker获得Root的mgr_fk 将默认帐户统一挂在Root用户下
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="type"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public bool AddAccount(out string account, string user_id,string setaccount,string pass, QSEnumAccountCategory type,int mgr_fk=0)
        {
            debug("清算中心为user:" + user_id + " 添加交易帐号到主柜员ID:"+mgr_fk.ToString(), QSEnumDebugLevel.INFO);
            account = null;
            mgr_fk  = (mgr_fk == 0 ? BasicTracker.ManagerTracker.GetRootFK() : mgr_fk);
            bool re = ORM.MAccount.AddAccount(out account, user_id, setaccount,pass, type,mgr_fk);

            //如果添加成功则将该账户加载到内存
            if (re)
            {
                //加载该账户数据
                LoadAccount(account);

                switch (type)
                {
                    //如果是模拟交易帐号则重置资金到模拟初始资金
                    case QSEnumAccountCategory.SIMULATION:
                    case QSEnumAccountCategory.DEALER:
                        {
                            //初始化账户权益资金
                            ResetEquity(account, simAmount);
                            break;
                        }
                    default:
                        break;

                }
                //对外触发交易帐号添加事件
                if (this.AccountAddEvent != null)
                    this.AccountAddEvent(account);

            }
            return re;
        }

        
        #endregion


        public IEnumerable<Position> GetPositions(string account)
        {
            return acctk.GetPositionBook(account);
        }
    }
}
