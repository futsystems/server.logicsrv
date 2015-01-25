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
    public class MResponse:MBase
    {
        /// <summary>
        /// 获得所有的配资服务
        /// </summary>
        /// <returns></returns>
        public static ResponseWrapper[] SelectResponse()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_response_instance";
                return db.Connection.Query<ResponseWrapper>(query, null).ToArray();
            }
        }


        /// <summary>
        /// 为某个交易帐户添加一个配资服务
        /// </summary>
        /// <param name="account"></param>
        /// <param name="sp_fk"></param>
        /// <returns></returns>
        public static void InsertResponse(ResponseWrapper resp)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_response_instance (`acct`,`response_templte_id`,`active`) VALUES ( '{0}','{1}','{2}')", resp.Acct, resp.Response_Template_ID, resp.Active ? 1 : 0);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => resp.ID = id, "id", "contrib_response_instance");
            }
        }

        /// <summary>
        /// 删除数据库记录
        /// </summary>
        /// <param name="stub"></param>
        /// <returns></returns>
        public static bool DeleteResponse(ResponseWrapper resp)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM contrib_response_instance WHERE id = '{0}'", resp.ID);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 获得所有策略模板
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ResponseTemplate> SelectResponseTemplate()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_response_template";
                return db.Connection.Query<ResponseTemplate>(query, null).ToArray();
            }
        }

        /// <summary>
        /// 插入策略模板
        /// </summary>
        /// <param name="template"></param>
        public static void InsertResponseTemplate(ResponseTemplate template)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_response_template (`name`,`title`,`classname`,`active`) VALUES ( '{0}','{1}','{2}','{3}')", template.Name, template.Title, template.ClassName, template.Active?1:0);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => template.ID = id, "id", "contrib_response_template");
            }
        }

    }
}
