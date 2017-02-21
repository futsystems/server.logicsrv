using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;
using TradingLib.Contrib.APIService;


namespace TradingLib.ORM
{
    public class MGateWayConfig : MBase
    {

        /// <summary>
        /// 插入支付网关配置
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static void InsertGateWayConfig(GateWayConfig config)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_cash_payment_gateway (`domain_id`,`gatewaytype`,`config`,`avabile`) VALUES ( '{0}','{1}','{2}','{3}')", config.Domain_ID, config.GateWayType, config.Config,config.Avabile?1:0);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => config.ID = id, "id", "contrib_cash_payment_gateway");
            }
        }

        /// <summary>
        /// 更新网关配置
        /// </summary>
        /// <param name="config"></param>
        public static void UpdateGateWayConfig(GateWayConfig config)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE contrib_cash_payment_gateway SET gatewaytype='{0}',config='{1}',avabile='{2}' WHERE id='{3}'", config.GateWayType, config.Config, config.Avabile ? 1 : 0, config.ID);
                db.Connection.Execute(query);
                
            }
        }

        /// <summary>
        /// 获取所有网关配置
        /// </summary>
        /// <param name="transid"></param>
        /// <returns></returns>
        public static IEnumerable<GateWayConfig> SelectGateWayConfig()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM contrib_cash_payment_gateway";
                return db.Connection.Query<GateWayConfig>(query);

            }
        }

        public static GateWayConfig SelectGateWayConfig(int domain_id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM contrib_cash_payment_gateway  WHERE domain_id ='{0}'", domain_id);
                return db.Connection.Query<GateWayConfig>(query).FirstOrDefault();

            }
        }
    }
}
