using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MManager:MBase
    {


        internal class ManagerAuth
        {
            public string Login { get; set; }
            public string Pass { get; set; }
        }
        /// <summary>
        /// 验证管理员帐号
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static bool ValidManager(string login, string pass)
        {
            using (DBMySql db = new DBMySql())
            {
                try
                {
                    string query = String.Format("SELECT a.login,a.pass FROM accounts_manager a WHERE login = '{0}'", login);
                    ManagerAuth auth = db.Connection.Query<ManagerAuth>(query, null).Single<ManagerAuth>();
                    return auth.Pass.Equals(pass);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 更新管理员密码
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static bool UpdateManagerPass(int id, string pass)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts_manager SET pass = '{0}' WHERE id = '{1}'", pass, id);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 更新管理员类别
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool UpdateManagerType(int id, QSEnumManagerType type)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts_manager SET type = '{0}' WHERE id = '{1}'", type.ToString(), id);
                return db.Connection.Execute(query) >= 0;
            }
        }
        /// <summary>
        /// 获得所有Manager
        /// </summary>
        /// <returns></returns>
        public static IList<Manager> SelectManager()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM accounts_manager");
                IList<Manager> mgrlsit = db.Connection.Query<Manager>(query, null).ToList<Manager>();
                return mgrlsit;
            }
            
        }


    }
}
