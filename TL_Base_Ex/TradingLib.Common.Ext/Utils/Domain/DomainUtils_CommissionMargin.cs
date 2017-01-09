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
        /// 获得域下所有手续费模板
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<CommissionTemplate> GetCommissionTemplate(this Domain domain)
        {
            return BasicTracker.CommissionTemplateTracker.CommissionTemplates.Where(t => t.Domain_ID == domain.ID);
        }

        /// <summary>
        /// 获得域下所有保证金模板
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<MarginTemplate> GetMarginTemplate(this Domain domain)
        {
            return BasicTracker.MarginTemplateTracker.MarginTemplates.Where(t => t.Domain_ID == domain.ID);
        }

    }
}
