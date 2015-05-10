using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    [CoreAttr(AccountManager.CoreName, "交易帐户管理模块", "交易帐户管理模块用于管理交易账户，添加，删除，修改，加载等")]
    public partial class AccountManager : BaseSrvObject, IModuleAccountManager
    {
        const string CoreName = "AccountManager";
        public string CoreId { get { return this.PROGRAME; } }

        protected ConcurrentDictionary<string, IAccount> AcctList = new ConcurrentDictionary<string, IAccount>();


        public AccountManager():
            base(AccountManager.CoreName)
        {
            LoadAccount();
            
        }
        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
        }

        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
        }

        /// <summary>
        /// 按交易帐户编号 获得交易帐号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IAccount this[string id]
        {
            get
            {
                if (string.IsNullOrEmpty(id)) return null;
                IAccount ac = null;
                AcctList.TryGetValue(id, out ac);
                return ac;
            }
        }

        /// <summary>
        /// 获得所有帐户对象
        /// </summary>
        public IEnumerable<IAccount> Accounts
        {
            get
            {
                return AcctList.Values;
            }
        }

        ///// <summary>
        ///// 查询是否有某个ID的账户
        ///// </summary>
        ///// <param name="a"></param>
        ///// <returns></returns>
        public bool HaveAccount(string account)
        {
            if (AcctList.ContainsKey(account))
                return true;
            else
                return false;
        }


        /// <summary>
        /// 账户属性变动事件
        /// </summary>
        /// <param name="account"></param>
        protected void AccountChanged(string account)
        {
            TLCtxHelper.EventAccount.FireAccountChangeEent(account);
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
            logger.Info("清算中心为user:" + create.UserID.ToString() + " 添加交易帐号到主柜员ID:" + create.BaseManagerID.ToString());

            Manager mgr = BasicTracker.ManagerTracker[create.BaseManagerID];

            if (mgr == null)
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

            //数据库添加交易帐户
            ORM.MAccount.AddAccount(ref create);

            //如果添加成功则将该账户加载到内存
            LoadAccount(create.Account);

            //对外触发交易帐号添加事件
            TLCtxHelper.EventAccount.FireAccountAddEvent(create.Account);
        }


        public void DelAccount(string account)
        {
            logger.Info("清算中心删除交易帐户:" + account);
            IAccount acc = this[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐号不存在");
            }

            try
            {
                //删除数据库
                ORM.MAccount.DelAccount(account);//删除数据库记录
                //删除内存记录
                TLCtxHelper.ModuleClearCentre.DropAccount(acc);
                //对外触发交易帐户删除事件
                TLCtxHelper.EventAccount.FireAccountDelEvent(account);
                acc.Deleted = true;
                AccountChanged(account);
            }
            catch (Exception ex)
            {
                logger.Error("删除交易帐户错误:" + ex.ToString());
                throw new FutsRspError("删除交易帐户异常，请手工删除相关信息");
            }
        }


        /// <summary>
        /// 从数据库加载某accID的交易账户
        /// </summary>
        /// <param name="accID"></param>
        private void LoadAccount(string account = null)
        {
            logger.Info("Loading accounts form database.....");
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
                    AcctList.TryAdd(acc.ID, acc);
                    
                    //获得帐户昨日权益 通过查找昨日结算记录中的结算权益来恢复
                    acc.LastEquity = ORM.MAccount.GetSettleEquity(acc.ID, TLCtxHelper.ModuleSettleCentre.LastSettleday);

                    //恢复昨日权益以及今日出入金数据
                    //这里累计NextTradingday的出入金数据 恢复到当前状态,结算之后的所有交易数据都归入以结算日为基础计算的下一个交易日
                    acc.Deposit(ORM.MAccount.CashInOfTradingDay(acc.ID, QSEnumEquityType.OwnEquity, TLCtxHelper.ModuleSettleCentre.NextTradingday));
                    acc.Withdraw(ORM.MAccount.CashOutOfTradingDay(acc.ID, QSEnumEquityType.OwnEquity, TLCtxHelper.ModuleSettleCentre.NextTradingday));

                    
                    //获得上个结算日的优先资金
                    acc.LastCredit = ORM.MAccount.GetSettleCredit(acc.ID, TLCtxHelper.ModuleSettleCentre.LastSettleday);
                    acc.CreditDeposit(ORM.MAccount.CashInOfTradingDay(acc.ID, QSEnumEquityType.CreditEquity, TLCtxHelper.ModuleSettleCentre.NextTradingday));
                    acc.CreditWithdraw(ORM.MAccount.CashOutOfTradingDay(acc.ID, QSEnumEquityType.CreditEquity, TLCtxHelper.ModuleSettleCentre.NextTradingday));


                    //载入清算中心
                    TLCtxHelper.ModuleClearCentre.CacheAccount(acc);
                }
            }
            catch (Exception ex)
            {
                Util.Debug(Util.GlobalPrefix + ex.ToString());
            }
        }
    }
}
