using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class RspInfoTracker
    {
        static XMLRspInfoTracker webapitracker = null;
        static RspInfoTracker()
        {
            webapitracker = new XMLRspInfoTracker("WEBAPI","webapierror.xml","errors");
        }

        private RspInfoTracker()
        { 
            
        }

        /// <summary>
        /// 获得web同步调用的回报信息维护器
        /// </summary>
        public static XMLRspInfoTracker WebAPIRspInfo
        {
            get
            {
                if (webapitracker == null)
                    webapitracker = new XMLRspInfoTracker("WEBAPI","webapierror.xml", "errors");
                return webapitracker;
            }
        }


    }
}
