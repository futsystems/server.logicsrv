using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.Common
{
    public class AccountConnectorPair
    {
        public AccountConnectorPair()
        {
            this.Account = string.Empty;
            this.Connector_ID = 0;
        }

        public AccountConnectorPair(string account, int connectorid)
        {
            this.Account = account;
            this.Connector_ID = connectorid;
        }
        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 实盘通道ID
        /// </summary>
        public int Connector_ID { get; set; }

    }
}
