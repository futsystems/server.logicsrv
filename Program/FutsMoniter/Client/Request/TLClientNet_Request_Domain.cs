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
        /// 更新域
        /// </summary>
        /// <param name="domain"></param>
        public void ReqUpdateDomain(DomainImpl domain)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateDomain", TradingLib.Mixins.LitJson.JsonMapper.ToJson(domain));
        }
    }
}
