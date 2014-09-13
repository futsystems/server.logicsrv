using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Quant;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 服务工厂，用于生成我们需要要的服务实例
    /// </summary>
    public abstract class ServiceFactory : MarshalByRefObject
    {
        // Methods
        protected ServiceFactory()
        {
        }

        public abstract IQService CreateService();
    }

 

}
