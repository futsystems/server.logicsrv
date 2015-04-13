using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {

        public void ReqQryRuleSet()
        {
            debug("请求查询风控规则列表", QSEnumDebugLevel.INFO);
            MGRQryRuleSetRequest request = RequestTemplate<MGRQryRuleSetRequest>.CliSendRequest(requestid++);

            SendPacket(request);
        }

        public void ReqQryRuleItem(string account, QSEnumRuleType type)
        {
            debug("请求查询帐户风控规则列表");
            MGRQryRuleItemRequest request = RequestTemplate<MGRQryRuleItemRequest>.CliSendRequest(requestid++);
            request.Account = account;
            request.RuleType = type;

            SendPacket(request);
        }
        public void ReqUpdateRuleSet(RuleItem item)
        {
            debug("请求更新风控规则", QSEnumDebugLevel.INFO);
            MGRUpdateRuleRequest request = RequestTemplate<MGRUpdateRuleRequest>.CliSendRequest(requestid++);
            request.RuleItem = item;

            SendPacket(request);
        }

        public void ReqDelRuleItem(RuleItem item)
        {
            debug("请求删除风控规则", QSEnumDebugLevel.INFO);
            MGRDelRuleItemRequest request = RequestTemplate<MGRDelRuleItemRequest>.CliSendRequest(requestid++);
            request.RuleItem = item;

            SendPacket(request);
        }
    }
}
