using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib
{
    public class WebMgrUtils
    {
        /// <summary>
        /// 是否在结算后与重置前
        /// </summary>
        /// <returns></returns>
        public static bool IsSettle2Reset()
        {
            DateTime now = DateTime.Now;
            if (now > DateTime.Parse("15:40:5") && now < DateTime.Parse("15:59"))
            {
                return true;
            }
            return false;
        }
    }
}
