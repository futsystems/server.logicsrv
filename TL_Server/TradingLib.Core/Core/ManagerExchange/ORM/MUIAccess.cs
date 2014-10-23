using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.ORM
{
    /// <summary>
    /// 管理员到UIAccess的map
    /// </summary>
    internal class Manager2UIACcess
    {
        public int manager_id { get; set; }
        public int access_id { get; set; }

    }

    public class MUIAccess:MBase
    {

       
        /// <summary>
        /// 从数据库加载所有界面权限
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<UIAccess> SelectUIAccess()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM manager_ui_access";
               return db.Connection.Query<UIAccess>(query);
            }
        }

        /// <summary>
        /// 加载所有帐户的界面访问权限映射
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Manager2UIACcess> SelectManager2UIAccess()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM manager_ui_access_to_manager";
                return db.Connection.Query<Manager2UIACcess>(query);
            }
        }





    }
}
