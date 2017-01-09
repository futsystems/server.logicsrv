using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


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


    }
}
