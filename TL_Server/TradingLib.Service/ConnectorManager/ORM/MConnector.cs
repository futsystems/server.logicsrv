using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    
    public class MConnector:MBase
    {
        public static IEnumerable<BrokerInterface> SelectBrokerInterface()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM connector_broker_interface";
                return db.Connection.Query<BrokerInterface>(query);
            }
        }

        public static IEnumerable<BrokerConfig> SelectBrokerConfig()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM connector_broker_config";
                return db.Connection.Query<BrokerConfig>(query);
            }
        }
    }
}
