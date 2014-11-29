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
        /// 插入新的帐户
        /// </summary>
        /// <param name="vendor"></param>
        public static void InsertVendor(VendorImpl vendor)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into vendors (`name`,`futcompany`,`lastequity`,`description`,`marginlimit`,`settledatetime`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')", vendor.Name, vendor.FutCompany, vendor.LastEquity, vendor.Description, vendor.MarginLimit, Util.ToTLDateTime(DateTime.Now - new TimeSpan(1, 0, 0, 0, 0)));
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => vendor.ID = id, "id", "vendors");
            }
        }

        public static void UpdateVendor(VendorImpl vendor)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE vendors SET futcompany='{0}',lastequity='{1}',description='{2}',marginlimit='{3}' WHERE id='{4}'",vendor.FutCompany,vendor.LastEquity,vendor.Description,vendor.MarginLimit,vendor.ID);
                db.Connection.Execute(query);
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
