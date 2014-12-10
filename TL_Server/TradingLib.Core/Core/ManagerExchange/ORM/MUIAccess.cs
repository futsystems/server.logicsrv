//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using TradingLib.Common;
//using TradingLib.Mixins.DataBase;
//using TradingLib.Mixins.JsonObject;
//using System.Reflection;



//namespace TradingLib.ORM
//{
//    /// <summary>
//    /// 管理员到UIAccess的map
//    /// </summary>
//    internal class Manager2UIACcess
//    {
//        public int manager_id { get; set; }
//        public int access_id { get; set; }

//    }

//    public class MUIAccess:MBase
//    {

       
//        /// <summary>
//        /// 从数据库加载所有界面权限
//        /// </summary>
//        /// <returns></returns>
//        public static IEnumerable<UIAccess> SelectUIAccess()
//        {
//            using (DBMySql db = new DBMySql())
//            {
//                string query = "SELECT * FROM manager_ui_access";
//               return db.Connection.Query<UIAccess>(query);
//            }
//        }

//        /// <summary>
//        /// 加载所有帐户的界面访问权限映射
//        /// </summary>
//        /// <returns></returns>
//        internal static IEnumerable<Manager2UIACcess> SelectManager2UIAccess()
//        {
//            using (DBMySql db = new DBMySql())
//            {
//                string query = "SELECT * FROM manager_ui_manager_to_access";
//                return db.Connection.Query<Manager2UIACcess>(query);
//            }
//        }

//        //UPDATE manager_bankac SET bank_id = '{0}' ,name = '{1}',bank_ac = '{2}',b

//        static string GetUpdateString(UIAccess access)
//        {
//            string query = "UPDATE manager_ui_access SET ";
//            PropertyInfo[] propertyInfos = typeof(UIAccess).GetProperties();
//            for (int i = 0; i < propertyInfos.Length;i++)
//            {
//                PropertyInfo pi = propertyInfos[i];
//                if (pi.Name.Equals("id"))
//                    continue;
//                if (pi.Name.Equals("name"))
//                {
//                    query += pi.Name + "='" + pi.GetValue(access, null).ToString() + ((i != propertyInfos.Length - 1) ? "'," : "");
//                    continue;
//                }
//                if (pi.Name.Equals("desp"))
//                {
//                    query += pi.Name + "='" + pi.GetValue(access, null).ToString() + ((i != propertyInfos.Length - 1) ? "'," : "");
//                    continue;
//                }
//                query += pi.Name + "=" + (((bool)pi.GetValue(access, null)) ? 1 : 0).ToString()+ ((i!=propertyInfos.Length-1)?",":"");//不是最后一个属性需要加逗号分开 Desp Name ID需要放到前面
//            }

//            query =query + " WHERE id="+((int)typeof(UIAccess).GetProperty("id").GetValue(access,null)).ToString();
//            return query;
//        }

//        static string GetInsertString(UIAccess access)
//        {
//            string query = "INSERT INTO manager_ui_access (";
//            PropertyInfo[] propertyInfos = typeof(UIAccess).GetProperties();
//            for (int i = 0; i < propertyInfos.Length; i++)
//            {
//                PropertyInfo pi = propertyInfos[i];
//                if (pi.Name.Equals("id"))
//                    continue;
//                query += "`"+pi.Name+"`" + ((i!=propertyInfos.Length-1)?",":") VALUES(");//不是最后一个属性需要加逗号分开 Desp Name ID需要放到前面
//            }
//            for (int i = 0; i < propertyInfos.Length; i++)
//            {
//                PropertyInfo pi = propertyInfos[i];
//                if (pi.Name.Equals("id"))
//                {
//                    continue;
//                }
//                if (pi.Name.Equals("name"))
//                {
//                    query += "'"+pi.GetValue(access, null).ToString()+"'" + ((i != propertyInfos.Length - 1) ? "," : ")");
//                    continue;
//                }
//                if (pi.Name.Equals("desp"))
//                {
//                    query += "'" + pi.GetValue(access, null).ToString() + "'" + ((i != propertyInfos.Length - 1) ? "," : ")");
//                    continue;
//                }

//                query += (((bool)pi.GetValue(access, null)) ? 1 : 0).ToString()+ ((i!=propertyInfos.Length-1)?",":")");//不是最后一个属性需要加逗号分开 Desp Name ID需要放到前面
//            }

//            return query;
//        }

//        public static bool InsertUIAccess(UIAccess access)
//        {
//            using (DBMySql db = new DBMySql())
//            {
//                string query = GetInsertString(access);
//                Util.Debug("insert string:" + query);
//                int row = db.Connection.Execute(query);
//                SetIdentity(db.Connection, id => access.id = id, "id", "manager_ui_access");
//                return row > 0;
//            }
//        }
//        public static bool UpdateUIAccess(UIAccess access)
//        {
//            using (DBMySql db = new DBMySql())
//            {
//                string query = GetUpdateString(access);
//                Util.Debug("update string:" + query);
//                return db.Connection.Execute(query)>= 0;
//            }
//        }



//        /// <summary>
//        /// 更新代理的权限模板
//        /// </summary>
//        /// <param name="managerid"></param>
//        /// <param name="accessid"></param>
//        /// <returns></returns>
//        public static bool UpdateManagerPermissionSet(int managerid, int accessid)
//        {
//            using (DBMySql db = new DBMySql())
//            {
//                string query = string.Format("UPDATE manager_ui_manager_to_access SET access_id ='{0}' WHERE manager_id='{1}'", accessid, managerid);
//                int row = db.Connection.Execute(query);
//                return row > 0;
//            }
//        }

//        /// <summary>
//        /// 插入代理权限设置
//        /// </summary>
//        /// <param name="managerid"></param>
//        /// <param name="accessid"></param>
//        /// <returns></returns>
//        public static bool InsertManagerPermissionSet(int managerid, int accessid)
//        {
//            using (DBMySql db = new DBMySql())
//            {
//                string query = string.Format("INSERT INTO manager_ui_manager_to_access (manager_id,access_id) VALUES ('{0}','{1}')", managerid, accessid);
//                int row = db.Connection.Execute(query);
//                return row > 0;
//            }
//        }






//    }
//}
