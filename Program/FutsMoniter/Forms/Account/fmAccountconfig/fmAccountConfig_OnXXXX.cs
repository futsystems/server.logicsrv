using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class fmAccountConfig
    {
        /// <summary>
        /// 获得风控项删除回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        void OnRuleItemDel(RuleItem item)
        {
            if (item.RuleType == QSEnumRuleType.OrderRule)
            {
                InvokeGotOrderRuleItemDel(item, true);
            }
            else if (item.RuleType == QSEnumRuleType.AccountRule)
            {
                InvokeGotAccountRuleItemDel(item, true);
            }
        }

        void OnRuleItemUpdate(RuleItem item)
        {
            OnRuleItem(item, true);
        }
        /// <summary>
        /// 获得委托风控项
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        void OnRuleItem(RuleItem item, bool islast)
        {
            if (item.RuleType == QSEnumRuleType.OrderRule)
            {
                InvokeGotOrderRuleItem(item, islast);
            }
            else if (item.RuleType == QSEnumRuleType.AccountRule)
            {
                InvokeGotAccountRuleItem(item, islast);
            }
        }


        /// <summary>
        /// 当帐户有变化时 更新修改窗体
        /// </summary>
        /// <param name="account"></param>
        public void OnAccountChanged(IAccountLite account)
        {
            if (_account != null && _account.Account.Equals(account.Account))
            {
                SetAccount(account);
            }
        }

    }
}
