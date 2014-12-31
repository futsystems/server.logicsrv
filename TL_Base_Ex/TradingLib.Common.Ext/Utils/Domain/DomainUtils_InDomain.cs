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
        /// 某个Manger是否在某个域内
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static bool IsInDomain(this Domain domain, Manager mgr)
        {
            if (mgr == null) return false;
            return mgr.domain_id.Equals(domain.ID);
        }


        /// <summary>
        /// 某个Account是否属于某域
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool IsInDomain(this Domain domain, IAccount account)
        {
            if (account == null) return false;
            return account.Domain.ID.Equals(domain.ID);
        }


        /// <summary>
        /// 交易帐户是否在某个域内
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool IsAccountInDomain(this Domain domain, string account)
        {
            IAccount acc = TLCtxHelper.CmdAccount[account];
            return domain.IsInDomain(acc);
        }

        /// <summary>
        /// 判断某个管理员是否属于某域
        /// </summary>
        /// <param name="doman"></param>
        /// <param name="manger"></param>
        /// <returns></returns>
        public static bool IsManagerInDomain(this Domain doman, string manger)
        {
            Manager mgr = BasicTracker.ManagerTracker[manger];
            return doman.IsInDomain(mgr);
        }

        public static bool IsManagerInDomain(this Domain domain, int mgr_fk)
        {
            Manager mgr = BasicTracker.ManagerTracker[mgr_fk];
            return domain.IsInDomain(mgr);
        }


    }
}
