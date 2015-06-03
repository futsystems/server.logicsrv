using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.Race
{
    public static class RaceServiceUtils
    {
        /// <summary>
        /// 获得某个比赛服务的开始交易日
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static int GetStartTradingDay(this RaceService rs)
        {
            //如果记录了报名参赛的交易日信息则通过交易日历推断出下一个交易日就是开始交易日
            if (rs.EntrySettleday != 0)
            {
                return TradingCalendar.NextTradingDay(rs.EntrySettleday);
            }
            else
            {
                DateTime dt = Util.ToDateTime(rs.EntryTime);

                //报名当时交易日 如果在结算之前则和报名日期相同，如果结算之后则需要排到下一个交易日，同时为了保证一次有效结算则需要再延长一个交易日
                int day = TradingCalendar.NextTradingDay(Util.ToTLDate(dt));

                return TradingCalendar.NextTradingDay(day);//再延长一个交易日
            }
        }
    }
}
