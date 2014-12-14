using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutsMoniter;

namespace TradingLib.Common
{
    public partial class BasicInfoTracker
    {

        public void GotRuleClass(RuleClassItem item)
        {
            if (item.Type == QSEnumRuleType.OrderRule)
            {
                if (!orderruleclassmap.Keys.Contains(item.ClassName))
                {
                    orderruleclassmap[item.ClassName] = item;
                }
            }
            else if (item.Type == QSEnumRuleType.AccountRule)
            {
                if (!accountruleclassmap.Keys.Contains(item.ClassName))
                {
                    accountruleclassmap[item.ClassName] = item;
                }
            }
        }


        /// <summary>
        /// 委托风控map
        /// </summary>
        Dictionary<string, RuleClassItem> orderruleclassmap = new Dictionary<string, RuleClassItem>();
        /// <summary>
        /// 帐户风控map
        /// </summary>
        Dictionary<string, RuleClassItem> accountruleclassmap = new Dictionary<string, RuleClassItem>();

        #region 风控规则
        /// <summary>
        /// 委托规则集
        /// </summary>
        public IEnumerable<RuleClassItem> OrderRuleClass
        {
            get
            {
                return orderruleclassmap.Values;
            }
        }

        /// <summary>
        /// 帐户规则集
        /// </summary>
        public IEnumerable<RuleClassItem> AccountRuleClass
        {
            get
            {
                return accountruleclassmap.Values;
            }
        }

        public RuleClassItem GetOrderRuleClass(string classname)
        {
            RuleClassItem item = null;
            if (orderruleclassmap.TryGetValue(classname, out item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }


        public RuleClassItem GetAccountRuleClass(string classname)
        {
            RuleClassItem item = null;
            if (accountruleclassmap.TryGetValue(classname, out item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }

        public RuleClassItem GetRuleItemClass(RuleItem item)
        {
            if (item.RuleType == QSEnumRuleType.OrderRule)
            {
                return GetOrderRuleClass(item.RuleName);
            }
            else if (item.RuleType == QSEnumRuleType.AccountRule)
            {
                return GetAccountRuleClass(item.RuleName);
            }
            return null;
        }

        #endregion

    }
}
