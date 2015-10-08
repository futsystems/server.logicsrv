using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class SecurityUtils
    {
        public static string GetSecurityName(this SecurityFamily sec)
        {
            if (sec != null)
            {
                return sec.Name;
            }
            return "未知";
        }

        public static int GetMultiple(this SecurityFamily sec)
        {
            if (sec != null)
            {
                return sec.Multiple;
            }
            return 1;
        }

        /// <summary>
        /// 获得品种交易小节
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="extime"></param>
        /// <returns></returns>
        //public static TradingRange JudgeRange(this SecurityFamily sec,DateTime extime)
        //{
        //    if (sec.MarketTime != null)
        //    {
        //        return sec.MarketTime.JudgeRange(extime);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


    }
}
