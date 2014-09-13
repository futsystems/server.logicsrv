using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using TradingLib.API;

namespace TradingLib.MySql
{
    /// <summary>
    /// 用于读写比赛相关的信息到数据库
    /// </summary>
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
        public bool InsertRaceStatusChange(string account, DateTime time,string sourceid, QSEnumAccountRaceStatus source, string destid, QSEnumAccountRaceStatus dest)
        {
            this.SqlReady();
            string sql = String.Format("Insert into `race_account_status_log` (`account`,`datetime`,`source_race_id`,`source_status`,`dest_race_id`,`dest_status`) values('{0}','{1}','{2}','{3}','{4}','{5}')", account,time.ToString(),sourceid,source.ToString(),destid,dest.ToString());
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
        public bool UpdateAccountRaceStatus(string account, string raceid, QSEnumAccountRaceStatus status,DateTime time)
        {
            this.SqlReady();
            string sql = String.Format("UPDATE accounts SET race_id = '{0}' ,race_status = '{1}', entry_time='{2}'  WHERE account = '{3}'",raceid,status.ToString(),time.ToString(),account);
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
        public bool UpdateRaceInfo(int entrynum, int eliminatenum, int promotnum,string raceid)
        {
            this.SqlReady();
            string sql = String.Format("UPDATE race_session SET entry_num = '{0}' ,eliminate_num = '{1}', promot_num='{2}'  WHERE race_id = '{3}'",entrynum,eliminatenum,promotnum,raceid);
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }

        /// <summary>
        /// 新开一个初赛
        /// </summary>
        /// <param name="raceid"></param>
        /// <param name="start"></param>
        /// <param name="daysopen"></param>
        /// <returns></returns>
        public bool NewPrerace(string raceid,DateTime start,int daysopen)
        {
            this.SqlReady();
            string sql = String.Format("Insert into `race_session` (`starttime`,`begin_sign_time`,`end_sign_time`,`race_id`) values('{0}','{1}','{2}','{3}')",start,start,start.AddDays(daysopen),raceid);
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
            string sql = String.Format("delete from race_postransactions where account = '{0}'",account);
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }
        /// <summary>
        /// 获得所有赛季列表
        /// </summary>
        /// <returns></returns>
        public  DataSet getRaceSessions()
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
            string sql = String.Format("select * from race_session  WHERE race_id = '{0}'",raceid);
            // 创建一个适配器
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            // 创建DataSet，用于存储数据.
            DataSet retSet = new DataSet();
            // 执行查询，并将数据导入DataSet.
            adapter.Fill(retSet, "race_session");
            return retSet;

        }


        #region 比赛统计 
        /// <summary>
        /// 获得所有比赛统计数据(数据库语言对帐户的相关信息进行的统计)
        /// </summary>
        /// <returns></returns>
        public DataSet GetRaceStatistics()
        {
            this.SqlReady();
            string sql = String.Format("SELECT * FROM viewprofit_account_summary");
            // 创建一个适配器
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            // 创建DataSet，用于存储数据.
            DataSet retSet = new DataSet();
            // 执行查询，并将数据导入DataSet.
            adapter.Fill(retSet, "racestatistics");
            return retSet;
        }
        /// <summary>
        /// 插入账户比赛统计信息
        /// </summary>
        public bool InsertRaceStatistics(string account, string race_id, string race_status, DateTime entry_time, int race_day,
            decimal nowequity, decimal obverseequity, decimal commission, int pr_num, int winnum, int lossnum,
            decimal avg_profit, decimal avg_loss, int winday, int seqwinday, int lossday, int seqlossday, decimal avg_postransperday, decimal avg_posholdtime, decimal totalperformance, decimal entryperformance, decimal exitperformance, decimal winpercent, decimal profitfactor)
        {
            this.SqlReady();
            string sql = String.Format("Insert into race_statistics(`account`,`race_id`,`race_status`,`race_entrytime`,`race_day`,`now_equity`,`obverse_equity`,`commission`,`postrans_num`,`win_num`,`loss_num`,`avg_profit`,`avg_loss`,`win_day`,`seqwin_day`,`loss_day`,`seqloss_day`,`avg_postransperday`,`avg_posholdtime`,`total_performance`,`entry_performance`,`exit_performance`,`winpercent`,`profitfactor`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}')", account, race_id, race_status, entry_time, race_day, nowequity, obverseequity, commission, pr_num, winnum, lossnum, avg_profit, avg_loss, winday, seqwinday, lossday, seqlossday, avg_postransperday, avg_posholdtime, totalperformance, entryperformance, exitperformance, winpercent, profitfactor);
            cmd.CommandText = sql;
            return ((cmd.ExecuteNonQuery() > 0));
        }


        /// <summary>
        /// 清空比赛状态
        /// </summary>
        /// <returns></returns>
        public bool ClearRaceStatistics()
        {
            this.SqlReady();
            string sql = String.Format("delete from race_statistics");
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() >= 0);
        }
        /// <summary>
        /// 获得选手的每日盈亏
        /// </summary>
        /// <param name="account"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public DataSet ReTotalDaily(string account, DateTime start, DateTime end)
        {
            this.SqlReady();
            string sql = String.Format("SELECT account,settleday,realizedpl,unrealizedpl,commission,if(realizedpl+unrealizedpl-commission>25000 AND settleday >= '2013-07-29',25000,realizedpl+unrealizedpl-commission) as netprofit FROM settlement where account='{0}' and settleday>='{1}' and settleday<='{2}' and commission>0;", account, start.ToString(), end.ToString());
            // 创建一个适配器
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            // 创建DataSet，用于存储数据.
            DataSet retSet = new DataSet();
            // 执行查询，并将数据导入DataSet.
            adapter.Fill(retSet, "settlement");
            return retSet;
        }


        #endregion 

    }
}
