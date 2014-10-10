using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    
    public class CoreUtil
    {
        public const string CCFLAT = "CLEARCENTREFLAT";//清算中心强平标识
        public const string CCFLATERROR = "CLEARCENTREFLAT(Error)";//清算中心强平标识
        /// <summary>
        /// 检查当前是否是星期六的0:00->2:30
        /// </summary>
        /// <returns></returns>
        public static bool IsSat230()
        {
            DateTime now = DateTime.Now;
            if(now.DayOfWeek != DayOfWeek.Saturday)//不是星期六 则直接返回false
                return false;
            if (now <DateTime.Parse("2:30"))
                return true;
            return false;
        }
    }
}
