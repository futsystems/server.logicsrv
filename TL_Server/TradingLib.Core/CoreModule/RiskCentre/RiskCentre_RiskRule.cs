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
        Dictionary<string, RuleClassItem> dicRule = new Dictionary<string, RuleClassItem>();
        /// <summary>
        /// 加载风控规则从风控规则dll中加载对应的类然后用于每个交易账户设定规则进行实例化
        /// </summary>
        private void LoadRulePlugin()
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
        /// 加载某个账户的账户风控检查规则 并将其放入检查列表
        /// </summary>
        /// <param name="a"></param>
        void LoadRuleItem(IAccount account)
        {
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

            var cfgtemplate = BasicTracker.ConfigTemplateTracker[account.Config_ID];
            if (cfgtemplate != null)
            {
                IEnumerable<RuleItem> ruleitems = ORM.MRuleItem.SelectRuleItem(Const.CONFIG_TEMPLATE_PREFIX + cfgtemplate.ID.ToString());
                //没有加载任何委托风控 则从配置模板中加载委托风控
                if (!anyrule)
                {
                    foreach (var item in ruleitems)
                    {
                        LoadRuleItem(account, item);
                    }
                }
            }


            //加载完毕后 设定帐户的风控规则加载标识
            account.RuleItemLoaded = true;
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
                    account.AddOrderCheck(oc);
                    item.RuleDescription = oc.RuleDescription;
                }
                else if(item.RuleType == QSEnumRuleType.AccountRule)
                {
                    IAccountCheck ac = (IAccountCheck)klassitem.GenerateRuleInstance(item);
                    ac.Account = account;
                    account.AddAccountCheck(ac);
                    item.RuleDescription = ac.RuleDescription;
                    
                }
            }
        }

        



        /// <summary>
        /// 风控中心删除风控项
        /// </summary>
        /// <param name="item"></param>
        void DeleteRiskRule(RuleItem item)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[item.Account];
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
        /// 删除交易账户下所有风控规则
        /// </summary>
        /// <param name="account"></param>
        public void DelAccountRuleSet(IAccount account)
        {
            logger.Info(string.Format("Delete Account:{0}'s risk rule", account.ID));
            account.ClearAccountCheck();
            account.ClearOrderCheck();
            ORM.MRuleItem.DelRulteItem(account.ID);
        }

        /// <summary>
        /// 重新加载账户的风控策略,用于盘中入金，解冻交易账户
        /// 则需要重新加载风控规则,风控规则被处罚后，相关标志会处于状态位，解冻后需要恢复规则初始状态
        /// </summary>
        /// <param name="account"></param>
        public void ResetRuleSet(IAccount account)
        {
            logger.Info(string.Format("Account:{0} Reload RuleSet", account.ID));
            account.ClearAccountCheck();
            account.ClearOrderCheck();
            LoadRuleItem(account);

        }

        /// <summary>
        /// 加载所有交易账户的风控规则
        /// 只做一次数据库查询 放入内存后进行操作 避免遍历所有交易账户进行数据库查询
        /// </summary>
        void LoadRuleItemAll()
        {
            logger.Info("Load risk rule for account");
            IEnumerable<RuleItem> ruleitems = ORM.MRuleItem.SelectAllRuleItems();
            foreach (IAccount account in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                bool anyrule = false;
                if (account.RuleItemLoaded)
                {
                    account.ClearAccountCheck();
                    account.ClearOrderCheck();
                }
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
                //加载完毕后 设定帐户的风控规则加载标识
                account.RuleItemLoaded = true;
                
            }
        }

    }
}
