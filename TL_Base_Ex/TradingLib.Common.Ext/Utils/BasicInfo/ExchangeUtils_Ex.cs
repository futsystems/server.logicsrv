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

        /// <summary>
        /// 判定某个时间是否是特殊假日
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="extime"></param>
        /// <returns></returns>
        public static bool IsInSpecialHoliday(this IExchange exchange, DateTime extime)
        {
            Calendar calendar = exchange.GetCalendar();
            return calendar.IsSpecialHoliday(extime);
        }
       

        /// <summary>
        /// 交易所返回某个日期的下一个非假期工作日
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="extime"></param>
        /// <returns></returns>
        public static DateTime NextWorkDayWithoutHoliday(this IExchange exchange, DateTime extime)
        {
            //获取给定时间的下一个工作日
            DateTime workday = extime.NextWorkDay(); 
            //循环判断workday是否是节假日 如果是节假日则去下一个工作日
            while (true)
            {
                if (!exchange.IsInHoliday(workday))
                {
                    return workday;
                }
                workday = workday.NextWorkDay();
            }
        }

        /// <summary>
        /// 特例
        /// 1.星期六 凌晨交易时间段的判定
        /// 2.星期日 交易时间段的判定
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="extime"></param>
        /// <returns></returns>
        public static int CurrentTradingDay(this IExchange exchange, DateTime extime)
        { 
            //
            if (!extime.IsWorkDay()) return 0; //如果是非工作日 则返回0
            if (exchange.IsInHoliday(extime)) return 0;//如果是节假日 则返回0
            DateTime tradingday = extime;
            //如果当前时间在交易所收盘之后 则进入下一个交易日
            if (Util.ToTLTime(extime) > exchange.CloseTime)
            {
                tradingday = exchange.NextWorkDayWithoutHoliday(tradingday);
            }


            return 0;
        }





    }
}


