using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class DomainUtils
    {

    }

    public static class DomainObjectSet
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<IDomainPartition> SameDomain(this IEnumerable<IDomainPartition> set, Domain domain)
        {
            return set.Where(obj => obj.Domain.Equals(domain));
        }
    
    }
}
