using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class UtilTradingRange
    {/// <summary>
        /// 判断某个时间是否在交易小节之内
        /// 注意给定的时间为与交易时间段时区相同的时间
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static bool IsInRange(this TradingRange range, DateTime time)
        {
            DayOfWeek w = time.DayOfWeek;
            int t = Util.ToTLTime(time);//获得时间
            // 星期星期日:0一:1 .... 星期六:6
            //起止日期 星期一到星期二， 星期天到星期一
            if (range.StartDay <= range.EndDay)
            {
                //如果当前所处日期在开始和结束日期之外
                if (w < range.StartDay) return false;
                if (w > range.EndDay) return false;

                if (w == range.StartDay && t < range.StartTime) return false;
                if (w == range.EndDay && t > range.EndTime) return false;
                //以上通过排除法 将不在区间外的时间排除
                return true;
            }
            else //星期6到星期一
            {
                if (w < range.StartDay && w > range.EndDay) return false;
                if (w == range.StartDay && t < range.StartTime) return false;
                if (w == range.EndDay && t > range.EndTime) return false;
                return true;
            }
        }

        /// <summary>
        /// 判断交易小节上某个时间点 所属交易日
        /// 注该日期需要和对应的交易所时间一致
        /// 交易小节是一个规律性的时间段规则，需要提供具体的交易时间才可以判定交易日
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime TradingDay(this TradingRange range, DateTime extime)
        {
            if (!range.IsInRange(extime))
            {
                throw new ArgumentException("提供的时间必须在交易小节内");
            }

            //交易小节开始于结束在同一天
            if (range.StartDay == range.EndDay)
            {
                if (range.SettleFlag == QSEnumRangeSettleFlag.T)
                {
                    return extime.Date;
                }
                else if (range.SettleFlag == QSEnumRangeSettleFlag.T1)
                {
                    return extime.Date.NextWorkDay();
                }
                return extime;
            }
            else if (range.StartDay < range.EndDay)
            {
                //如果是星期日 则属于星期一对应的结算日
                if (range.StartDay == DayOfWeek.Sunday) //不存在T 和 T+1的判断
                {
                    if (extime.DayOfWeek == range.StartDay)
                    {
                        return extime.Date.NextWorkDay();
                    }
                    else if (extime.DayOfWeek == range.EndDay)
                    {
                        return extime.Date;
                    }
                }

                //当前时间在交易小节前半段 星期4晚上 9:00到星期5凌晨2点(T+1)，在星期四时间段内 则对应的交易日为星期四对应的交易日+1
                if (extime.DayOfWeek == range.StartDay)
                {
                    if (range.SettleFlag == QSEnumRangeSettleFlag.T)
                    {
                        return extime.Date;
                    }
                    else if (range.SettleFlag == QSEnumRangeSettleFlag.T1)
                    {
                        return extime.Date.NextWorkDay();
                    }
                    return extime.Date;
                }
                //当前时间在交易小节后半段 星期4晚上 9:00到星期5凌晨2点(T+1)，在星期五时间段内 则对应的交易日为星期五对应的日期
                //我们假定每个交易小节只属于一个交易日不跨越多个交易日
                else if (extime.DayOfWeek == range.EndDay)
                {
                    if (range.SettleFlag == QSEnumRangeSettleFlag.T)
                    {

                        return extime.Date.AddDays(-1);
                    }
                    else if (range.SettleFlag == QSEnumRangeSettleFlag.T1)
                    {
                        return extime.Date.AddDays(-1).NextWorkDay();
                    }
                }

            }
            else //range.StartDay>range.EndDay
            {

            }

            return extime.Date;

        }
    }
}
