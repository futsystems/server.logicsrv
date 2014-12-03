using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Contrib.FinService
{
    public static class FinServiceStubUtils
    {
        public static JsonWrapperFinServiceStub ToJsonWrapperFinServiceStub(this FinServiceStub stub)
        {
            JsonWrapperFinServiceStub ret = new JsonWrapperFinServiceStub();

            ret.Account = stub.Acct;
            ret.ID = stub.ID;
            ret.ServicePlaneFK = stub.serviceplan_fk;
            DBServicePlan sp = FinTracker.ServicePlaneTracker[stub.serviceplan_fk];
            ret.ServicePlaneName = sp != null ? sp.Title : "空";
            ret.Active = stub.Active;
            ret.FinService = stub.FinService.ToJsonWrapperFinService();
            
            return ret;
        }
    }
}
