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
                    string query = String.Format("SELECT a.login,a.pass FROM manager a WHERE login = '{0}'", login);
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
                string query = String.Format("UPDATE manager SET pass = '{0}' WHERE id = '{1}'", pass, id);
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
                string query = String.Format("UPDATE manager SET type = '{0}' WHERE id = '{1}'", type.ToString(), id);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 更新管理员激活或冻结
        /// </summary>
        /// <param name="id"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public static bool UpdateManagerActive(int id,bool active)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE manager SET active = '{0}' WHERE id = '{1}'",active?1:0, id);
                return db.Connection.Execute(query) >= 0;
            }
        }

        public static bool UpdateManager(Manager manager)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE manager SET  name = '{0}' , mobile = '{1}' , qq = '{2}' , acclimit = '{3}' WHERE id = '{4}'",manager.Name,manager.Mobile,manager.QQ,manager.AccLimit,manager.ID);
                return db.Connection.Execute(query) >= 0;
            }
        }



        public static bool InsertManager(Manager manger)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into manager (`login`,`type`,`name`,`mobile`,`qq`,`acclimit`,`domain_id`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", manger.Login, manger.Type.ToString(), manger.Name, manger.Mobile, manger.QQ, manger.AccLimit,manger.domain_id);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => manger.ID = id, "id", "manager");

                //如果Root/Agent需要更新mgr_fk为自己的全局ID，其余manger在添加时设定了mgr_fk(柜员域)
                if (manger.Type == QSEnumManagerType.AGENT || manger.Type == QSEnumManagerType.ROOT)
                {
                    manger.mgr_fk = manger.ID;
                }
                query = String.Format("UPDATE manager SET mgr_fk='{0}' WHERE id='{1}'", manger.mgr_fk, manger.ID);
                db.Connection.Execute(query);


                if (manger.Type == QSEnumManagerType.ROOT)
                {
                    manger.parent_fk = manger.ID;
                }
                query = String.Format("UPDATE manager SET parent_fk='{0}' WHERE id='{1}'", manger.parent_fk, manger.ID);
                db.Connection.Execute(query);

                return true;
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
                string query = string.Format("SELECT * FROM manager");
                IList<Manager> mgrlsit = db.Connection.Query<Manager>(query, null).ToList<Manager>();
                return mgrlsit;
            }

        }



        #region manager的结算 出入金 等财务操作



        #endregion

    }
}
