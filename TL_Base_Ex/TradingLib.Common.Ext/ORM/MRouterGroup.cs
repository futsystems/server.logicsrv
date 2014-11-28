using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;
using TradingLib.Mixins.JsonObject;
using System.Reflection;


namespace TradingLib.ORM
{
    public class MRouterGroup:MBase
    {
        public static IEnumerable<VendorImpl> SelectVendor()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM vendors";
                return db.Connection.Query<VendorImpl>(query);
            }
        }
        /// <summary>
        /// 获得所有RouterGroup
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RouterGroupImpl> SelectRouterGroup()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM router_group";
                return db.Connection.Query<RouterGroupImpl>(query);
            }
        }

        /// <summary>
        /// 从数据库加载所有路由映射关系
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RouterItemImpl> SelectRouterItem()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM router_items";
                return db.Connection.Query<RouterItemImpl>(query);
            }
        }
    }

    
}
