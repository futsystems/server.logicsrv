using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class RiskCentre
    {

        ConcurrentDictionary<string, IAccount> activeaccount = new ConcurrentDictionary<string, IAccount>();

        void ClearActiveAccount()
        {
            activeaccount.Clear();
        }

        void InsertActiveAccount(IAccount account)
        {
            if (!activeaccount.Keys.Contains(account.ID))
            {
                activeaccount.TryAdd(account.ID, account);
            }
        }

        #region 加载 交易帐户委托规则与帐户规则

        //风控规则集类别 名称与 Type的对应,用于从XML配置文件中生成分控检测插件
        Dictionary<string, RuleClassItem> dicRule = new Dictionary<string, RuleClassItem>();
        /// <summary>
        /// 加载风控规则从风控规则dll中加载对应的类然后用于每个交易账户设定规则进行实例化
        /// </summary>
        private void LoadRuleSet()
        {
            dicRule.Clear();//清空当前映射列表
            foreach (Type t in PluginHelper.LoadOrderRule())
            {
                try
                {
                    RuleClassItem item = RuleClassItem.Type2RuleClassItem(t);
                    dicRule.Add(item.ClassName, item);
                    debug("[RuleSet Loaded] " + item.ClassName, QSEnumDebugLevel.INFO);
                }
                catch (Exception ex)
                {
                    debug("OrderRuleSet:" + t.FullName + " load error", QSEnumDebugLevel.ERROR);
                }
            }
            foreach (Type t in PluginHelper.LoadAccountRule())
            {
                try
                {
                    RuleClassItem item = RuleClassItem.Type2RuleClassItem(t);
                    dicRule.Add(item.ClassName, item);
                    debug("[RuleSet Loaded] " + item.ClassName, QSEnumDebugLevel.INFO);
                }
                catch (Exception ex)
                {
                    debug("AccountRuleSet:" + t.FullName + " load error", QSEnumDebugLevel.ERROR);
                }
            }
        }

        /// <summary>
        /// 加载某个账户的账户检查规则
        /// </summary>
        /// <param name="a"></param>
        public void LoadRuleItem(IAccount account)
        {
            debug("加载账户:" + account.ID + " 账户规则", QSEnumDebugLevel.DEBUG);
            foreach (RuleItem item in ORM.MRuleItem.SelectRuleItem(account.ID, QSEnumRuleType.OrderRule))
            {
                AddRule(account, item);
            }
            foreach (RuleItem item in ORM.MRuleItem.SelectRuleItem(account.ID, QSEnumRuleType.AccountRule))
            {
                AddRule(account, item);
            }
            account.RuleItemLoaded = true;
            InsertActiveAccount(account);
        }



        void AddRule(IAccount account,RuleItem item)
        {
            debug("添加风控项，帐户:" + account.ID + " rultename:" + item.RuleName, QSEnumDebugLevel.INFO);
            RuleClassItem klassitem = null;
            //从风控类型字典中找到对应的类型并进行实例化
            if (dicRule.TryGetValue(item.RuleName, out klassitem))
            {
                if (item.RuleType == QSEnumRuleType.OrderRule)
                {
                    //创建风控实例
                    IOrderCheck oc = (IOrderCheck)klassitem.GenerateRuleInstance(item);
                    //绑定帐户
                    oc.Account = new AccountAdapterToExp(account);
                    //将ordercheck 加载到account
                    account.AddOrderCheck(oc);
                    //将风控实例的规则描述传递给RuleItem ????????改进
                    item.RuleDescription = oc.RuleDescription;
                }
                else if(item.RuleType == QSEnumRuleType.AccountRule)
                {
                    IAccountCheck ac = (IAccountCheck)klassitem.GenerateRuleInstance(item);
                    ac.Account = new AccountAdapterToExp(account);
                    account.AddAccountCheck(ac);
                    item.RuleDescription = ac.RuleDescription;
                    
                }
            }
        }
        /// <summary>
        /// 更新风控规则
        /// 如果没有则新增
        /// </summary>
        /// <param name="item"></param>
        public void UpdateRule(RuleItem item)
        {
            IAccount account = _clearcentre[item.Account];
            //判断帐户是否存在
            if (account != null)
            {
                //id不为0 标识新增
                if (item.ID != 0)
                {
                    //删除旧的委托规则
                    if (item.RuleType == QSEnumRuleType.OrderRule)
                    {
                        account.DelOrderCheck(item.ID);
                    }
                    else if(item.RuleType == QSEnumRuleType.AccountRule)
                    {
                        account.DelAccountCheck(item.ID);
                    }
                    //更新数据库
                    ORM.MRuleItem.UpdateRuleItem(item);
                    //添加新的委托规则
                    AddRule(account, item);
                    
                }
                //id不为0则新增
                else
                {
                    //插入数据库
                    ORM.MRuleItem.InsertRuleItem(item);//数据先添加 获得全局ID号
                    //3.添加新的rule到帐户
                    AddRule(account,item);
                }
            }
        }

        /// <summary>
        /// 风控中心删除风控项
        /// </summary>
        /// <param name="item"></param>
        public void DeleteRiskRule(RuleItem item)
        {
            IAccount account = _clearcentre[item.Account];
            if (account != null)
            {
                if (item.RuleType == QSEnumRuleType.OrderRule)
                {
                    account.DelOrderCheck(item.ID);
                    ORM.MRuleItem.DelRulteItem(item);
                }
                else if (item.RuleType == QSEnumRuleType.AccountRule)
                {
                    account.DelAccountCheck(item.ID);
                    ORM.MRuleItem.DelRulteItem(item);
                }

            }
        }

        /// <summary>
        /// 重新加载账户的风控策略,用于盘中入金，解冻交易账户
        /// 则需要重新加载风控规则,风控规则被处罚后，相关标志会处于状态位，解冻后需要恢复规则初始状态
        /// </summary>
        /// <param name="account"></param>
        public void ResetRuleSet(string account)
        {
            IAccount acc = _clearcentre[account];
            if (acc != null)
            {
                acc.ClearAccountCheck();
                acc.ClearOrderCheck();
                LoadRuleItem(acc);
            }
        }
        #endregion


        /// <summary>
        /// 获得RuleClass列表
        /// 用于管理获取当前可用风控规则列表
        /// </summary>
        /// <returns></returns>
        public RuleClassItem[] GetRuleClassItems(QSEnumRuleType type)
        {
            return dicRule.Values.Where(r => (r.Type == type)).ToArray();
        }

        /// <summary>
        /// 获得所有风控规则
        /// </summary>
        /// <returns></returns>
        public RuleClassItem[] GetRuleClassItems()
        {
            return dicRule.Values.ToArray();
        }



        //#region 帐户进入不同阶段的比赛 设定对应的风控规则
        ///// <summary>
        ///// 初始化账户风控规则,当账户晋级,淘汰,或者报名均需要初始化风控规则,自动为帐户配置相应的风控规则
        ///// 相应事件  某个账户加入到了某个比赛的事件
        ///// </summary>
        ///// <param name="acc"></param>
        //public void OnAccountEntryRace(IAccount acc, QSEnumRaceType type)
        //{
        //    debug(PROGRAME + "账户:" + acc.ID + " 加入类型为:" + LibUtil.GetEnumDescription(type) + " 比赛,风控中心调整账户风控策略", QSEnumDebugLevel.INFO);
        //    switch (type)
        //    {
        //        case QSEnumRaceType.PRERACE:
        //            PRERACERiskSet(acc);
        //            break;
        //        case QSEnumRaceType.SEMIRACE:
        //            SEMIRACERiskSet(acc);
        //            break;
        //        case QSEnumRaceType.REAL1:
        //            REAL1RiskSet(acc);
        //            break;
        //        case QSEnumRaceType.REAL2:
        //            REAL2RiskSet(acc);
        //            break;
        //        case QSEnumRaceType.REAL3:
        //            REAL3RiskSet(acc);
        //            break;
        //        case QSEnumRaceType.REAL4:
        //            REAL4RiskSet(acc);
        //            break;
        //        case QSEnumRaceType.REAL5:
        //            REAL5RiskSet(acc);
        //            break;
        //        default:
        //            break;
        //    }

        //}
        ///// <summary>
        ///// 初赛初始化规则
        ///// 初赛不设定任何风控规则
        ///// </summary>
        ///// <param name="acc"></param>
        //void PRERACERiskSet(IAccount acc)
        //{
        //    debug(PROGRAME + ":账户加入初赛,绑定风控规则");
        //    try
        //    {
        //        //lock (orderrulefile_lk)
        //        {
        //            AccountCheckTracker.delRuleFromAccount(acc.ID);//清空xml文件中的账户检查
        //            acc.ClearAccountCheck();//清除账户检查
        //            OrderCheckTracker.delRuleFromAccount(acc.ID);
        //            acc.ClearOrderCheck();

        //            //设定默认手续费规则
        //            string rsname = "OrderRuleSet.RSCommission";
        //            string cfgtext = "OrderRuleSet.RSCommission,True,LessEqual,1500,";
        //            if (dicRule.ContainsKey(rsname))
        //            {
        //                //IOrderCheck rc = ((IOrderCheck)Activator.CreateInstance(dicRule[rsname])).FromText(cfgtext) as IOrderCheck;
        //                //OrderCheckTracker.addRuleIntoAccount(acc.ID, rc);
        //                //AccountCheckTracker.addRuleIntoAccount(acc.ID, rc);//将规则写入到xml配置文件
        //                //acc.AddAccountCheck(rc);//将规则加入到账户accountcheck列表
        //            }

        //            LoadOrderRule(acc);
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        debug(PROGRAME + "报名初赛赛账户:" + acc.ID + " 风控规则出错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
        //    }

        //}
        ///// <summary>
        ///// 复赛初始化规则
        ///// </summary>
        ///// <param name="acc"></param>
        //void SEMIRACERiskSet(IAccount acc)
        //{
        //    debug(PROGRAME + ":账户加入复赛,绑定风控规则");
        //    try
        //    {
        //        //lock (accountrulefile_lk)
        //        {
        //            AccountCheckTracker.delRuleFromAccount(acc.ID);//清空xml文件中的账户检查
        //            acc.ClearAccountCheck();//清除账户检查
        //            OrderCheckTracker.delRuleFromAccount(acc.ID);
        //            acc.ClearOrderCheck();

        //            string rsname = "AccountRuleSet.RSMaxLossPercent2Step";
        //            string cfgtext = "AccountRuleSet.RSMaxLossPercent2Step,True,Greater,2,";
        //            if (dicRule.ContainsKey(rsname))
        //            {
        //                //IAccountCheck rc = ((IAccountCheck)Activator.CreateInstance(dicRule[rsname])).FromText(cfgtext) as IAccountCheck;
        //                //AccountCheckTracker.addRuleIntoAccount(acc.ID, rc);//将规则写入到xml配置文件
        //                //acc.AddAccountCheck(rc);//将规则加入到账户accountcheck列表
        //            }

        //            //加载规则
        //            LoadAccountRule(acc);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        debug(PROGRAME + "初始化复赛账户:" + acc.ID + " 风控规则出错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
        //    }
        //}

        //void REAL1RiskSet(IAccount acc)
        //{
        //    debug(PROGRAME + ":账户加入实盘1,绑定风控规则");
        //    try
        //    {
        //        //lock (accountrulefile_lk)
        //        {
        //            AccountCheckTracker.delRuleFromAccount(acc.ID);//清空xml文件中的账户检查
        //            acc.ClearAccountCheck();//清除账户检查
        //            OrderCheckTracker.delRuleFromAccount(acc.ID);
        //            acc.ClearOrderCheck();

        //            string rsname = "AccountRuleSet.RSMaxLossPercent2Step";
        //            string cfgtext = "AccountRuleSet.RSMaxLossPercent2Step,True,Greater,2,";
        //            if (dicRule.ContainsKey(rsname))
        //            {
        //                //IAccountCheck rc = ((IAccountCheck)Activator.CreateInstance(dicRule[rsname])).FromText(cfgtext) as IAccountCheck;
        //                //AccountCheckTracker.addRuleIntoAccount(acc.ID, rc);//将规则写入到xml配置文件
        //                //acc.AddAccountCheck(rc);//将规则加入到账户accountcheck列表
        //            }

        //            //加载规则
        //            LoadAccountRule(acc);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        debug(PROGRAME + "初始化复赛账户:" + acc.ID + " 风控规则出错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
        //    }

        //}
        //void REAL2RiskSet(IAccount acc)
        //{

        //}
        //void REAL3RiskSet(IAccount acc)
        //{

        //}
        //void REAL4RiskSet(IAccount acc)
        //{

        //}
        //void REAL5RiskSet(IAccount acc)
        //{

        //}


        //#endregion


        //#region 为配资帐户绑定风控规则
        ///// <summary>
        ///// 配资账户生成时,我们给该账户绑定对应的规则
        ///// </summary>
        ///// <param name="account"></param>
        //public void SetLoaneeRule(string account)
        //{
        //    try
        //    {
        //        //lock (accountrulefile_lk)
        //        {
        //            IAccount acc = _clearcentre[account];
        //            if (acc == null) return;
        //            //清空账户的所有风控规则
        //            AccountCheckTracker.delRuleFromAccount(account);//清空xml文件中的账户检查
        //            acc.ClearAccountCheck();
        //            //清空委托检查规则
        //            OrderCheckTracker.delRuleFromAccount(account);
        //            acc.ClearOrderCheck();
        //            string rsname;
        //            string cfgtext;

        //            //风控规则的加载,我们通过开放风控规则加载接口 统一移植到对应的扩展模块中去
        //            //如果默认激活固定金额配资,我们加载不同类型的风控规则
        //            //if (CoreGlobal.EnableFixedMargin)
        //            //{
        //            //    rsname = "AccountRuleSet.RSFineServiceFixMargin";
        //            //    cfgtext = "AccountRuleSet.RSFineServiceFixMargin,True,Greater," + CoreGlobal.MarginPerLotStop.ToString() + ",";
        //            //}
        //            //else
        //            {
        //                rsname = "AccountRuleSet.RSFineService";
        //                cfgtext = "AccountRuleSet.RSFineService,True,Greater,2,";
        //            }

        //            if (dicRule.ContainsKey(rsname))
        //            {
        //                //生成对应的IAccountCheck
        //                //IAccountCheck rc = ((IAccountCheck)Activator.CreateInstance(dicRule[rsname])).FromText(cfgtext) as IAccountCheck;
        //                //AccountCheckTracker.addRuleIntoAccount(account, rc);//将规则写入到xml配置文件
        //            }

        //            //加载账户检查规则
        //            LoadAccountRule(acc);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        debug("设定配资账户风控规则出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
        //    }
        //}
        //#endregion
    }
}
