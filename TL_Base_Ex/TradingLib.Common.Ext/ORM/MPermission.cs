using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;
using System.Reflection;



namespace TradingLib.ORM
{
    public class MPermission:MBase
    {
        /// <summary>
        /// 从数据库加载所有界面权限
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Permission> SelectUIAccess()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM cfg_permission_template";
                return db.Connection.Query<Permission>(query);
            }
        }


        /// <summary>
        /// 获得更新字符串
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        static string GetUpdateString(Permission access)
        {
            string query = "UPDATE cfg_permission_template SET ";
            PropertyInfo[] propertyInfos = typeof(Permission).GetProperties();
            for (int i = 0; i < propertyInfos.Length;i++)
            {
                PropertyInfo pi = propertyInfos[i];
                if (pi.Name.Equals("id"))
                    continue;
                if (pi.Name.Equals("manager_id"))
                    continue;
                if (pi.Name.Equals("name"))
                {
                    query += pi.Name + "='" + pi.GetValue(access, null).ToString() + ((i != propertyInfos.Length - 1) ? "'," : "");
                    continue;
                }
                if (pi.Name.Equals("domain_id"))
                {
                    query += pi.Name + "='" + pi.GetValue(access, null).ToString() + ((i != propertyInfos.Length - 1) ? "'," : "");
                    continue;
                }
                if (pi.Name.Equals("desp"))
                {
                    query += pi.Name + "='" + pi.GetValue(access, null).ToString() + ((i != propertyInfos.Length - 1) ? "'," : "");
                    continue;
                }
                query += pi.Name + "=" + (((bool)pi.GetValue(access, null)) ? 1 : 0).ToString()+ ((i!=propertyInfos.Length-1)?",":"");//不是最后一个属性需要加逗号分开 Desp Name ID需要放到前面
            }

            query = query + " WHERE id=" + ((int)typeof(Permission).GetProperty("id").GetValue(access, null)).ToString();
            return query;
        }

        /// <summary>
        /// 获得插入字符串
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        static string GetInsertString(Permission access)
        {
            string query = "INSERT INTO cfg_permission_template (";
            PropertyInfo[] propertyInfos = typeof(Permission).GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo pi = propertyInfos[i];
                if (pi.Name.Equals("id"))
                    continue;
                query += "`"+pi.Name+"`" + ((i!=propertyInfos.Length-1)?",":") VALUES(");//不是最后一个属性需要加逗号分开 Desp Name ID需要放到前面
            }
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo pi = propertyInfos[i];
                if (pi.Name.Equals("id"))
                {
                    continue;
                }
                if (pi.Name.Equals("name"))
                {
                    query += "'"+pi.GetValue(access, null).ToString()+"'" + ((i != propertyInfos.Length - 1) ? "," : ")");
                    continue;
                }
                if (pi.Name.Equals("domain_id"))
                {
                    query += "'" + pi.GetValue(access, null).ToString() + "'" + ((i != propertyInfos.Length - 1) ? "," : ")");
                    continue;
                }
                if (pi.Name.Equals("manager_id"))
                {
                    query += "'" + pi.GetValue(access, null).ToString() + "'" + ((i != propertyInfos.Length - 1) ? "," : ")");
                    continue;
                }
                if (pi.Name.Equals("desp"))
                {
                    query += "'" + pi.GetValue(access, null).ToString() + "'" + ((i != propertyInfos.Length - 1) ? "," : ")");
                    continue;
                }

                query += (((bool)pi.GetValue(access, null)) ? 1 : 0).ToString()+ ((i!=propertyInfos.Length-1)?",":")");//不是最后一个属性需要加逗号分开 Desp Name ID需要放到前面
            }

            return query;
        }

        /// <summary>
        /// 插入权限模板
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        public static bool InsertPermissionTemplate(Permission access)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = GetInsertString(access);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => access.id = id, "id", "cfg_permission_template");
                return row > 0;
            }
        }

        /// <summary>
        /// 更新权限模板
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        public static bool UpdatePermissionTemplate(Permission access)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = GetUpdateString(access);
                return db.Connection.Execute(query)>= 0;
            }
        }

        /// <summary>
        /// 删除权限模板
        /// </summary>
        /// <param name="template_id"></param>
        public static void DeletePermissionTemplate(int template_id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM cfg_permission_template WHERE id={0}", template_id);
                db.Connection.Execute(query);
            }
        }

    }
}
