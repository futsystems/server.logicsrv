using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public class Protocol
    {
        public static T ParseJsonResponse<T>(string json)
        {
            TradingLib.Mixins.LitJson.JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(json);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                T obj = TradingLib.Mixins.JsonReply.ParsePlayload<T>(jd);
                return obj;
            }
            else
            {
                //throw new FutsRspError()
                return default(T);
            }
        }
    }
}
