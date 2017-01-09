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
        /// 获得某个域的Root Manager
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Manager GetRootManager(this Domain domain)
        {
            return BasicTracker.ManagerTracker.Managers.Where(mgr => mgr.domain_id == domain.ID && mgr.IsRoot()).FirstOrDefault();
        }
    }
}
