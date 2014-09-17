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
            //通过serviceplane_fk获得度应的dbserviceplane
            int serviceplan_fk = stub.serviceplan_fk;

            //获得DBServicePlane
            DBServicePlan plane = FinTracker.ServicePlaneTracker[serviceplan_fk];

            //动态生成IFinService
            return null;
        }
    }
}
