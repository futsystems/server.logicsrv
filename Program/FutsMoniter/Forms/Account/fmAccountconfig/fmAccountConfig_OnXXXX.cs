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
        public void GotRuleItemDel(RuleItem item, bool islast)
        {
            if (item.RuleType == QSEnumRuleType.OrderRule)
            {
                InvokeGotOrderRuleItemDel(item, islast);
            }
            else if (item.RuleType == QSEnumRuleType.AccountRule)
            {
                InvokeGotAccountRuleItemDel(item, islast);

            }
        }
        /// <summary>
        /// 获得委托风控项
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        public void GotRuleItem(RuleItem item, bool islast)
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
        /// 获得帐户财务信息
        /// </summary>
        /// <param name="info"></param>
        public void GotAccountInfo(IAccountInfo info)
        {
            if (Account.Account.Equals(info.Account))
            {
                ctFinanceInfo1.GotAccountInfo(info);

            }
        }


        /// <summary>
        /// 当帐户有变化时 更新修改窗体
        /// </summary>
        /// <param name="account"></param>
        public void GotAccountChanged(IAccountLite account)
        {
            if (this.Account != null && Account.Account.Equals(account.Account))
            {
                this.Account = account;
            }
        }

    }
}
