using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public partial class TLClientNet
    {
        public void ReqQryDomain()
        {
            this.ReqContribRequest("MgrExchServer", "QryDomain", "");
        }

        public void ReqUpdateDomain(DomainImpl domain)
        {
            this.ReqContribRequest("MgrExchServer", "UpdateDomain", TradingLib.Mixins.LitJson.JsonMapper.ToJson(domain));
        }
    }
}
