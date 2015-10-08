using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 交易所状态
    /// </summary>
    public class ExchangeStatus
    {
        /// <summary>
        /// 当前交易日
        /// </summary>
        public int TradingDay { get; set; }


    }


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

        public static DateTime GetExchangeTime(this IExchange exchange)
        {
            return exchange.GetExchangeTime(DateTime.Now);
        }
        /// <summary>
        /// 将系统时间转换成交易所时间
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="systime"></param>
        /// <returns></returns>
        public static DateTime GetExchangeTime(this IExchange exchange, DateTime systime)
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
        /// 将交易所时间转换成本地系统时间
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="extime"></param>
        /// <returns></returns>
        public static DateTime GetSystemTime(this IExchange exchange, DateTime extime)
        {
            DateTime target = extime;
            if (exchange.TimeZoneInfo != null)
            {
                target = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(extime, exchange.TimeZone, "China Standard Time");
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

        /// <summary>
        /// 交易所当前交易日
        /// </summary>
        /// <returns></returns>
        //public static int TradingDay(this IExchange exchange)
        //{
        //    DateTime extime = exchange.GetTargetTime(DateTime.Now);//获得交易所时间

        //    if (Util.ToTLTime(extime) > exchange.CloseTime)
        //    {
        //        return extime.NextWorkDay();
        //    }
        //}
        
        ///// <summary>
        ///// 给定某个交易所时间 计算该时间的下一个交易日
        ///// </summary>
        ///// <param name="exchange"></param>
        ///// <param name="extime"></param>
        ///// <returns></returns>
        //public static DateTime NextTradingDay(this IExchange exchange, DateTime extime)
        //{ 
            
        //}

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


