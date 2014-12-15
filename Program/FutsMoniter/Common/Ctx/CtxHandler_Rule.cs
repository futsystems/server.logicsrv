using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using FutsMoniter;

namespace TradingLib.Common
{
    /// <summary>
    /// 消息处理中继,实现ILogicHandler,用于处理底层回报上来的消息
    /// 界面层订阅这里的事件 实现数据展示
    /// </summary>
    public partial class Ctx
    {

        public event Action<RuleItem, bool> GotRuleItemEvent;

        public event Action<RuleItem> GotRuleItemUpdateEvemt;

        public event Action<RuleItem> GotRuleItemDeleteEvent;

        /// <summary>
        /// 帐户风控规则项目回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        public void OnMGRRuleItemResponse(RuleItem item, bool islast)
        {
            if (GotRuleItemEvent != null)
                GotRuleItemEvent(item, islast);
        }


        /// <summary>
        /// 风控项更新回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        public void OnMGRRuleItemUpdate(RuleItem item, bool islast)
        {
            if (GotRuleItemUpdateEvemt != null)
                GotRuleItemUpdateEvemt(item);
        }

        /// <summary>
        /// 删除风控项回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        public void OnMGRRulteItemDelete(RuleItem item, bool islast)
        {
            if (GotRuleItemDeleteEvent != null)
                GotRuleItemDeleteEvent(item);
        }

    }
}
