using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace DemeRecharge
{
    public partial class DemoRecharge
    {
        [ContribEventAttr(
            "DemoRecharge",
            "QryUserFollowerNumEvent",
            "qry user's followers number",
            "用于调用SocialGate相关方法获得用户的粉丝数量")]
        public event QryUserFollowersNum QryUserFollowersNumEvent;


        [ContribCommandAttr(QSEnumCommandSource.MessageExchange, "recharge", "recharge - will recharege account Equity", "当交易帐户对应的社交帐户满足一定条件时,我们可以进行帐户充值")]
        public void recharge(ISession session,string email)
        {

            TLCtxHelper.Debug("客户端请求资金重置:" + session.ClientID);
            int num=0;
            if (QryUserFollowersNumEvent != null)
                num = QryUserFollowersNumEvent(email);

            TLCtxHelper.Debug("User:" + email + "有粉丝:" + num.ToString() + "个");

        }

    }
}
