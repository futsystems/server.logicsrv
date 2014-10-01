using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 用于在某个app domain中创建ServiceFactory
    /// </summary>
    public abstract class ServiceAppDomainFactory
    {
        // Methods
        protected ServiceAppDomainFactory()
        {
        }

        public abstract ServiceFactory CreateFactoryInDomain(AppDomain domain);
    }

 

}
