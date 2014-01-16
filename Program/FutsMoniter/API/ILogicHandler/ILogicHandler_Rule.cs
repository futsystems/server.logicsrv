using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.API
{
    public partial interface ILogicHandler
    {
        #region 风控规则类
        /// <summary>
        /// 风控规则种类回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        void OnMGRRuleClassResponse(RuleClassItem item, bool islast);

        /// <summary>
        /// 帐户风控规则项目回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        void OnMGRRuleItemResponse(RuleItem item, bool islast);

        /// <summary>
        /// 风控项更新回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        void OnMGRRuleItemUpdate(RuleItem item, bool islast);
        /// <summary>
        /// 删除风控项回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        void OnMGRRulteItemDelete(RuleItem item, bool islast);
        #endregion
    }
}
