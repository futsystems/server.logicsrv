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
        /// 更新同步实盘帐户
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static void UpdateSyncVendor(this Domain domain,int id)
        {
            BasicTracker.DomainTracker.UpdateSyncVendor(domain as DomainImpl, id);
        }
    }
}
