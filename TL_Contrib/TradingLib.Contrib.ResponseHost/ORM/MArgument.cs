using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Mixins.DataBase;


namespace TradingLib.Contrib.ResponseHost.ORM
{
    public class MArgument:MBase
    {
        internal class ArgumentCount
        {
            public int ArgCount { get; set; }
        }
        /// <summary>
        /// 获得基准参数
        /// </summary>
        /// <returns></returns>
        public static ArgumentBase[] SelectArgumentBase()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_response_argument_base";
                return db.Connection.Query<ArgumentBase>(query, null).ToArray();
            }
        }

        /// <summary>
        /// 获得策略实例参数
        /// </summary>
        /// <returns></returns>
        public static ArgumentInstance[] SelectArgumentInstance()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_response_argument_instance";
                return db.Connection.Query<ArgumentInstance>(query, null).ToArray();
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
                string query = string.Format("INSERT INTO contrib_response_argument_base (`name`,`value`,`type`,`template_id`) VALUES ( '{0}','{1}','{2}','{3}')", arg.Name, arg.Value, arg.Type, arg.Template_ID);
                int row = db.Connection.Execute(query);
                return row > 0;
            }
        }

        /// <summary>
        /// 判断数据库中是否存在某个策略实例参数
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool HaveArgumentInstance(ArgumentInstance arg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT count(*) as  ArgCount FROM contrib_response_argument_instance WHERE instance_id='{0}' AND name='{1}'",arg.Instance_ID, arg.Name);
                ArgumentCount num = db.Connection.Query<ArgumentCount>(query).SingleOrDefault();
                if (num != null && num.ArgCount > 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 更新策略实例参数
        /// </summary>
        /// <param name="arg"></param>
        public static void UpdateArgumentInstance(ArgumentInstance arg)
        {
            using (DBMySql db = new DBMySql())
            {
                if (HaveArgumentInstance(arg))//更新
                {
                    string query = String.Format("UPDATE contrib_response_argument_instance SET value = '{0}' WHERE instance_id = '{1}' AND name='{2}'", arg.Value, arg.Instance_ID, arg.Name);
                    db.Connection.Execute(query);
                }
                else//插入
                {
                    string query = string.Format("INSERT INTO contrib_response_argument_instance (`name`,`value`,`type`,`instance_id`) VALUES ( '{0}','{1}','{2}','{3}')", arg.Name, arg.Value, arg.Type, arg.Instance_ID);
                    db.Connection.Execute(query);
                }
            }
        }
    }
}
