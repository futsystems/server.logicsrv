using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class ExchangeUtils_Ex
    {
        /// <summary>
        /// 获得某个交易所的假日对象
        /// </summary>
        /// <param name="exchagne"></param>
        /// <returns></returns>
        private static Calendar GetCalendar(this IExchange exchagne)
        {
            return BasicTracker.CalendarTracker[exchagne.Calendar];
        }

        /// <summary>
        /// 将系统时间转换成交易所时间
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="systime"></param>
        /// <returns></returns>
        public static DateTime GetTargetTime(this IExchange exchange, DateTime systime)
        {
            DateTime target = systime;
            //如果交易所设定时区 则按该时区获得当前时间
            if (exchange.TimeZoneInfo != null)
            {
                target = TimeZoneInfo.ConvertTime(systime, exchange.TimeZoneInfo);
            }
            return target;
        }

        /// <summary>
        /// 判定某个时间 交易所是否在节假日
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="extime"></param>
        /// <returns></returns>
        public static bool IsInHoliday(this IExchange exchange, DateTime extime)
        {
            //获得日历对象
            Calendar calendar = exchange.GetCalendar();
            return calendar.IsHoliday(extime);
        }




    }
}
