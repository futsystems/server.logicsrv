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
    public class MFinService:MBase
    {
        /// <summary>
        /// 获得所有的配资服务
        /// </summary>
        /// <returns></returns>
        public static FinServiceStub[] SelectFinService()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_finservice_service";
                return db.Connection.Query<FinServiceStub>(query, null).ToArray();
            }
        }


        //public static bool InsertServicePlan(DBServicePlan sp)
        //{
        //    using (DBMySql db = new DBMySql())
        //    {
        //        string query = string.Format("INSERT INTO contrib_finservice_serviceplan (`name`,`title`,`classname`) VALUES ( '{0}','{1}','{2}')",sp.Name,sp.Title,sp.ClassName);
        //        int row = db.Connection.Execute(query);
        //        SetIdentity(db.Connection, id => sp.ID = id, "id", "contrib_finservice_serviceplan");
        //        return row > 0;
        //    }
        //}
    }
}
