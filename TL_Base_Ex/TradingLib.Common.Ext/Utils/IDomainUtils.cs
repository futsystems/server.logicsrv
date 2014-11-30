using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class IDomainUtils
    {
         /// <summary>
        /// 获得域内
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static IEnumerable<IAccount> GetAccounts(this Domain domain)
        {
            return TLCtxHelper.CmdAccount.Accounts.Where(acc => acc.Domain.Equals(domain));
        }

        public static IEnumerable<ConnectorConfig> GetConnectorConfig(this Domain domain)
        {
            return BasicTracker.ConnectorConfigTracker.ConnecotrConfigs.Where(cfg => cfg.Domain.Equals(domain));
        }
    }
}
