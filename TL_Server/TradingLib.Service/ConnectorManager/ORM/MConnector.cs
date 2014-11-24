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

        /// <summary>
        /// 更新接口参数
        /// </summary>
        /// <param name="itface"></param>
        public static void UpdateConnectorInterface(ConnectorInterface itface)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE connector_broker_interface SET type_name='{0}',libpath_wrapper='{1}',libname_wrapper='{2}',libpath_broker='{3}',libname_broker='{4}',name='{5}',isxapi='{6}',type='{7}' WHERE id='{8}'",itface.type_name,itface.libpath_wrapper,itface.libname_wrapper,itface.libpath_broker,itface.libname_broker,itface.Name,itface.IsXAPI?1:0,itface.Type,itface.ID);
                db.Connection.Execute(query);
            }
        }
    }
}
