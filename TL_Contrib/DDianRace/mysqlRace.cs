using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MySql;
using MySql.Data.MySqlClient;
using System.Data;

namespace Lottoqq.Race
{
    public class mysqlRace:mysqlDBBase
    {
        /// <summary>
        /// 插入一条某个账户的比赛 记录变动
        /// </summary>
        /// <param name="account"></param>
        /// <param name="time"></param>
        /// <param name="sourceid"></param>
        /// <param name="source"></param>
        /// <param name="destid"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool InsertRaceStatusChange(string account, DateTime time, string sourceid, QSEnumAccountRaceStatus source, string destid, QSEnumAccountRaceStatus dest)
        {
            this.SqlReady();
            string sql = String.Format("Insert into `race_account_status_log` (`account`,`datetime`,`source_race_id`,`source_status`,`dest_race_id`,`dest_status`) values('{0}','{1}','{2}','{3}','{4}','{5}')", account, time.ToString(), sourceid, source.ToString(), destid, dest.ToString());
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }

        /// <summary>
        /// 更新某个账户的比赛状态
        /// </summary>
        /// <param name="account"></param>
        /// <param name="raceid"></param>
        /// <param name="status"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool UpdateAccountRaceStatus(string account, string raceid, QSEnumAccountRaceStatus status, DateTime time)
        {
            this.SqlReady();
            string sql = String.Format("UPDATE accounts SET race_id = '{0}' ,race_status = '{1}', entry_time='{2}'  WHERE account = '{3}'", raceid, status.ToString(), time.ToString(), account);
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);

        }

        /// <summary>
        /// 更新比赛信息 参赛人数，淘汰人数，晋级人数，比赛ID
        /// </summary>
        /// <param name="entrynum"></param>
        /// <param name="eliminatenum"></param>
        /// <param name="promotnum"></param>
        /// <param name="raceid"></param>
        /// <returns></returns>
        public bool UpdateRaceInfo(int entrynum, int eliminatenum, int promotnum, string raceid)
        {
            this.SqlReady();
            string sql = String.Format("UPDATE race_session SET entry_num = '{0}' ,eliminate_num = '{1}', promot_num='{2}'  WHERE race_id = '{3}'", entrynum, eliminatenum, promotnum, raceid);
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }

        /// <summary>
        /// 删除某个帐户的RacePositionTransactoin
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool DelAccountPR(string account)
        {
            this.SqlReady();
            string sql = String.Format("delete from race_postransactions where account = '{0}'", account);
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }
        /// <summary>
        /// 获得所有赛季列表
        /// </summary>
        /// <returns></returns>
        public DataSet getRaceSessions()
        {
            this.SqlReady();
            string sql = String.Format("select * from race_session");
            // 创建一个适配器
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            // 创建DataSet，用于存储数据.
            DataSet retSet = new DataSet();
            // 执行查询，并将数据导入DataSet.
            adapter.Fill(retSet, "race_session");
            return retSet;

        }


        /// <summary>
        /// 获得某个赛季
        /// </summary>
        /// <returns></returns>
        public DataSet getRaceSessions(string raceid)
        {
            this.SqlReady();
            string sql = String.Format("select * from race_session  WHERE race_id = '{0}'", raceid);
            // 创建一个适配器
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            // 创建DataSet，用于存储数据.
            DataSet retSet = new DataSet();
            // 执行查询，并将数据导入DataSet.
            adapter.Fill(retSet, "race_session");
            return retSet;

        }
    }
}
