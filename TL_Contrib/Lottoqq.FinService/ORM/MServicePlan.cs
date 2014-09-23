using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Contrib.FinService;
using TradingLib.Mixins.DataBase;


namespace TradingLib.Contrib.FinService.ORM
{
    public class MServicePlan:MBase
    {
        /// <summary>
        /// 获得所有服务计划
        /// </summary>
        /// <returns></returns>
        public static DBServicePlan[] SelectServicePlan()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_finservice_serviceplan";
                return db.Connection.Query<DBServicePlan>(query, null).ToArray();
            }
        }


        public static bool InsertServicePlan(DBServicePlan sp)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_finservice_serviceplan (`name`,`title`,`classname`) VALUES ( '{0}','{1}','{2}')",sp.Name,sp.Title,sp.ClassName);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => sp.ID = id, "id", "contrib_finservice_serviceplan");
                return row > 0;
            }
        }
    }
}
