using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 不同操作系统下的时区映射关系
    /// </summary>
    public class TimeZoneIDPair
    {
        /// <summary>
        /// Windows时区ID
        /// </summary>
        public string Windows_TimeZoneID{get;set;}

        /// <summary>
        /// Unix时区ID
        /// </summary>
        public string Unix_TimeZoneID {get;set;}

        /// <summary>
        /// 时区
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }

    }
}
