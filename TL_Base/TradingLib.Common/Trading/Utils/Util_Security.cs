﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class Util_Security
    {
        /// <summary>
        /// 生成开始与结束日期之间Security的MarketDay
        /// 开始与结束时间应当使用交易所对应的本地时间
        /// </summary>
        /// <param name="security"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Dictionary<int,MarketDay> GetMarketDay(this SecurityFamily security, DateTime start, DateTime end)
        {
            if (start > end) throw new Exception("Start should smaller then end");

            Dictionary<DayOfWeek, List<TradingRange>> dayRangeMap = security.MarketTime.GetRangeOfWeekDay();
            DateTime date = start;
            Dictionary<int, MarketDay> mdmap = new Dictionary<int, MarketDay>();
            while (date <= end)
            {
                DayOfWeek dayofweek = date.DayOfWeek;
                List<TradingRange> rangelist = null;
                //如果当前日期是交易日 则通过tradinglist 生成MarketDay
                if (dayRangeMap.TryGetValue(dayofweek, out rangelist))
                {
                    var item = MarketDay.CreateMarketDay(date, rangelist);
                    mdmap.Add(item.TradingDay,item);
                }
                date = date.AddDays(1);
            }
            return mdmap;
        }

        /// <summary>
        /// 获得某天上一个交易日
        /// </summary>
        /// <param name="security"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static MarketDay GetLastMarketDay(this SecurityFamily security, DateTime date)
        {
            Dictionary<DayOfWeek, List<TradingRange>> dayRangeMap = security.MarketTime.GetRangeOfWeekDay();
           
            MarketDay lastmd = null;
            DateTime seek = date;
            while (lastmd == null || lastmd.TradingDay >= date.ToTLDate())
            {
                seek = seek.AddDays(-1);
                DayOfWeek dayofweek = seek.DayOfWeek;
                List<TradingRange> rangelist = null;
                if (dayRangeMap.TryGetValue(dayofweek, out rangelist))
                {
                    lastmd = MarketDay.CreateMarketDay(seek, rangelist);
                }
                
            }
            return lastmd;
        }

        /// <summary>
        /// 获得某天下一个交易日
        /// </summary>
        /// <param name="security"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static MarketDay GetNextMarketDay(this SecurityFamily security, DateTime date)
        {
            Dictionary<DayOfWeek, List<TradingRange>> dayRangeMap = security.MarketTime.GetRangeOfWeekDay();

            MarketDay nextmd = null;
            DateTime seek = date;
            while (nextmd == null || nextmd.TradingDay <= date.ToTLDate())
            {
                seek = seek.AddDays(1);
                DayOfWeek dayofweek = seek.DayOfWeek;
                List<TradingRange> rangelist = null;
                if (dayRangeMap.TryGetValue(dayofweek, out rangelist))
                {
                    nextmd = MarketDay.CreateMarketDay(seek, rangelist);
                }
                
            }
            return nextmd;
        }

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
