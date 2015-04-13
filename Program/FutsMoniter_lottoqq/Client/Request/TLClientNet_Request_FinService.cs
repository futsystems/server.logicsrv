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
        /// 查询某个代理 某个服务计划的成本参数
        /// 如果没有设置则返回默认参数
        /// 
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="spfk"></param>
        public void ReqQrySPAgentArg(int agentfk, int spfk)
        {
            this.ReqContribRequest("FinServiceCentre", "QryAgentSPArg", agentfk.ToString() + "," + spfk.ToString());
        }

        /// <summary>
        /// 更新某个代理的某个服务计划的参数
        /// </summary>
        /// <param name="playload"></param>
        public void ReqUpdateSPAgentArg(string playload)
        {
            this.ReqContribRequest("FinServiceCentre", "UpdateAgentSPArg", playload);
        }
        /// <summary>
        /// 查询某个交易帐户的配资参数
        /// </summary>
        /// <param name="account"></param>
        public void ReqQryFinService(string account)
        {
            this.ReqContribRequest("FinServiceCentre", "QryFinService", account);
        }

        /// <summary>
        /// 更新配资服务参数
        /// </summary>
        /// <param name="playload"></param>
        public void ReqUpdateFinServiceArgument(string playload)
        {
            this.ReqContribRequest("FinServiceCentre", "UpdateArguments", playload);
        }

        /// <summary>
        /// 查询服务计划
        /// </summary>
        public void ReqQryServicePlan()
        {
            this.ReqContribRequest("FinServiceCentre", "QryFinServicePlan", "");
        }

        /// <summary>
        /// 修改某个帐户的配资服务
        /// </summary>
        /// <param name="playload"></param>
        public void ReqChangeFinService(string playload)
        {
            this.ReqContribRequest("FinServiceCentre", "ChangeServicePlane", playload);
        }

        /// <summary>
        /// 删除某个交易帐号的配资服务
        /// </summary>
        /// <param name="account"></param>
        public void ReqDeleteFinService(string account)
        {
            this.ReqContribRequest("FinServiceCentre", "DeleteServicePlane", account);
        }


      
    }
}
