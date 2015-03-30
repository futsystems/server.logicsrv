using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Mixins.DataBase;

namespace TradingLib.Contrib.MiniService.ORM
{
    public class MMiniService:MBase
    {
        /// <summary>
        /// 获得所有的迷你服务
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MiniServiceSetting> SelectMiniService()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_miniservice_service";
                return db.Connection.Query<MiniServiceSetting>(query, null).ToArray();
            }
        }


        /// <summary>
        /// 添加迷你服务
        /// </summary>
        /// <param name="account"></param>
        /// <param name="sp_fk"></param>
        /// <returns></returns>
        public static void InsertMiniService(MiniServiceSetting ms)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_miniservice_service (`account`,`active`,) VALUES ( '{0}','{1}')",ms.Account,ms.Active?1:0);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => ms.ID = id, "id", "contrib_miniservice_service");
            }
        }

        public static void UpdateMiniService(MiniServiceSetting ms)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE  contrib_miniservice_service SET active='{0}' WHERE id='{1}'", ms.Active ? 1 : 0, ms.ID);
                db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 删除迷你服务
        /// </summary>
        /// <param name="stub"></param>
        /// <returns></returns>
        public static void DeleteMiniService(MiniServiceSetting ms)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM contrib_miniservice_service WHERE id = '{0}'", ms.ID);
                db.Connection.Execute(query);
            }
        }


    }
}
