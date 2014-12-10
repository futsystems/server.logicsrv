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
        /// 查询交易帐户信息
        /// </summary>
        /// <param name="account"></param>
        public void ReqQryAccountInfo2(string account)
        {
            this.ReqContribRequest("MgrExchServer", "QryAccountInfo", account);
        }
    }
}
