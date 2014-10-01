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

        public static bool UpdateManager(Manager manager)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts_manager SET login= '{0}' , type = '{1}' , name = '{2}' , mobile = '{3}' , qq = '{4}' , acclimit = '{5}' WHERE id = '{6}'",manager.Login,manager.Type.ToString(),manager.Name,manager.Mobile,manager.QQ,manager.AccLimit,manager.ID);
                return db.Connection.Execute(query) >= 0;
            }
        }

        public static bool InsertManager(Manager manger)
        {
            Util.Debug("add manger,mgrfk:" + manger.mgr_fk.ToString());
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into accounts_manager (`login`,`type`,`name`,`mobile`,`qq`,`acclimit`) values('{0}','{1}','{2}','{3}','{4}','{5}')", manger.Login, manger.Type.ToString(), manger.Name, manger.Mobile, manger.QQ, manger.AccLimit);//orderpostflag`,`forceclose`,`hedgeflag`,`orderref`,`orderexchid`,`orderseq`

                int row = db.Connection.Execute(query);

                SetIdentity(db.Connection, id => manger.ID = id, "id", "accounts_manager");
                //如果是agent代理类别的manager则需要更新其mgr_fk为自己，其余manger的添加 均已添加时设定的mgr_fk为准
                if (manger.Type == QSEnumManagerType.AGENT)
                {
                    manger.mgr_fk = manger.ID;
                }
                query = String.Format("UPDATE accounts_manager SET mgr_fk='{0}' WHERE id='{1}'", manger.mgr_fk, manger.ID);

                row = db.Connection.Execute(query);

                return row > 0;
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
