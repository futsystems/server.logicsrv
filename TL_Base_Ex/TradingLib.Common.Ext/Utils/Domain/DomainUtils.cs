using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.Common
{
    public static partial class DomainUtils
    {
        
        /// <summary>
        /// 判定该域是否过期
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static bool IsExpired(this Domain domain)
        {
            //设置了有效的过期时间并且过期日小于当前日期，则过期
            return domain.DateExpired > 0 && domain.DateExpired < Util.ToTLDate();
        }
        /// <summary>
        /// 返回某个域的所有管理员地址
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<ILocation> GetRootLocations(this Domain domain)
        { 
            Predicate<Manager> p = null;
            p =(mgr)=>{
                if(mgr.Domain != null && mgr.domain_id == domain.ID && mgr.Type == QSEnumManagerType.ROOT)
                {
                    return true;
                }
                else
                    return false;
            };
            return TLCtxHelper.Ctx.MessageMgr.GetNotifyTargets(p);
        }


         /// <summary>
        /// 获得域内Account
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static IEnumerable<IAccount> GetAccounts(this Domain domain)
        {
            return TLCtxHelper.ModuleAccountManager.Accounts.Where(acc => acc.Domain.Equals(domain));
        }

        /// <summary>
        /// 查询域内交易帐户出入金操作请求
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<TradingLib.Mixins.JsonObject.JsonWrapperCashOperation> GetAccountCashOperation(this Domain domain)
        {
            return ORM.MCashOpAccount.GetAccountLatestCashOperationTotal().Where(op => domain.IsInDomain(TLCtxHelper.ModuleAccountManager[op.Account]));
        }

        /// <summary>
        /// 查询域内所有代理出入金操作请求
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<TradingLib.Mixins.JsonObject.JsonWrapperCashOperation> GetAgentCashOperation(this Domain domain)
        {
            return ORM.MAgentFinance.GetAgentLatestCashOperationTotal().Where(op => domain.IsInDomain(BasicTracker.ManagerTracker[op.mgr_fk]));
        }

        /// <summary>
        /// 返回该域的所有权限模板
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<UIAccess> GetUIAccesses(this Domain domain)
        {
            return BasicTracker.UIAccessTracker.UIAccesses.Where(ui => ui.domain_id == domain.ID);
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
        /// 获得域内实盘ConnectorConfig
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<ConnectorConfig> GetConnectorConfigs(this Domain domain)
        {
            return BasicTracker.ConnectorConfigTracker.ConnecotrConfigs.Where(cfg => cfg.domain_id == domain.ID).Where(cfg=>cfg.NeedVendor);
        }

        /// <summary>
        /// 获得域内默认通道设置 行情与模拟成交 通道设置 默认通道设置不用绑定vendor
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<ConnectorConfig> GetDefaultConnectorConfigs(this Domain domain)
        {
            return BasicTracker.ConnectorConfigTracker.ConnecotrConfigs.Where(cfg => cfg.domain_id == domain.ID).Where(cfg => !cfg.NeedVendor);
        }

        

        /// <summary>
        /// 获得域可以设置的Interface
        /// 超级管理员可以获得所有接口
        /// 分区管理员只能获得设置范围内的接口
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<ConnectorInterface> GetInterface(this Domain domain)
        {
            if (domain.Super)//如果是超级管理员则返回所有
            {
                return BasicTracker.ConnectorConfigTracker.Interfaces;
            }
            else
            {
                int[] idlist = domain.InterfaceList.Split(',').Select(s => int.Parse(s)).ToArray();
                return BasicTracker.ConnectorConfigTracker.Interfaces.Where(itface => idlist.Contains(itface.ID));
            }
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

        /// <summary>
        /// 查找域下某个路由组
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="rgid"></param>
        /// <returns></returns>
        public static RouterGroupImpl GetRouterGroup(this Domain domain, int rgid)
        {
            return domain.GetRouterGroups().FirstOrDefault(rg => rg.ID == rgid);
        }
    }
}
