using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    /// <summary>
    /// 手续费数据库操作
    /// </summary>
    public class MCommission:MBase
    {
        

        /// <summary>
        /// 更新手续费模板
        /// </summary>
        /// <param name="t"></param>
        public static void UpdateCommissionTemplate(CommissionTemplate t)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE cfg_commission_template SET name='{0}',description='{1}' WHERE id='{2}'", t.Name, t.Description, t.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 插入一条手续费模板
        /// </summary>
        /// <param name="t"></param>
        public static void InsertCommissionTemplate(CommissionTemplate t)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO cfg_commission_template (`name`,`description`,`domain_id`) VALUES ( '{0}','{1}','{2}')",t.Name,t.Description,t.Domain_ID);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => t.ID = id, "id", "cfg_commission_template");
            }
        }

        /// <summary>
        /// 获得所有手续费模板
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CommissionTemplate> SelectCommissionTemplates()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM cfg_commission_template";
                return db.Connection.Query<CommissionTemplate>(query);
            }
        }

        /// <summary>
        /// 更新手续费项目
        /// </summary>
        /// <param name="item"></param>
        public static void UpdateCommissionTemplateItem(CommissionTemplateItem item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE cfg_commission SET openbymoney='{0}',openbyvolume='{1}',closetodaybymoney='{2}',closetodaybyvolume='{3}',closebymoney='{4}',closebyvolume='{5}',chargetype='{6}' WHERE id='{7}'", item.OpenByMoney, item.OpenByVolume, item.CloseTodayByMoney, item.CloseTodayByVolume, item.CloseByMoney, item.CloseByVolume, item.ChargeType, item.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 插入手续费模板项目
        /// </summary>
        /// <param name="item"></param>
        public static void InsertCommissionTemplateItem(CommissionTemplateItem item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO cfg_commission (`code`,`month`,`openbymoney`,`openbyvolume`,`closetodaybymoney`,`closetodaybyvolume`,`closebymoney`,`closebyvolume`,`chargetype`,`template_id`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", item.Code,item.Month,item.OpenByMoney,item.OpenByVolume,item.CloseTodayByMoney,item.CloseTodayByVolume,item.CloseByMoney,item.CloseByVolume,item.ChargeType,item.Template_ID);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => item.ID = id, "id", "cfg_commission");
            }
        }

        /// <summary>
        /// 获得所有手续费模板项目
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CommissionTemplateItem> SelectCommissionTemplateItems()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM cfg_commission";
                return db.Connection.Query<CommissionTemplateItem>(query);
            }
        }

    }
}
