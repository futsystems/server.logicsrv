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

        /// <summary>
        /// 将某个交易帐户放入实时监控列表
        /// </summary>
        /// <param name="account"></param>
        public void AttachAccountCheck(string account)
        {
            //logger.Info(string.Format("清算中心将帐户:{0} 放入实时监控列表", account));
            if (!activeaccount.Keys.Contains(account))
            { 
                IAccount target = TLCtxHelper.ModuleAccountManager[account];
                if(target == null)
                {
                    logger.Warn(string.Format("Account:{0} do not exist",account));
                    return;
                }
                activeaccount.TryAdd(account, target);
            }
        }

        /// <summary>
        /// 将某个交易帐户从实时监控列表脱离
        /// </summary>
        /// <param name="account"></param>
        public void DetachAccountCheck(string account)
        {
            //logger.Info(string.Format("清算中心取消帐户:{0} 实时监控", account));
            IAccount target = null;
            if (activeaccount.Keys.Contains(account))
            {
                activeaccount.TryRemove(account, out target);
            }
        }

        /// <summary>
        /// 清空监控中的帐户列表
        /// </summary>
        void ClearActiveAccount()
        {
            activeaccount.Clear();
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
        public void LoadRuleItem(IAccount account)
        {
            logger.Info("加载账户:" + account.ID + " 账户规则");
            bool any = false;
            foreach (RuleItem item in ORM.MRuleItem.SelectRuleItem(account.ID, QSEnumRuleType.OrderRule))
            {
                AddRule(account, item);
            }
            foreach (RuleItem item in ORM.MRuleItem.SelectRuleItem(account.ID, QSEnumRuleType.AccountRule))
            {
                any = true;
                AddRule(account, item);
            }
            //加载完毕后 设定帐户的风控规则加载标识
            account.RuleItemLoaded = true;
            
            //将交易帐户加入风控监控列表 (取消对是否有风控规则的检查，如果帐户没有添加风控规则 冻结账户后，应该将该帐户清仓)
            AttachAccountCheck(account.ID);
            
        }


        /// <summary>
        /// 为某个交易帐户添加风控规则
        /// </summary>
        /// <param name="account"></param>
        /// <param name="item"></param>
        void AddRule(IAccount account,RuleItem item)
        {
            logger.Info("添加风控项，帐户:" + account.ID + " rultename:" + item.RuleName);
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
        /// 更新风控规则 都要将帐户添加到风控检查列表 否则新增风控规则后会出现规则不生效的问题
        /// </summary>
        /// <param name="item"></param>
        public void UpdateRule(RuleItem item)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[item.Account];
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

                    //添加帐户到风控监控列表
                    this.AttachAccountCheck(account.ID);


                }
                else //添加到数据库然后再加入到帐户规则中
                {
                    //插入数据库
                    ORM.MRuleItem.InsertRuleItem(item);//数据先添加 获得全局ID号
                    //3.添加新的rule到帐户
                    AddRule(account, item);

                    //添加帐户到风控监控列表
                    this.AttachAccountCheck(account.ID);
                }
            }
        }

        /// <summary>
        /// 风控中心删除风控项
        /// </summary>
        /// <param name="item"></param>
        public void DeleteRiskRule(RuleItem item)
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
        /// 重新加载账户的风控策略,用于盘中入金，解冻交易账户
        /// 则需要重新加载风控规则,风控规则被处罚后，相关标志会处于状态位，解冻后需要恢复规则初始状态
        /// </summary>
        /// <param name="account"></param>
        public void ResetRuleSet(IAccount account)
        {
            account.ClearAccountCheck();
            account.ClearOrderCheck();
            LoadRuleItem(account);
            
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
                if (!account.RuleItemLoaded)
                {
                    foreach (RuleItem item in ruleitems.Where(r => account.ID == r.Account))
                    {
                        AddRule(account, item);
                    }
                    //加载完毕后 设定帐户的风控规则加载标识
                    account.RuleItemLoaded = true;
                    //将帐户插入激活的检查列表
                    AttachAccountCheck(account.ID);
                }
            }
        }

    }
}
