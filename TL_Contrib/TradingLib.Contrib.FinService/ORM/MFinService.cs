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


        /// <summary>
        /// 为某个交易帐户添加一个配资服务
        /// </summary>
        /// <param name="account"></param>
        /// <param name="sp_fk"></param>
        /// <returns></returns>
        public static bool InsertFinService(FinServiceStub stub)
        { 
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_finservice_service (`acct`,`serviceplan_fk`,`active`,`modifiedtime`) VALUES ( '{0}','{1}','{2}','{3}')",stub.Acct,stub.serviceplan_fk,stub.Active?1:0,stub.ModifiedTime);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => stub.ID = id, "id", "contrib_finservice_service");
                return row > 0;
            }
        }

        /// <summary>
        /// 删除数据库记录
        /// </summary>
        /// <param name="stub"></param>
        /// <returns></returns>
        public static bool DeleteFinService(FinServiceStub stub)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM contrib_finservice_service WHERE id = '{0}'", stub.ID);
                return db.Connection.Execute(query)>=0;
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
