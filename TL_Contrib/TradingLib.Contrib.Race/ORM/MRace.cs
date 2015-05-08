using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

using TradingLib.ORM;
using TradingLib.Mixins.DataBase;


namespace TradingLib.Contrib.Race.ORM
{
    public class MRace:MBase
    {

        /// <summary>
        /// 获得所有比赛
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Race> SelectRaces()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_race_session";
                return db.Connection.Query<Race>(query, null).ToArray();
            }
        }

        public static void UpdateRace(Race race)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE  contrib_race_session SET entrynum='{0}' ,eliminatenum='{1}' ,promotnum='{2}' WHERE raceid='{3}'", race.EntryNum,race.EliminateNum,race.PromotNum,race.RaceID);
                db.Connection.Execute(query);
            }
        }

        public static void InsertRace(RaceSetting race)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_race_session (`entrynum`,`eliminatenum`,`promotnum`,`racetype`,`starttime`,`biginsigntime`,`engsigntime`,`raceid`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", race.EntryNum, race.EliminateNum, race.PromotNum, race.RaceType,race.StartTime,race.BeginSignTime,race.EndSignTime,race.RaceID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 获得所有比赛服务
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RaceService> SelectRaceServices()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_race_service";
                return db.Connection.Query<RaceService>(query, null).ToArray();
            }
        }

        

        /// <summary>
        /// 插入比赛服务
        /// </summary>
        /// <param name="rs"></param>
        public static void InsertRaceService(RaceService rs)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_race_service (`account`,`raceid`,`entrytime`,`racestatus`) VALUES ( '{0}','{1}','{2}','{3}')",rs.Account,rs.RaceID,rs.EntryTime,rs.RaceStatus);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新比赛服务
        /// </summary>
        /// <param name="rs"></param>
        public static void UpdateRaceService(RaceService rs)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE  contrib_race_service SET raceid='{0}' ,entrytime='{1}' ,racestatus='{2}' WHERE account='{3}'",rs.RaceID,rs.EntryTime,rs.RaceStatus,rs.Account);
                db.Connection.Execute(query);
            }
        }



        /// <summary>
        /// 插入交易帐户比赛状态变动过程
        /// </summary>
        /// <param name="change"></param>
        public static void InsertRaceChangeLog(RaceStatusChange change)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_race_log (`account`,`datetime`,`srcstatus`,`deststatus`,`srcraceid`,`destraceid`,`settleday`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}')",change.Account,change.DateTime,change.SrcStatus,change.DestStatus,change.SrcRaceID,change.DestRaceID,TLCtxHelper.CmdSettleCentre.LastSettleday);
                db.Connection.Execute(query);
            }
        }
    }
}
