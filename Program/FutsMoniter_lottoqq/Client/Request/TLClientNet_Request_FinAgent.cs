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
        /// 查询代理的财务信息
        /// </summary>
        public void ReqQryAgentFinanceInfo()
        {
            this.ReqContribRequest("MgrExchServer", "QryFinanceInfo", "");
        }

        /// <summary>
        /// 查询代理精简财务信息
        /// </summary>
        public void ReqQryAgentFinanceInfoLite()
        {
            this.ReqContribRequest("MgrExchServer", "QryFinanceInfoLite", "");
        }

        /// <summary>
        /// 更新代理主域的银行卡信息
        /// </summary>
        /// <param name="playload"></param>
        public void ReqUpdateAgentBankInfo(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateAgentBankAccount", playload);
        }
      
    }
}
