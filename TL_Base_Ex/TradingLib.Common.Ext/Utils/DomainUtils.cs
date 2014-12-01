using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class DomainUtils
    {
        /// <summary>
        /// 获得某个域的Root Manager
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Manager GetRootManager(this Domain domain)
        {
            return BasicTracker.ManagerTracker.Managers.Where(mgr => mgr.domain_id == domain.ID).FirstOrDefault();
        }

         /// <summary>
        /// 获得域内Account
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static IEnumerable<IAccount> GetAccounts(this Domain domain)
        {
            return TLCtxHelper.CmdAccount.Accounts.Where(acc => acc.Domain.Equals(domain));
        }

        /// <summary>
        /// 获得域内Vendor
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<VendorImpl> GetVendors(this Domain domain)
        {
            return BasicTracker.VendorTracker.Vendors.Where(vendor => vendor.domain_id==domain.ID);
        }

        /// <summary>
        /// 获得域内所有交易通道
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<IBroker> GetBrokers(this Domain domain)
        {
            return domain.GetVendors().Where(v => v.Broker != null).Select(v => v.Broker);
        }

        /// <summary>
        /// 获得域内Manager
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<Manager> GetManagers(this Domain domain)
        {
            return BasicTracker.ManagerTracker.Managers.Where(manger => manger.domain_id == domain.ID);
        }

        /// <summary>
        /// 获得域内ConnectorConfig
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<ConnectorConfig> GetConnectorConfigs(this Domain domain)
        {
            return BasicTracker.ConnectorConfigTracker.ConnecotrConfigs.Where(cfg => cfg.domain_id == domain.ID);
        }


        /// <summary>
        /// 获得域内所有路由组
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<RouterGroupImpl> GetRouterGroups(this Domain domain)
        {
            return BasicTracker.RouterGroupTracker.RouterGroups.Where(rg => rg.domain_id == domain.ID);
        }

    }
}
