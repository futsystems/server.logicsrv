using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

using TradingLib.Mixins.JsonObject;


namespace TradingLib.Contrib.FinService
{
    public static class IFinServiceUtils
    {
        public static IEnumerable<ArgumentAttribute> GetAttribute(this IFinService fs)
        {
            return FinTracker.ServicePlaneTracker.GetAttribute(fs);
        }


        public static IEnumerable<JsonWrapperArgument> GetJsonWrapperArgument(this IFinService fs)
        {
            ServicePlanBase sp = fs as ServicePlanBase;
            IEnumerable<ArgumentAttribute> attrlist = GetAttribute(fs);
            Dictionary<string, Argument> argumap= sp.AccountArgumentMap;
            List<JsonWrapperArgument> list = new List<JsonWrapperArgument>();
            foreach (string key in argumap.Keys)
            {
                Argument arg = argumap[key];
                string name = arg.Name;
                string value = arg.Value;
                string type = arg.Type.ToString();
                ArgumentAttribute attr = attrlist.Where(a => a.Name.Equals(name)).SingleOrDefault();
                if (attr == null)
                {
                    continue;
                }

                string title = attr.Title;
                
                bool editable = attr.Editable;
                list.Add(new JsonWrapperArgument { 
                    ArgName = name,
                    ArgTitle = title,
                    ArgValue = value,
                    ArgType = type,
                    Editable = editable,
                    
                });

            }
            return list;

        }

        public static JsonWrapperFinService ToJsonWrapperFinService(this IFinService fs)
        {
            JsonWrapperFinService ret = new JsonWrapperFinService();
            ret.ChargeType = Util.GetEnumDescription(fs.ChargeType);//.ToString();
            ret.CollectType = Util.GetEnumDescription(fs.CollectType);//.ToString();
            ret.Arguments = GetJsonWrapperArgument(fs).ToArray();
            return ret;
        }
    }
}
