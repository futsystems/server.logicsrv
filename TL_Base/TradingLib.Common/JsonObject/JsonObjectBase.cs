using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.Json;

namespace TradingLib.Common
{
    public class JsonObjectBase
    {

    }

    public static class JsonObjectBaseUtils
    { 
        public static string ToJson(this JsonObjectBase obj)
        {
            return TradingLib.Mixins.Json.JsonMapper.ToJson(obj);
        }
    }
}
