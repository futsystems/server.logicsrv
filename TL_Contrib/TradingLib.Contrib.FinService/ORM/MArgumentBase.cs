using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Mixins.DataBase;


namespace TradingLib.Contrib.FinService.ORM
{
    public class MArgumentBase:MBase
    {

        /// <summary>
        /// 获得基准参数
        /// </summary>
        /// <returns></returns>
        public static ArgumentBase[] SelectArgumentBase()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_finservice_argument_base";
                return  db.Connection.Query<ArgumentBase>(query, null).ToArray();
            }
        }

        /// <summary>
        /// 插入基准参数
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool InsertArgumentBase(ArgumentBase arg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_finservice_argument_base (`name`,`value`,`type`,`serviceplan_fk`,`argclass`) VALUES ( '{0}','{1}','{2}','{3}','{4}')", arg.Name, arg.Value, arg.Type, arg.serviceplan_fk, arg.ArgClass);
                int row = db.Connection.Execute(query);
                return row > 0;
            }
        }

        /// <summary>
        /// 获得代理参数
        /// </summary>
        /// <returns></returns>
        public static ArgumentAgent[] SelectArgumentAgent()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_finservice_argument_agent";
                return db.Connection.Query<ArgumentAgent>(query, null).ToArray();
            }
        }


        /// <summary>
        /// 获得帐户参数
        /// </summary>
        /// <returns></returns>
        public static ArgumentAccount[] SelectArgumentAccount()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_finservice_argument_account";
                return db.Connection.Query<ArgumentAccount>(query, null).ToArray();
            }
        }



    }
}
