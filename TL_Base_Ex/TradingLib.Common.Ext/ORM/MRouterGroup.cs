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
                string query = String.Format("Insert into vendors (`name`,`futcompany`,`lastequity`,`description`,`marginlimit`,`settledatetime`,`domain_id`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", vendor.Name, vendor.FutCompany, vendor.LastEquity, vendor.Description, vendor.MarginLimit, Util.ToTLDateTime(DateTime.Now - new TimeSpan(1, 0, 0, 0, 0)),vendor.Domain.ID);
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

        public static void UpdateRouterGroup(RouterGroupImpl group)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE router_group SET strategy='{0}',description='{1}' WHERE id='{2}'", group.Strategy, group.Description,group.ID);
                db.Connection.Execute(query);
            }
        }

        public static void InsertRouterGroup(RouterGroupImpl group)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into router_group (`name`,`strategy`,`description`,`domain_id`) VALUES ('{0}','{1}','{2}','{3}')", group.Name,group.Strategy,group.Description,group.domain_id);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => group.ID = id, "id", "router_group");
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

        /// <summary>
        /// 更新路由项目
        /// </summary>
        /// <param name="item"></param>
        public static void UpdateRouterItem(RouterItemImpl item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE router_items SET priority='{0}',rule='{1}' ,active='{2}'  WHERE id='{3}'", item.priority, item.rule, item.Active?1:0, item.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 插入路由项目
        /// </summary>
        /// <param name="item"></param>
        public static void InsertRouterItem(RouterItemImpl item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into router_items (`routegroup_id`,`vendor_id`,`priority`,`rule`,`active`) VALUES ('{0}','{1}','{2}','{3}','{4}')", item.routegroup_id, item.vendor_id, item.priority, item.rule, item.Active ? 1 : 0);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => item.ID = id, "id", "router_items");
            }
        }


    }

    
}
