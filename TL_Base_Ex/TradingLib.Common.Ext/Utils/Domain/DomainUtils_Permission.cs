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
        /// 返回该域的所有权限模板
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<UIAccess> GetUIAccesses(this Domain domain)
        {
            return BasicTracker.UIAccessTracker.UIAccesses.Where(ui => ui.domain_id == domain.ID);
        }
    }
}
