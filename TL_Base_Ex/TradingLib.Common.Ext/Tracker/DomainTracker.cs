using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class DomainTracker
    {
        ConcurrentDictionary<int, DomainImpl> domainmap = new ConcurrentDictionary<int, DomainImpl>();

        public DomainTracker()
        {
            //加载所有Domain
            foreach (DomainImpl domain in ORM.MDomain.SelectDomains())
            {
                domainmap.TryAdd(domain.ID, domain);
            }
        }

        /// <summary>
        /// 按DomainID返回对应的域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DomainImpl this[int id]
        {
            get
            {
                DomainImpl domain = null;
                if (domainmap.TryGetValue(id,out domain))
                {
                    return domain;
                }
                return null;
            }
        }
    }
}
