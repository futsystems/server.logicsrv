using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /* 
     *  交易时间段对象
     * 
     * 
     * 
     * 
     * 
     * 
     * **/

    /// <summary>
    /// 交易小节 结算标识
    /// 用于标注交易小节属于当前交易日还是下一个交易日
    /// </summary>
    public enum QSEnumRangeSettleFlag
    {
        /// <summary>
        /// 属于当前交易日
        /// </summary>
        T,
        /// <summary>
        /// 属于下一交易日
        /// </summary>
        T1
    }

    /// <summary>
    /// 交易时间小节
    /// </summary>
    public class TradingRange
    {
        /// <summary>
        /// 结算标识
        /// </summary>
        public QSEnumRangeSettleFlag SettleFlag { get; set; }
        /// <summary>
        /// 开始日
        /// </summary>
        public DayOfWeek StartDay { get; set; }
        /// <summary>
        /// 结束日
        /// </summary>
        public DayOfWeek EndDay { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public int StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public int EndTime { get; set; }
    }

    public static class TradingRangeUtils
    {
        /// <summary>
        /// 判断某个时间是否在交易小节之内
        /// 注意给定的时间为与交易时间段时区相同的时间
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        internal static bool IsInRange(this TradingRange range, DateTime time)
        {
            DayOfWeek w = time.DayOfWeek;
            int t = Util.ToTLTime(time);//获得时间
            //如果当前所处日期在开始和结束日期之外
            if (w < range.StartDay) return false;
            if (w > range.EndDay) return false;

            if (w == range.StartDay && t < range.StartTime) return false;
            if (w == range.EndDay && t > range.EndTime) return false;
            return true;
        }
    }
}
