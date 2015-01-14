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
    }
}
