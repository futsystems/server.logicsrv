using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{

    public class MConfigTemplate: MBase
    {
        /// <summary>
        /// 获得所有配置模板
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ConfigTemplate> SelectConfigTemplates()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM cfg_config_template";
                return db.Connection.Query<ConfigTemplate>(query);
            }
        }

         /// <summary>
        /// 更新配置模板
        /// </summary>
        /// <param name="item"></param>
        public static void UpdateConfigTemplate(ConfigTemplate item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE cfg_config_template SET description='{0}',commission_id='{1}',margin_id='{2}',strategy_id='{3}'  WHERE id='{4}'", item.Description, item.Commission_ID, item.ExStrategy_ID, item.Margin_ID,item.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 插入配置模板
        /// </summary>
        /// <param name="item"></param>
        public static void InsertConfigTemplate(ConfigTemplate item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO cfg_config_template (`domain_id`,`manager_id`,`name`,`description`,`commission_id`,`margin_id`,`strategy_id`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}')", item.Domain_ID, item.Manager_ID, item.Name, item.Description, item.Commission_ID, item.Margin_ID, item.ExStrategy_ID);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => item.ID = id, "id", "cfg_config_template");
            }
        }

        /// <summary>
        /// 删除配置模板
        /// 珊瑚模板的同时将模板设定的风控规则也删除
        /// </summary>
        /// <param name="template_id"></param>
        public static void DeleteConfigTemplate(int template_id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM cfg_config_template WHERE id={0}", template_id);
                db.Connection.Execute(query);

                //query = string.Format("DELETE FROM cfg_rule WHERE account={0}", string.Format("CFG_TEMPLATE_{0}",template_id));
                //db.Connection.Execute(query);
            }
        }
    }
}
