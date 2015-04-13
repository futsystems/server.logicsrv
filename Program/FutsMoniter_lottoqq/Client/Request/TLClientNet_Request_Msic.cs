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
        /// 查询系统状态
        /// </summary>
        public void ReqQrySystemStatus()
        {
            this.ReqContribRequest("MgrExchServer", "QrySystemStatus","");
        }

        /// <summary>
        /// 查询银行列表
        /// </summary>
        public void ReqQryBank()
        {
            this.ReqContribRequest("MgrExchServer", "QryBank", "");
        }

        /// <summary>
        /// 查询收款银行
        /// </summary>
        public void ReqQryReceiveableBank()
        {
            this.ReqContribRequest("MgrExchServer", "QryReceiveableBank", "");
        }
    }
}
