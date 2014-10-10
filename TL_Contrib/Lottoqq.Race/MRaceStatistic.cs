using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottoqq.Race;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    /// <summary>
    /// 比赛统计数据库访问对象
    /// 
    /// </summary>
    public class MRaceStatistic : MBase
    {
        /// <summary>
        /// 插入比赛统计
        /// </summary>
        /// <param name="mjservice"></param>
        /// <returns></returns>
        public static int InsertRaceStatistic(RaceStatistic stat)
        {
            using (DBMySql db = new DBMySql())
            {
                TLCtxHelper.Debug("数据库插入比赛服务记录.............");
                const string query = "INSERT INTO lottoqq_race_statistics (account,userid,optquity,mjequity,futequity,optcommission,mjcommission,futcommission,opttransnum,mjtransnum,futtransnum,optwinnum,mjwinnum,futwinnum,opttotalprofit,mjtotalprofit,futtotalprofit,opttotalloss,mjtotalloss,futtotalloss) values (@account,@userid,@optquity,@mjequity,@futequity,@optcommission,@mjcommission,@futcommission,@opttransnum,@mjtransnum,@futtransnum,@optwinnum,@mjwinnum,@futwinnum,@opttotalprofit,@mjtotalprofit,@futtotalprofit,@opttotalloss,@mjtotalloss,@futtotalloss)";
                int row = db.Connection.Execute(query, new { account = stat.Account, userid = stat.UserID, optquity = stat.OptEquity, mjequity = stat.MJEquity, futequity = stat.FutEquity, optcommission = stat.OptCommission, mjcommission = stat.MJCommission, futcommission = stat.FutCommission, opttransnum = stat.OptTransNum, mjtransnum = stat.MJTransNum, futtransnum = stat.FutTransNum, optwinnum = stat.OptWinNum, mjwinnum = stat.MJWinNum, futwinnum = stat.FutWinNum, opttotalprofit = stat.OptTotalProfit, mjtotalprofit = stat.MJTotalProfit, futtotalprofit = stat.FutTotalProfit, opttotalloss = stat.OptTotalLoss, mjtotalloss = stat.MJTotalLoss, futtotalloss = stat.FutTotalLoss });
                //更新对象的Id为数据库里新增的Id,假如增加之后不需要获得新增的对象，
                //只需将对象添加到数据库里，可以将下面的一行注释掉。
                SetIdentity(db.Connection, id => stat.ID = id, "id", "lottoqq_race_statistics");
                return row;
            }
        }

        /// <summary>
        /// 清空比赛统计
        /// </summary>
        public static void ClearRaceStatic()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "DELETE FROM lottoqq_race_statistics";
                int row = db.Connection.Execute(query, null);
            }
        }

        /// <summary>
        /// 获得所有PR累计统计数据
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PRStatistic> SelectPRStatistic()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT account as account,type as type ,sum(commission) as totalcommission,count(*) as totaltransnum,sum((case when(netprofit >= 0) then netprofit end)) AS `totalprofit`,sum((case when(netprofit < 0) then netprofit end)) AS `totalloss`,sum((case when(netprofit > 0) then 1 end)) AS `totalwinnum` FROM lottoqq_race_postransactions group by account,type";
                IEnumerable<PRStatistic> result = db.Connection.Query<PRStatistic>(query, null);
                return result;
            }
        }
    }
}
