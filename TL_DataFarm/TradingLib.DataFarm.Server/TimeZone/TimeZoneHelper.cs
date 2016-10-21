using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using NodaTime;

namespace TradingLib.Common.DataFarm
{
    public class TimeZoneHelper
    {
        ILog logger = LogManager.GetLogger("");
        Dictionary<string, DateTimeZone> exchangeTimeZoneMap = new Dictionary<string, DateTimeZone>();

        Dictionary<QSEnumDataFeedTypes, DateTimeZone> datafeedTimeZoneMap = new Dictionary<QSEnumDataFeedTypes, DateTimeZone>();


        string GetDataFeedTimeZoneID(QSEnumDataFeedTypes type)
        {
            switch (type)
            { 
                case QSEnumDataFeedTypes.IQFEED:
                    return "US/Eastern";
                default:
                    return "Asia/Shanghai";
            }
        }

        /// <summary>
        /// 获得交易所时区信息
        /// </summary>
        /// <param name="tzId"></param>
        /// <returns></returns>
        DateTimeZone GetTimeZoneByZoneID(string tzId)
        {
            DateTimeZone tz = null;
            if (!exchangeTimeZoneMap.TryGetValue(tzId, out tz))
            {
                tz = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(tzId);
                exchangeTimeZoneMap.Add(tzId, tz);
            }
            return tz;
        }


        /// <summary>
        /// 将行情源时间转换成目标时区时间
        /// 行情源发送的行情对应的时间已经转换成了对应交易所时区的时间
        /// </summary>
        /// <param name="feedTime"></param>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public DateTime ConvertToTimeZone(QSEnumDataFeedTypes datafeedtype,DateTime feedTime,IExchange exchange)
        {
            DateTimeZone exchangeTimeZone = GetTimeZoneByZoneID(exchange.TimeZoneID);
            DateTimeZone feedTimeZone = GetTimeZoneByZoneID(GetDataFeedTimeZoneID(datafeedtype));

            if (exchangeTimeZone != null && feedTimeZone != null)
            {
                LocalDateTime ldt = LocalDateTime.FromDateTime(feedTime);
                ZonedDateTime feedtime = feedTimeZone.AtStrictly(ldt);
                DateTime exchangetime = feedtime.WithZone(exchangeTimeZone).ToDateTimeUnspecified();
                return exchangetime;
            }
            else
            {
                logger.Error(string.Format("Exchange:{0} Time Convert need timezones", exchange.EXCode));
            }
            return DateTime.MinValue;

        }
    }
}
