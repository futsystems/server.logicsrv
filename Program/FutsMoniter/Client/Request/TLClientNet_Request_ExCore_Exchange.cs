using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {

        /// <summary>
        /// 注销交易帐户的登入客户端
        /// </summary>
        public void ReqClearTerminals(string account)
        {
            debug("注销登入客户端", QSEnumDebugLevel.INFO);
            this.ReqContribRequest("MsgExch", "ClearAccountTerminals", account);
        }
    }
}
