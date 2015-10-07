using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /// <summary>
    /// 交易时间段
    /// 交易时间段指定一个特定的时区,同时包含一组交易小节列表
    /// </summary>
    public class MarketTime2
    {
        /// <summary>
        /// 交易小节列表
        /// </summary>
        public TradingRange[] RangeList { get; set; }

        /// <summary>
        /// 数据库全局编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 时间段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 时间段描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 时区ID
        /// </summary>
        public string TimeZone { get; set; }


    }

    public static class MarketTimeUtils
    {
        /// <summary>
        /// 判断某个时间是否处于交易时间段内
        /// </summary>
        /// <param name="mt"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        public static bool IsInMarketTime(this MarketTime2 mt, DateTime now)
        {
            foreach (var range in mt.RangeList)
            {
                if (range.IsInRange(now))
                    return true;
            }
            return false;
        }
    }

}
