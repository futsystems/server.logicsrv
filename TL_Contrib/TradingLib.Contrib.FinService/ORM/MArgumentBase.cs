using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Mixins.DataBase;


namespace TradingLib.Contrib.FinService.ORM
{

    internal class ArgumentCount
    {
        public int ArgCount { get; set; }
    }

    public class MArgumentBase:MBase
    {



        /// <summary>
        /// 获得基准参数
        /// </summary>
        /// <returns></returns>
        public static ArgumentBase[] SelectArgumentBase()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_finservice_argument_base";
                return  db.Connection.Query<ArgumentBase>(query, null).ToArray();
            }
        }

        /// <summary>
        /// 插入基准参数
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool InsertArgumentBase(ArgumentBase arg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_finservice_argument_base (`name`,`value`,`type`,`serviceplan_fk`,`argclass`) VALUES ( '{0}','{1}','{2}','{3}','{4}')", arg.Name, arg.Value, arg.Type, arg.serviceplan_fk, arg.ArgClass);
                int row = db.Connection.Execute(query);
                return row > 0;
            }
        }

        /// <summary>
        /// 获得代理参数
        /// </summary>
        /// <returns></returns>
        public static ArgumentAgent[] SelectArgumentAgent()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_finservice_argument_agent";
                return db.Connection.Query<ArgumentAgent>(query, null).ToArray();
            }
        }


        /// <summary>
        /// 获得帐户参数
        /// </summary>
        /// <returns></returns>
        public static ArgumentAccount[] SelectArgumentAccount()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_finservice_argument_account";
                return db.Connection.Query<ArgumentAccount>(query, null).ToArray();
            }
        }
        /// <summary>
        /// 查询数据库是否存在参数arg
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool HaveArgumentAccount(ArgumentAccount arg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT count(*) as  ArgCount FROM contrib_finservice_argument_account WHERE service_fk='{0}' AND name='{1}'", arg.service_fk,arg.Name);
                ArgumentCount num = db.Connection.Query<ArgumentCount>(query).SingleOrDefault();
                if (num != null && num.ArgCount > 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 如果arg存在则更新 如果不存在则插入新记录
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool UpdateArgumentAccount(ArgumentAccount arg)
        {
            using (DBMySql db = new DBMySql())
            {
                if (HaveArgumentAccount(arg))//更新
                {
                    string query = String.Format("UPDATE contrib_finservice_argument_account SET value = '{0}' WHERE service_fk = '{1}' AND name='{2}'", arg.Value,arg.service_fk,arg.Name);
                    return db.Connection.Execute(query) >= 0;
                }
                else//插入
                {
                    string query = string.Format("INSERT INTO contrib_finservice_argument_account (`name`,`value`,`type`,`service_fk`) VALUES ( '{0}','{1}','{2}','{3}')", arg.Name, arg.Value, arg.Type, arg.service_fk);
                    return db.Connection.Execute(query) >= 0;
                }
            }
        }

        /// <summary>
        /// 判断数据库中是否存在某个代理的服务计划参数
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool HaveArgumentAgent(ArgumentAgent arg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT count(*) as  ArgCount FROM contrib_finservice_argument_agent WHERE agent_fk='{0}' AND serviceplan_fk='{0}' AND name='{1}'", arg.agent_fk, arg.serviceplan_fk, arg.Name);
                ArgumentCount num = db.Connection.Query<ArgumentCount>(query).SingleOrDefault();
                if (num != null && num.ArgCount > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public static bool UpdateArgumentAgent(ArgumentAgent arg)
        {
            using (DBMySql db = new DBMySql())
            {
                if (HaveArgumentAgent(arg))//更新
                {
                    string query = String.Format("UPDATE contrib_finservice_argument_agent SET value = '{0}' WHERE serviceplan_fk = '{1}' AND name='{2}' AND agent_fk='{3}'", arg.Value, arg.serviceplan_fk, arg.Name,arg.agent_fk);
                    return db.Connection.Execute(query) >= 0;
                }
                else//插入
                {
                    string query = string.Format("INSERT INTO contrib_finservice_argument_agent (`name`,`value`,`type`,`serviceplan_fk`,`agent_fk`) VALUES ( '{0}','{1}','{2}','{3}','{4}')", arg.Name, arg.Value, arg.Type, arg.serviceplan_fk,arg.agent_fk);
                    return db.Connection.Execute(query) >= 0;
                }
            }
        
        }
    }
}
