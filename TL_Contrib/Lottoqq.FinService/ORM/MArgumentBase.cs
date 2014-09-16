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
