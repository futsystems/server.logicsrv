using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.FinService
{

    /// <summary>
    /// 服务工厂
    /// 通过指定 servicep对象来生成对应的服务
    /// 
    /// </summary>
    public class ServiceFactory
    {

        public static IFinService GenFinService(FinServiceStub stub)
        {
            LibUtil.Debug("ServiceFactory 生成FinService");
            Type type = FinTracker.ServicePlaneTracker.GetFinServiceType(stub.serviceplan_fk);
            LibUtil.Debug("Type:" + type.ToString());
            if (type != null)
            {
                //生成FinService对象
                IFinService finservice = (IFinService)Activator.CreateInstance(type);
                return finservice;
            }
            return null;
        }
    }
}
