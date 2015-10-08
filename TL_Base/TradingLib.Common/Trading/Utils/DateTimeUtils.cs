using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class DateTimeUtils
    {
        /// <summary>
        /// 判断某个时间是否是工作日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsWorkDay(this DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                return false;
            return true;
        }

        /// <summary>
        /// 求某个时间的下一个工作日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime NextWorkDay(this DateTime dt)
        {
            //在当前日期上加一日
            DateTime workday = dt.Date.AddDays(1);
            //循环判断workday是否是节假日 如果不是则加一日
            while (true)
            {
                if (workday.IsWorkDay())
                {
                    return workday;
                }
                workday = workday.Date.AddDays(1);
            }
        }

        /// <summary>
        /// 求某个时间的上一个工作日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime LastWorkDay(this DateTime dt)
        {
            //在当前日期上加一日
            DateTime workday = dt.Date.AddDays(-1);
            //循环判断workday是否是节假日 如果不是则加一日
            while (true)
            {
                if (workday.IsWorkDay())
                {
                    return workday;
                }
                workday = workday.Date.AddDays(-1);
            }
        }

        public static int ToTLDate(this DateTime dt)
        {
            return Util.ToTLDate(dt);
        }

        public static int ToTLTime(this DateTime dt)
        {
            return Util.ToTLTime(dt);
        }

        public static long ToTLDateTime(this DateTime dt)
        {
            return Util.ToTLDateTime(dt);
        }
    }
}
