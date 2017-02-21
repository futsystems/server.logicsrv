using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.APIService
{
    public class APITracker
    {
        static APITracker defaultinstance;
        static APITracker()
        { 
            defaultinstance = new APITracker();
        }

        private APITracker()
        { 
        
        }

        IdTracker _followIDTracker = new IdTracker();

        /// <summary>
        /// 生成递增订单编号
        /// </summary>
        public static string NextRef
        {
            get
            {
                return defaultinstance._followIDTracker.AssignId.ToString();
            }
        }
    }
}
