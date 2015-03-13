using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 交易帐户与主帐户绑定关系的管理
    /// </summary>
    public partial class TLClientNet
    {

        public void ReqQryAccountConnectorPair(string account)
        {
            debug("请求查询交易帐户:" + account + " 的主帐户绑定");
            this.ReqContribRequest("BrokerRouterPassThrough", "QryAccountConnectorPair",account);
     
        }

        public void ReqQryAvabileConnectors()
        {
            debug("请求查询未绑定主帐户");
            this.ReqContribRequest("BrokerRouterPassThrough", "QryAvabileConnectors","");

        }

        public void ReqUpdateAccountConnector(string account, int connector_id)
        {
            this.ReqContribRequest("BrokerRouterPassThrough", "UpdateAccountConnectorPair", account + "," + connector_id.ToString());
        }


        public void ReqDelAccountConnector(string account)
        {
            this.ReqContribRequest("BrokerRouterPassThrough", "DelAccountConnectorPair", account);
        }

        public void ReqSyncData(string account)
        {
            this.ReqContribRequest("BrokerRouterPassThrough", "SyncExData", account);
        }

        public void ReqQryConnectorAccountInfo(string account)
        {
            this.ReqContribRequest("BrokerRouterPassThrough", "QryBrokerAccountInfo", account);
        }

    }
}
