//Copyright 2013 by FutSystems,Inc.
//20170807 将风控规则处理逻辑从Account对象集中放到风控模块内部

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

        #region 基础函数
        Dictionary<string, RuleClassItem> dicRule = new Dictionary<string, RuleClassItem>();
        /// <summary>
        /// 加载风控规则从风控规则dll中加载对应的类然后用于每个交易账户设定规则进行实例化
        /// </summary>
        void LoadRulePlugin()
        {
            dicRule.Clear();
            foreach (Type t in PluginHelper.LoadOrderRule())
            {
                try
                {
                    RuleClassItem item = RuleClassItem.Type2RuleClassItem(t);
                    dicRule.Add(item.ClassName, item);
                    logger.Debug("[RuleSet Loaded] " + item.ClassName);
                }
                catch (Exception ex)
                {
                    logger.Error("OrderRuleSet:" + t.FullName + " load error:" + ex.ToString());
                }
            }

            foreach (Type t in PluginHelper.LoadAccountRule())
            {
                try
                {
                    RuleClassItem item = RuleClassItem.Type2RuleClassItem(t);
                    dicRule.Add(item.ClassName, item);
                    logger.Debug("[RuleSet Loaded] " + item.ClassName);
                }
                catch (Exception ex)
                {
                    logger.Error("AccountRuleSet:" + t.FullName + " load error:"+ex.ToString());
                }
            }
        }

        /// <summary>
        /// 为某个交易帐户添加风控规则
        /// </summary>
        /// <param name="account"></param>
        /// <param name="item"></param>
        void LoadRuleItem(IAccount account, RuleItem item)
        {
            logger.Debug(string.Format("Load RuleSet Account:{0} Name:{1}", account.ID, item.RuleName));
            RuleClassItem klassitem = null;
            //从风控类型字典中找到对应的类型并进行实例化
            if (dicRule.TryGetValue(item.RuleName, out klassitem))
            {
                if (item.RuleType == QSEnumRuleType.OrderRule)
                {
                    IOrderCheck oc = (IOrderCheck)klassitem.GenerateRuleInstance(item);
                    oc.Account = account;
                    item.RuleDescription = oc.RuleDescription;
                    CacheRiskRule(oc);
                }
                else if (item.RuleType == QSEnumRuleType.AccountRule)
                {
                    IAccountCheck ac = (IAccountCheck)klassitem.GenerateRuleInstance(item);
                    ac.Account = account;
                    item.RuleDescription = ac.RuleDescription;
                    CacheRiskRule(ac);

                }
            }
            else
            {
                logger.Warn(string.Format("RiskRule:{0} plugin not loaded", item.RuleName));
            }
        }
        #endregion



        /// <summary>
        /// 加载某个账户的账户风控检查规则 并将其放入检查列表
        /// </summary>
        /// <param name="a"></param>
        public void LoadRiskRule(IAccount account)
        {
            logger.Info(string.Format("Load Account:{0}'s RiskRule", account.ID));

            ClearRiskRule(account.ID);
            bool anyrule = false;
            foreach (RuleItem item in ORM.MRuleItem.SelectRuleItem(account.ID, QSEnumRuleType.OrderRule))
            {
                anyrule = true;
                LoadRuleItem(account, item);
            }
            foreach (RuleItem item in ORM.MRuleItem.SelectRuleItem(account.ID, QSEnumRuleType.AccountRule))
            {
                anyrule = true;
                LoadRuleItem(account, item);
            }

            //没有加载任何委托风控 则尝试从配置模板中加载委托风控
            if (!anyrule)
            {
                var cfgtemplate = BasicTracker.ConfigTemplateTracker[account.Config_ID];
                if (cfgtemplate != null)
                {
                    IEnumerable<RuleItem> ruleitems = ORM.MRuleItem.SelectRuleItem(Const.CONFIG_TEMPLATE_PREFIX + cfgtemplate.ID.ToString());
                    foreach (var item in ruleitems)
                    {
                        LoadRuleItem(account, item);
                    }
                }
            }
        }

        /// <summary>
        /// 删除交易账户下所有风控规则
        /// </summary>
        /// <param name="account"></param>
        public void DeleteRiskRule(IAccount account)
        {
            logger.Info(string.Format("Delete Account:{0}'s RiskRule", account.ID));
            ClearRiskRule(account.ID);
            ORM.MRuleItem.DelRulteItem(account.ID);
        }


        /// <summary>
        /// 加载所有交易账户的风控规则
        /// 只做一次数据库查询 放入内存后进行操作 避免遍历所有交易账户进行数据库查询
        /// </summary>
        void BatchLoadRiskRule()
        {
            logger.Info("Batch Load RiskRule");
            IEnumerable<RuleItem> ruleitems = ORM.MRuleItem.SelectAllRuleItems();
            foreach (IAccount account in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                bool anyrule = false;
                ClearRiskRule(account.ID);
                foreach (RuleItem item in ruleitems.Where(r => account.ID == r.Account))
                {
                    anyrule = true;
                    LoadRuleItem(account, item);
                }
                //没有加载任何委托风控 则从配置模板中加载委托风控
                if (!anyrule)
                {
                    string cfgprefix = Const.CONFIG_TEMPLATE_PREFIX + account.Config_ID.ToString();
                    foreach (var item in ruleitems.Where(t => t.Account == cfgprefix))
                    {
                        LoadRuleItem(account, item);
                    }
                }
            }
        }


        #region 风控规则数据结构、风控检查
        
        static IEnumerable<IOrderCheck> EMPTY_ORDERCHECK = new List<IOrderCheck>();
        static IEnumerable<IAccountCheck> EMPTY_ACCOUNTCHECK = new List<IAccountCheck>();

        ConcurrentDictionary<string, ConcurrentDictionary<int, IOrderCheck>> orderCheckMap = new ConcurrentDictionary<string, ConcurrentDictionary<int, IOrderCheck>>();
        ConcurrentDictionary<string, ConcurrentDictionary<int, IAccountCheck>> accountCheckMap = new ConcurrentDictionary<string, ConcurrentDictionary<int, IAccountCheck>>();

        /// <summary>
        /// 将委托规则放到风控规则数据结构
        /// </summary>
        /// <param name="check"></param>
        void CacheRiskRule(IOrderCheck check)
        {
            ConcurrentDictionary<int, IOrderCheck> checkmap = null;
            if (!orderCheckMap.TryGetValue(check.Account.ID, out checkmap))
            {
                checkmap = new ConcurrentDictionary<int, IOrderCheck>();
                orderCheckMap.TryAdd(check.Account.ID, checkmap);
            }
            if (!checkmap.TryAdd(check.ID, check))
            {
                logger.Warn(string.Format("OrderCheck ID:{0} already added", check.ID));
            }
        }

        /// <summary>
        /// 将账户规则放到风控规则数据结构
        /// </summary>
        /// <param name="check"></param>
        void CacheRiskRule(IAccountCheck check)
        {
            ConcurrentDictionary<int, IAccountCheck> checkmap = null;
            if (!accountCheckMap.TryGetValue(check.Account.ID, out checkmap))
            {
                checkmap = new ConcurrentDictionary<int, IAccountCheck>();
                accountCheckMap.TryAdd(check.Account.ID, checkmap);
            }
            if (!checkmap.TryAdd(check.ID, check))
            {
                logger.Warn(string.Format("AccountCheck ID:{0} already added", check.ID));
            }
        }

        /// <summary>
        /// 从风控数据结构中清楚某个账户的风控规则
        /// </summary>
        /// <param name="account"></param>
        void ClearRiskRule(string account)
        {
            ConcurrentDictionary<int, IOrderCheck> checkmap1 = null;
            orderCheckMap.TryRemove(account, out checkmap1);

            ConcurrentDictionary<int, IAccountCheck> checkmap2 = null;
            accountCheckMap.TryRemove(account, out checkmap2);
        }

        /// <summary>
        /// 获得交易账户所有委托风控
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        IEnumerable<IOrderCheck> GetOrderCheck(string account)
        { 
             ConcurrentDictionary<int, IOrderCheck> checkmap = null;
             if (orderCheckMap.TryGetValue(account, out checkmap))
             {
                 return checkmap.Values;
             }
             return EMPTY_ORDERCHECK;
        }

        /// <summary>
        /// 获得交易账户所有账户风控
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        IEnumerable<IAccountCheck> GetAccountCheck(string account)
        {
            ConcurrentDictionary<int, IAccountCheck> checkmap = null;
            if (accountCheckMap.TryGetValue(account, out checkmap))
            {
                return checkmap.Values;
            }
            return EMPTY_ACCOUNTCHECK;
        }


        
        /// <summary>
        /// 委托检查
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool CheckOrderRule(Order o, out string msg)
        {
            msg = "";
            foreach (IOrderCheck rc in GetOrderCheck(o.Account))
            {
                if (!rc.checkOrder(o, out msg))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 账户检查
        /// </summary>
        /// <param name="account"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool CheckAccountRule(string account, out string msg)
        {
            msg = "";
            foreach (IAccountCheck rc in GetAccountCheck(account))
            {
                if (!rc.CheckAccount(out msg))
                    return false;
            }
            return true;
        }
        #endregion

    }
}
