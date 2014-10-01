using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Lottoqq.Race;
using TradingLib.Mixins.DataBase;


namespace TradingLib.ORM
{

    internal class AccountId
    {
        public string AccId { get; set; }
    }

    /// <summary>
    /// RaceService 数据操作
    /// </summary>
    public class MRaceService : MBase
    {

        /// <summary>
        /// 获得所有比赛服务
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RaceService> SelectRaceService()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT a.id,a.entrytime,a.status,a.lastfutequity,a.lastoptequity,a.lastmjequity,a.settleday,a.accid  from lottoqq_race a";
                IEnumerable<RaceService> result = db.Connection.Query<RaceService, AccountId, RaceService>(query, (rs, acc) => { rs.Account = TLCtxHelper.CmdAccount[acc.AccId]; return rs; }, null, null, false, "accid", null, null).ToArray();
                return result;
            }
        }

        /// <summary>
        /// 添加比赛服务
        /// </summary>
        /// <param name="mjservice"></param>
        /// <returns></returns>
        public static int InsertRaceService(RaceService rs)
        {
            using (DBMySql db = new DBMySql())
            {
                TLCtxHelper.Debug("数据库插入比赛服务记录.............");
                const string query = "INSERT INTO lottoqq_race (entrytime,status,lastfutequity,lastoptequity,lastmjequity,settleday,accid) values (@entrytime,@status,@lastfutequity,@lastoptequity,@lastmjequity,@settleday,@accid)";
                int row = db.Connection.Execute(query, new { entrytime = rs.EntryTime, status = rs.Status.ToString(), lastfutequity = rs.LastFutEquity, lastoptequity = rs.LastOptEquity, lastmjequity = rs.LastMJEquity, settleday = rs.SettleDay, accid = rs.Account.ID });
                //更新对象的Id为数据库里新增的Id,假如增加之后不需要获得新增的对象，
                //只需将对象添加到数据库里，可以将下面的一行注释掉。
                SetIdentity(db.Connection, id => rs.ID = id, "id", "lottoqq_race");

                return row;
            }
        }
        /// <summary>
        /// 更新比赛服务的参赛时间
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static int UpdateEntryTime(RaceService rs)
        {
            using (DBMySql db = new DBMySql())
            {

                const string query = "UPDATE lottoqq_race SET entrytime = @entrytime WHERE id = @ID";
                int row = db.Connection.Execute(query, new { entrytime = rs.EntryTime, ID = rs.ID });

                return row;
            }
        }

        /// <summary>
        /// 更新比赛服务的状态
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static int UpdateRaceStatus(RaceService rs)
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "UPDATE lottoqq_race SET status = @status WHERE id = @ID";
                int row = db.Connection.Execute(query, new { status = rs.Status.ToString(), ID = rs.ID });

                return row;
            }
        }

      


        /// <summary>
        /// 结算更新权益数据和结算日期
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static int UpdateSettle(RaceService rs)
        {
            rs.SettleDay = DateTime.Now;

            using (DBMySql db = new DBMySql())
            {

                const string query = "UPDATE lottoqq_race SET lastfutequity = @lastfutequity,lastoptequity = @lastoptequity,lastmjequity = @lastmjequity,settleday = @settleday WHERE id = @ID";
                int row = db.Connection.Execute(query, new { lastfutequity = rs.LastFutEquity, lastoptequity = rs.LastOptEquity, lastmjequity = rs.LastMJEquity, settleday = rs.SettleDay, ID = rs.ID });

                return row;
            }
        }


        /// <summary>
        /// 更新比赛服务所有字段
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static int UpdateAll(RaceService rs)
        {
            rs.SettleDay = DateTime.Now;

            using (DBMySql db = new DBMySql())
            {
                const string query = "UPDATE lottoqq_race SET lastfutequity = @lastfutequity,lastoptequity = @lastoptequity,lastmjequity = @lastmjequity,settleday = @settleday,entrytime=@entrytime,status=@status WHERE id = @ID";
                int row = db.Connection.Execute(query, new { lastfutequity = rs.LastFutEquity, lastoptequity = rs.LastOptEquity, lastmjequity = rs.LastMJEquity, settleday = rs.SettleDay, status = rs.Status.ToString(), entrytime = rs.EntryTime, ID = rs.ID });


                return row;
            }
        }
    }
}
