﻿using System;
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
        /// 查询某个Manger的密码
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static string GetManagerPass(string login)
        {
            using (DBMySql db = new DBMySql())
            {
                    string query = String.Format("SELECT a.login,a.pass FROM manager a WHERE login = '{0}'", login);
                    ManagerAuth auth = db.Connection.Query<ManagerAuth>(query, null).Single<ManagerAuth>();
                    return auth.Pass;
            }
        }

        /// <summary>
        /// 数据库中是否已存在管理员登入名
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static bool ExitManagerLogin(string login)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT a.login,a.pass FROM manager a WHERE login = '{0}'", login);
                return db.Connection.Query<ManagerAuth>(query, null).Count() > 0;
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
        /// 更新管理员权限模板
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permission_id"></param>
        public static void UpdateManagerPermission(int id, int permission_id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE manager SET permission_id = '{0}' WHERE id = '{1}'", permission_id, id);
                db.Connection.Execute(query);
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
                string query = String.Format("UPDATE manager SET  acclimit = '{0}',agentlimit = '{1}'  WHERE id = '{2}'",  manager.AccLimit, manager.AgentLimit, manager.ID);
                return db.Connection.Execute(query) >= 0;
            }
        }



        public static bool InsertManager(Manager manger)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into manager (`login`,`type`,`acclimit`,`domain_id`,`active`,`agentlimit`) values('{0}','{1}','{2}','{3}','{4}','{5}')", manger.Login, manger.Type.ToString(), manger.AccLimit, manger.domain_id, manger.Active ? 1 : 0, manger.AgentLimit);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => manger.ID = id, "id", "manager");

                //如果Root/Agent需要更新管理域mgr_fk为自己的全局ID，其余manger在添加时设定了mgr_fk(柜员域)
                if (manger.Type == QSEnumManagerType.AGENT || manger.Type == QSEnumManagerType.ROOT)
                {
                    manger.mgr_fk = manger.ID;
                }
                query = String.Format("UPDATE manager SET mgr_fk='{0}' WHERE id='{1}'", manger.mgr_fk, manger.ID);
                db.Connection.Execute(query);

                //Root没有父域,父域ID与自身域ID一致
                if (manger.Type == QSEnumManagerType.ROOT)
                {
                    manger.parent_fk = manger.ID;
                }
                //更新Parent_fk 注插入时没有写入Parent_fk,因此这个语句不能放在上面的条件内
                query = String.Format("UPDATE manager SET parent_fk='{0}' WHERE id='{1}'", manger.parent_fk, manger.ID);
                db.Connection.Execute(query);
                

                return true;
            }
        }

       
        public static void MarkManagerDeleted(int mgr_id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE manager SET deleted = '1' , deletedtime='{0}', deletedsettleday='{1}' WHERE mgr_fk = '{2}'", Util.ToTLDateTime(), TLCtxHelper.ModuleSettleCentre.Tradingday, mgr_id);
                db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="mgr_id"></param>
        public static void DeleteManager(ManagerSetting mgr)
        {
            using (DBMySql db = new DBMySql())
            {
                string delquery = string.Empty;
                delquery = string.Format("DELETE FROM manager WHERE id = '{0}'", mgr.ID);
                db.Connection.Execute(delquery);

                delquery = string.Format("DELETE FROM manager_profile WHERE account = '{0}'", mgr.Login);
                db.Connection.Execute(delquery);

                delquery = string.Format("DELETE FROM agents WHERE account = '{0}'", mgr.Login);
                db.Connection.Execute(delquery);

                delquery = string.Format("DELETE FROM log_agent_cashtrans WHERE account = '{0}'", mgr.Login);
                db.Connection.Execute(delquery);

                delquery = string.Format("DELETE FROM log_agent_commission_split WHERE account = '{0}'", mgr.Login);
                db.Connection.Execute(delquery);

                delquery = string.Format("DELETE FROM log_agent_settlement WHERE account = '{0}'", mgr.Login);
                db.Connection.Execute(delquery);

                delquery = string.Format("DELETE FROM cfg_permission_template WHERE manager_id = '{0}'", mgr.ID);
                db.Connection.Execute(delquery);

                foreach (var template in ORM.MCommission.SelectCommissionTemplates().Where(item => item.Manager_ID == mgr.ID))
                {
                    //需要删除管理员添加的模板 而不是所有模板
                    //foreach (var v in ORM.MCommission.SelectCommissionTemplates())
                    //{
                    delquery = string.Format("DELETE FROM cfg_commission WHERE template_id = '{0}'", template.ID);
                        db.Connection.Execute(delquery);
                    //}
                }

                delquery = string.Format("DELETE FROM cfg_commission_template WHERE manager_id = '{0}'", mgr.ID);
                db.Connection.Execute(delquery);
                

                foreach(var template in ORM.MMargin.SelectMarginTemplates().Where(item=>item.Manager_ID == mgr.ID))
                //foreach (var v in ORM.MMargin.SelectMarginTemplates())
                {
                    delquery = string.Format("DELETE FROM cfg_margin WHERE template_id = '{0}'", template.ID);
                    db.Connection.Execute(delquery);
                }

                delquery = string.Format("DELETE FROM cfg_margin_template WHERE manager_id = '{0}'", mgr.ID);
                db.Connection.Execute(delquery);

                foreach(var template in ORM.MExStrategy.SelectExStrategyTemplates().Where(item=>item.Manager_ID == mgr.ID))
                //foreach (var v in ORM.MExStrategy.SelectExStrategyTemplates())
                {
                    delquery = string.Format("DELETE FROM cfg_strategy WHERE template_id = '{0}'", template.ID);
                    db.Connection.Execute(delquery);
                }

                delquery = string.Format("DELETE FROM cfg_strategy_template WHERE manager_id = '{0}'", mgr.ID);
                db.Connection.Execute(delquery);

                
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
