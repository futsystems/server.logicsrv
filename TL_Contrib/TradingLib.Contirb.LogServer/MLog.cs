using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;
using TradingLib.Contirb.LogServer;
using TradingLib.Contirb.Protocol;

namespace TradingLib.ORM
{
  

    public class MLog:MBase
    {
        /// <summary>
        /// 插入持仓明细
        /// </summary>
        /// <param name="?"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static void InsertLogTaskEvent(LogTaskEvent log)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into log_system_task (`settleday`,`uuid`,`taskname`,`tasktype`,`taskmemo`,`date`,`time`,`result`,`exception`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", log.Settleday, log.UUID, log.TaskName, log.TaskType, log.TaskMemo, log.Date, log.Time, log.Result ? 1 : 0, log.Exception);
                db.Connection.Execute(query);
            }
        }

        public static void InsertLogPacketEvent(LogPacketEvent log)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into log_system_packet (`settleday`,`loginid`,`date`,`time`,`type`,`content`,`iscontrib`,`moduleid`,`cmdstr`,`sessiontype`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", log.Settleday, "", log.Date, log.Time, log.Type, log.Content, 0, log.ModuleID, log.CMDStr, log.SessionType);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 查询任务运行日志
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<LogTaskEvent> SelectLotTaskEvents()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM log_system_task";
                return db.Connection.Query<LogTaskEvent>(query, null);
            }
        }


        /// <summary>
        /// 删除多少天以前的日志记录
        /// 日志记录使用滚动记录的方式，只保存一定的天数 避免数据库记录过度膨胀
        /// </summary>
        /// <param name="days"></param>
        public static void DeleteLogs(int days)
        {
            using (DBMySql db = new DBMySql())
            {
                string query1 = string.Format("delete from log_system_packet where UNIX_TIMESTAMP(date(date))< (UNIX_TIMESTAMP(curdate()) -3600*24*{0});", days);
                string query2 = string.Format("delete from log_system_task where UNIX_TIMESTAMP(date(date))< (UNIX_TIMESTAMP(curdate()) -3600*24*{0});", days);
                db.Connection.Execute(query1);
                db.Connection.Execute(query2);
            }
        }
    }

    

}
