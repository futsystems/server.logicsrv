using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {
        void CliOnMGRRuleClass(RspMGRQryRuleSetResponse response)
        {
            debug("got ruleset response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRRuleClassResponse(response.RuleClassItem, response.IsLast);
        }

        void CliOnMGRRuleItem(RspMGRQryRuleItemResponse response)
        {
            debug("got ruleitem response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRRuleItemResponse(response.RuleItem, response.IsLast);
        }
        void CliOnMGRUpdateRuleItem(RspMGRUpdateRuleResponse response)
        {
            debug("got ruleitem update response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRRuleItemUpdate(response.RuleItem, response.IsLast);
        }
        void CliOnMGRDelRule(RspMGRDelRuleItemResponse response)
        {
            debug("got ruleitem delete response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRRulteItemDelete(response.RuleItem, response.IsLast);
        }
    }
}
