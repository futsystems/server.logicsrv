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
        /// 获得PriceTick对应的格式化输出样式
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static string GetPriceFormat(this SecurityFamily sec)
        {
            decimal pricetick = sec.PriceTick;
            string[] p = pricetick.ToString().Split('.');
            if (p.Length <= 1)
                return "{0:F0}";
            else
                return "{0:F" + p[1].ToCharArray().Length.ToString() + "}";
        }

        /// <summary>
        /// 获得某个品种的小数位数
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static int GetDecimalPlaces(this SecurityFamily sec)
        {
            return sec.PriceTick.GetDecimalPlaces();
        }


    }
}
