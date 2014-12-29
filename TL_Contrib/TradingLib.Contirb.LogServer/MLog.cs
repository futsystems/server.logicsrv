using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;
using TradingLib.Contirb.LogServer;

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
        public static bool InsertLogTaskEvent(LogTaskEvent log)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into log_system_task (`settleday`,`uuid`,`taskname`,`tasktype`,`taskmemo`,`date`,`time`,`result`,`exception`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", log.Settleday, log.UUID, log.TaskName, log.TaskType, log.TaskMemo, log.Date, log.Time, log.Result ? 1 : 0, log.Exception);
                return db.Connection.Execute(query) > 0;
            }
        }

      
    }

    

}
