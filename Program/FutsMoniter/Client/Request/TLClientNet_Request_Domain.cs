using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {
        /// <summary>
        /// 查询域
        /// </summary>
        public void ReqQryDomain()
        {
            this.ReqContribRequest("MgrExchServer", "QryDomain", "");
        }

        /// <summary>
        /// 查询分区管理员登入信息
        /// </summary>
        /// <param name="domainid"></param>
        public void ReqQryDomainRootLoginInfo(int domainid)
        {
            this.ReqContribRequest("MgrExchServer", "QryDomainRootLoginInfo", domainid.ToString());
        }
        /// <summary>
        /// 更新域
        /// </summary>
        /// <param name="domain"></param>
        public void ReqUpdateDomain(DomainImpl domain)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateDomain", TradingLib.Mixins.Json.JsonMapper.ToJson(domain));
        }

        /// <summary>
        /// 更新同步实盘帐户
        /// </summary>
        /// <param name="vid"></param>
        public void ReqUpdateSyncVendor(int vid)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateDomainCFGSyncSymbolVendor",vid.ToString());
        }
    }
}
