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
        /// 获得域下所有计算策略模板
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<ExStrategyTemplate> GetExStrategyTemplate(this Domain domain)
        {
            return BasicTracker.ExStrategyTemplateTracker.ExStrategyTemplates.Where(t => t.Domain_ID == domain.ID);
        }
    }
}
