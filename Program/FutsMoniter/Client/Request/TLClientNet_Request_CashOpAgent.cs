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
        /// 查询代理对应的支付信息
        /// </summary>
        /// <param name="agentfk"></param>
        public void ReqQryAgentPaymentInfo(int agentfk)
        {
            this.ReqContribRequest("MgrExchServer", "QryAgentPaymentInfo", agentfk.ToString());
        }

        /// <summary>
        /// 查询所有代理的出入金操作
        /// </summary>
        public void ReqQryAgentCashopOperationTotal()
        {
            this.ReqContribRequest("MgrExchServer", "QryAgentCashOperationTotal", "");
        }

        /// <summary>
        /// 查询代理出入金记录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void ReqQryAgentCashTrans(int mgrfk, long start, long end)
        {
            this.ReqContribRequest("MgrExchServer", "QueryAgentCashTrans", mgrfk.ToString() + "," + start.ToString() + "," + end.ToString());
        }

        /// <summary>
        /// 请求出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqRequestCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "RequestCashOperation", playload);
        }

        /// <summary>
        /// 确认出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqConfirmCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "ConfirmCashOperation", playload);
        }

        /// <summary>
        /// 取消出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqCancelCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "CancelCashOperation", playload);
        }

        /// <summary>
        /// 拒绝出入金操作
        /// </summary>
        /// <param name="playload"></param>
        public void ReqRejectCashOperation(string playload)
        {
            this.ReqContribRequest("MgrExchServer", "RejectCashOperation", playload);
        }
    }
}
