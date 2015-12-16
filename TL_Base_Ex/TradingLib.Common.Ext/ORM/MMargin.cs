using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MMargin:MBase
    {
        /// <summary>
        /// 插入保证金模板
        /// </summary>
        /// <param name="t"></param>
        public static void InsertMarginTemplate(MarginTemplate t)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO cfg_margin_template (`name`,`description`,`domain_id`,`manager_id`) VALUES ( '{0}','{1}','{2}','{3}')", t.Name, t.Description, t.Domain_ID,t.Manager_ID);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => t.ID = id, "id", "cfg_margin_template");
            }
        }

        /// <summary>
        /// 更新保证金模板
        /// </summary>
        /// <param name="t"></param>
        public static void UpdateMarginTemplate(MarginTemplate t)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE cfg_margin_template SET name='{0}',description='{1}' WHERE id='{2}'", t.Name, t.Description, t.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 删除某个保证金模板
        /// </summary>
        /// <param name="template_id"></param>
        public static void DeleteMarginTemplate(int template_id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM cfg_margin_template WHERE id={0}", template_id);
                db.Connection.Execute(query);

                query = string.Format("DELETE FROM cfg_margin WHERE template_id={0}", template_id);
                db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 获得所有手续费模板
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MarginTemplate> SelectMarginTemplates()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM cfg_margin_template";
                return db.Connection.Query<MarginTemplate>(query);
            }
        }

        /// <summary>
        /// 插入保证金模板项
        /// </summary>
        /// <param name="item"></param>
        public static void InsertMarginTemplateItem(MarginTemplateItem item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO cfg_margin (`code`,`month`,`marginbymoney`,`marginbyvolume`,`percent`,`chargetype`,`template_id`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}')",item.Code,item.Month,item.MarginByMoney,item.MarginByVolume,item.Percent,item.ChargeType,item.Template_ID);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => item.ID = id, "id", "cfg_margin");
            }
        }

        /// <summary>
        /// 获得所有保证金模板项目
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MarginTemplateItem> SelectMarginTemplateItems()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM cfg_margin";
                return db.Connection.Query<MarginTemplateItem>(query);
            }
        }

        /// <summary>
        /// 更新保证金模板项目
        /// </summary>
        /// <param name="item"></param>
        public static void UpdateMarginTemplateItem(MarginTemplateItem item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE cfg_margin SET marginbymoney='{0}',marginbyvolume='{1}' ,percent='{2}' ,chargetype='{3}' WHERE id='{4}'",item.MarginByMoney,item.MarginByVolume,item.Percent,item.ChargeType,item.ID);
                db.Connection.Execute(query);
            }
        }
    }
}
