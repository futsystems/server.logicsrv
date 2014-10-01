using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace SocialLink
{
    public partial class SocialGate
    {

        [ContribCommandAttr("DemoRecharge","QryUserFollowerNumEvent","QryUserFollowersNum","绑定到DemoRecharge的查询粉丝数量事件","用于接收DemoRecharge模块的查询粉丝数量查询事件")]
        public int QryUserFollowersNum(string email)
        {
            TLCtxHelper.Debug("查询用户:" + email + " 粉丝数量");
            SocialDB db = conn.mysqlDB;
            int num = db.GetFollowers(email);
            conn.Return(db);
            return num;
        }
    }
}
