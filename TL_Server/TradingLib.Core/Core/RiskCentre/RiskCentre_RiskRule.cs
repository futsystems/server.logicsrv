﻿using System;
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

        /// <summary>
        /// 清空监控中的帐户列表
        /// </summary>
        void ClearActiveAccount()
        {
            activeaccount.Clear();
        }
        /// <summary>
        /// 插入某帐户 风控中心以实时监控
        /// </summary>
        /// <param name="account"></param>
        void InsertActiveAccount(IAccount account)
        {
            if (!activeaccount.Keys.Contains(account.ID))
            {
                activeaccount.TryAdd(account.ID, account);
            }
        }

        #region 加载 交易帐户委托规则与帐户规则
        Dictionary<string, RuleClassItem> dicRule = new Dictionary<string, RuleClassItem>();
        /// <summary>
        /// 加载风控规则从风控规则dll中加载对应的类然后用于每个交易账户设定规则进行实例化
        /// </summary>
        private void LoadRuleClass()
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
                    debug("OrderRuleSet:" + t.FullName + " load error:"+ex.ToString(), QSEnumDebugLevel.ERROR);
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
        /// 加载所有交易账户的风控规则
        /// 只做一次数据库查询 放入内存后进行操作 避免遍历所有交易账户进行数据库查询
        /// </summary>
        void LoadRuleItemAll()
        {
            debug("加载所有交易账户 风控规则", QSEnumDebugLevel.INFO);
            IEnumerable<RuleItem> ruleitems = ORM.MRuleItem.SelectAllRuleItems();
            foreach (IAccount account in _clearcentre.Accounts)
            {
                if (!account.RuleItemLoaded)
                {
                    foreach (RuleItem item in ruleitems.Where(r => account.ID == r.Account))
                    {
                        AddRule(account, item);
                    }
                    //加载完毕后 设定帐户的风控规则加载标识
                    account.RuleItemLoaded = true;
                    //将帐户插入激活的检查列表
                    InsertActiveAccount(account);
                }
            }
        }
        /// <summary>
        /// 加载某个账户的账户风控检查规则 并将其放入检查列表
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
            //加载完毕后 设定帐户的风控规则加载标识
            account.RuleItemLoaded = true;
            //将帐户插入激活的检查列表
            InsertActiveAccount(account);
        }


        /// <summary>
        /// 为某个交易帐户添加风控规则
        /// </summary>
        /// <param name="account"></param>
        /// <param name="item"></param>
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
        /// 验证风控规则 防止设定错误的风控规则比如参数是否是数字等
        /// </summary>
        /// <param name="item"></param>
        void ValidRuleItem(RuleItem item)
        {
            RuleClassItem klassitem = null;
            if (dicRule.TryGetValue(item.RuleName, out klassitem))
            {
                klassitem.GenerateRuleInstance(item);
            }
            else
            {
                throw new FutsRspError("风控规则集不存在");
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
                //id不为0 更新风控规则 删除后然后再添加
                if (item.ID != 0)
                {
                    //删除旧的委托规则
                    if (item.RuleType == QSEnumRuleType.OrderRule)
                    {
                        account.DelOrderCheck(item.ID);
                    }
                    else if (item.RuleType == QSEnumRuleType.AccountRule)
                    {
                        account.DelAccountCheck(item.ID);
                    }
                    //更新数据库
                    ORM.MRuleItem.UpdateRuleItem(item);
                    //添加新的委托规则
                    AddRule(account, item);

                }
                else //添加到数据库然后再加入到帐户规则中
                {
                    //插入数据库
                    ORM.MRuleItem.InsertRuleItem(item);//数据先添加 获得全局ID号
                    //3.添加新的rule到帐户
                    AddRule(account, item);
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
        public IEnumerable<RuleClassItem> GetRuleClassItems(QSEnumRuleType type)
        {
            return dicRule.Values.Where(r => (r.Type == type));
        }

        /// <summary>
        /// 获得所有风控规则
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RuleClassItem> GetRuleClassItems()
        {
            return dicRule.Values;
        }
    }
}