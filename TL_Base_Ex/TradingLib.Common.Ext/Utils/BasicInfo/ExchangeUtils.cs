using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class ExchangeUtils
    {
        /// <summary>
        /// 获得某个交易所的假日对象
        /// </summary>
        /// <param name="exchagne"></param>
        /// <returns></returns>
        public static Calendar GetCalendar(this IExchange exchagne)
        {
            return BasicTracker.CalendarTracker.GetCalendar(exchagne);
        }


    }
}
