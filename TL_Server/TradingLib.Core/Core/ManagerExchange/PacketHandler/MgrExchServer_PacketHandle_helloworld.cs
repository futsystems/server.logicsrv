using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        /// <summary>
        /// 管理端演示命令
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "HelloWorld", "HelloWorld - HelloWorld", "管理端演示命令")]
        public void CTE_HelloWorld(ISession session,string reqeust)
        {
            session.ReplyMgr(reqeust);
        }
    }
}
