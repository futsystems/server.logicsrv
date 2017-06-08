using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 管理员
    /// </summary>
    public partial class MgrExchServer
    {
        /// <summary>
        /// 设定观察交易账户列表
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "WatchAgents", "WatchAgents - watch  agent", "设置当前观察代理", QSEnumArgParseType.Json)]
        public void CTE_WatchAgents(ISession session, string json)
        {
            string[] accounts = json.DeserializeObject<string[]>();
            var c = customerExInfoMap[session.Location.ClientID];
            c.WatchAgents(accounts);
        }



    }
}
