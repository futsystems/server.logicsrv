using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace Lottoqq.Race
{
    public class StatisticTracker
    {

        static ConcurrentDictionary<string, PRStatistic> accountprstatisticmap = new ConcurrentDictionary<string, PRStatistic>();

        /// <summary>
        /// 重新充数据库运算并加载PR统计
        /// </summary>
        public static void ReCollectPRStatistic()
        {
            accountprstatisticmap.Clear();
            foreach (PRStatistic prs in TradingLib.ORM.MRaceStatistic.SelectPRStatistic())
            {
                accountprstatisticmap.TryAdd(RaceHelper.PRStatisticKey(prs), prs);
            }
        }

        /// <summary>
        /// 获得某个帐户某个品种的持仓回合统计数据
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PRStatistic GetPRStatistic(string account, SecurityType type)
        {
            string key = account + "-" + type.ToString();
            PRStatistic prs=null;
            if (accountprstatisticmap.TryGetValue(key,out prs))
            {
                return prs;
            }
            return null;
        }

        /// <summary>
        /// 填充比赛统计数据
        /// </summary>
        /// <param name="rs"></param>
        public static void FillRaceStatistic(RaceStatistic rs)
        {
            rs.FillFutStatistic(GetPRStatistic(rs.Account, SecurityType.FUT));
            rs.FillOptStatistic(GetPRStatistic(rs.Account, SecurityType.OPT));
            rs.FillMJStatistic(GetPRStatistic(rs.Account, SecurityType.INNOV));
        }
    }
}
