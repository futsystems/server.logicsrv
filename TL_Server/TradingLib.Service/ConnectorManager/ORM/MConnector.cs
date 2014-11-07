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
        public static IEnumerable<ConnectorInterface> SelectBrokerInterface()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM connector_broker_interface";
                return db.Connection.Query<ConnectorInterface>(query);
            }
        }

        public static IEnumerable<ConnectorConfig> SelectBrokerConfig()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM connector_broker_config";
                return db.Connection.Query<ConnectorConfig>(query);
            }
        }
    }
}
