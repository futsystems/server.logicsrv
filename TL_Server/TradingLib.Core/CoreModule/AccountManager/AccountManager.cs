﻿using System;
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

        protected ConcurrentDictionary<string, IAccount> _accMap = new ConcurrentDictionary<string, IAccount>();

        ConfigDB _cfgdb;

        bool _deleteAccountCheckEquity = false;

        IdTracker _txnTracker = new IdTracker(IdTracker.OWNER_TXN_ACC);
        public AccountManager():
            base(AccountManager.CoreName)
        {
            
            //1.加载配置文件
            _cfgdb = new ConfigDB(AccountManager.CoreName);

            if (!_cfgdb.HaveConfig("DeleteAccountCheckEquity"))
            {
                _cfgdb.UpdateConfig("DeleteAccountCheckEquity", QSEnumCfgType.Bool, false, "删除交易帐户是否检查帐户权益");
            }
            _deleteAccountCheckEquity = _cfgdb["DeleteAccountCheckEquity"].AsBool();

            
            
            LoadAccount();
            logger.Info(string.Format("Load Account form database,total num:{0}", _accMap.Count));
            
        }
        public void Start()
        {
            logger.StatusStart(this.PROGRAME);
        }

        public void Stop()
        {
            logger.StatusStop(this.PROGRAME);
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
                _accMap.TryGetValue(id, out ac);
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
                return _accMap.Values;
            }
        }


        /// <summary>
        /// 账户属性变动事件
        /// </summary>
        /// <param name="account"></param>
        protected void AccountChanged(IAccount account)
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
            //TLCtxHelper.EventAccount.FireAccountAddEvent(this[create.Account]);

            logger.Info(string.Format("Account:{0} UserID:{1} Added Under Manager:{2}", create.Account, create.UserID, create.BaseManagerID));
        }

        /// <summary>
        /// 由于引入代理结算后 删除账户无法立即生效，需要等待账户结算完毕之后才可以正常删除账户
        /// </summary>
        /// <param name="id"></param>
        public void DelAccount(string id)
        {
            if (!GlobalConfig.DeleteDirect)//非直接删除
            {
                IAccount account = this[id];
                if (account == null)
                {
                    logger.Warn(string.Format("Account:{0} do not exist", id));
                    return;
                }

                account.Deleted = true;
                //数据库更新标识账户逻辑删除
                ORM.MAccount.MarkAccountDeleted(id);

                TLCtxHelper.EventAccount.FireAccountDelEvent(account);
                logger.Info(string.Format("Account:{0} Deleted", id));
            }
            else//直接删除
            {
                IAccount account = this[id];
                if (account == null)
                {
                    throw new FutsRspError("交易帐号不存在");
                }
                try
                {

                    _accMap.TryRemove(id, out account);
                    //删除数据库
                    ORM.MAccount.DelAccount(id);//删除数据库记录
                    //删除内存记录
                    TLCtxHelper.ModuleClearCentre.DropAccount(account);

                    BasicTracker.AccountProfileTracker.DropAccount(id);
                    //对外触发交易帐户删除事件
                    account.Deleted = true;

                    TLCtxHelper.EventAccount.FireAccountDelEvent(account);

                    logger.Info(string.Format("Account:{0} Deleted", id));
                }
                catch (Exception ex)
                {
                    logger.Error("删除交易帐户错误:" + ex.ToString());
                    throw new FutsRspError("删除交易帐户异常，请手工删除相关信息");
                }
            }
        }

        /// <summary>
        /// 判定某个用户ID是否已经绑定了交易账户
        /// </summary>
        /// <param name="userId"></param>
        public bool UserHaveAccount(int userID)
        {
            if (userID <= 0)
            {
                throw new Exception("UserID should greater than 0");
            }
            return _accMap.Values.Where(acc=>!acc.Deleted).Any(acc => acc.UserID == userID);
        }

        public IAccount GetUserAccount(int userID)
        {
            return _accMap.Values.Where(acc => acc.UserID == userID).FirstOrDefault();
        }
        /// <summary>
        /// 从数据库加载某accID的交易账户
        /// </summary>
        /// <param name="accID"></param>
        private void LoadAccount(string account = null)
        {
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
                    //账户删除 且删除时结算日已结算 则将账户从数据库中删除
                    if (acc.Deleted && TLCtxHelper.ModuleSettleCentre.Tradingday > acc.DeletedSettleday)
                    {
                        //物理删除 不处理删除账户数据
                        if (GlobalConfig.LogicDelete)
                        {
                            
                        }
                        else //逻辑删除
                        {
                            //删除数据库
                            ORM.MAccount.DelAccount(acc.ID);//删除数据库记录
                        }
                        continue;
                    }
                    //交易账户无对应管理员 不加载到内存
                    if (BasicTracker.ManagerTracker[acc.Mgr_fk] == null)
                    {
                        continue;
                    }
                    //1.将account添加在数据结构中
                    _accMap.TryAdd(acc.ID, acc);
                    //2.调用清算中心加载帐户
                    TLCtxHelper.ModuleClearCentre.CacheAccount(acc);
                }
            }
            catch (Exception ex)
            {
                logger.Error("LoadAccount Error:" + ex.ToString());
            }
        }
    }
}
