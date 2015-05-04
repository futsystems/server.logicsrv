using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class SecurityUtils
    {
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


        static int _actimestart_com = 85500;
        static int _actimeend_com = 85900;
        static int _normaltimestart_com = 90000;

        static int _actimestart_cf = 91000;
        static int _acttimeend_cf = 91400;
        static int _normaltimestart_cf = 91500;

        /// <summary>
        /// 是否处于集合竞价成交时间段
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static bool IsInActionExutionTime(this SecurityFamily sec)
        {
            int now = Util.ToTLTime();
            if (sec.Exchange.EXCode.Equals("CFFEX"))
            {
                return now > _acttimeend_cf && now < _normaltimestart_cf;
            }
            else
            {
                return now > _actimeend_com && now < _normaltimestart_com;
            }
        }
        /// <summary>
        /// 是否处于集合竞价报单时间段
        /// </summary>
        /// <returns></returns>
        public static bool IsInAuctionTime(this SecurityFamily sec)
        {
            int now = Util.ToTLTime();
            if (sec.Exchange.EXCode.Equals("CFFEX"))
            {
                return now > _actimestart_cf && now < _acttimeend_cf;
            }
            else
            {
                return now > _actimestart_com && now < _actimeend_com;
            }
        }
    }
}
