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

        public static void UpdateConnectorConfig(ConnectorConfig cfg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE connector_broker_config SET srvinfo_ipaddress='{0}',srvinfo_port='{1}',srvinfo_field1='{2}',srvinfo_field2='{3}',srvinfo_field3='{4}',usrinfo_userid='{5}',usrinfo_password='{6}',usrinfo_field1='{7}',usrinfo_field2='{8}',name='{9}' WHERE id='{10}'", cfg.srvinfo_ipaddress,cfg.srvinfo_port,cfg.srvinfo_field1,cfg.srvinfo_field2,cfg.srvinfo_field3,cfg.usrinfo_userid,cfg.usrinfo_password,cfg.usrinfo_field1,cfg.usrinfo_field2,cfg.Name,cfg.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新ConnectorConfg的Vendor字段
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="v"></param>
        public static void UpdateConnectorConfigVendor(ConnectorConfig cfg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE connector_broker_config SET vendor_id='{0}' WHERE id='{1}'",cfg.vendor_id,cfg.ID);
                db.Connection.Execute(query);
            }
        }

        public static void InsertConnectorConfig(ConnectorConfig cfg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into connector_broker_config (`srvinfo_ipaddress`,`srvinfo_port`,`srvinfo_field1`,`srvinfo_field2`,`srvinfo_field3`,`usrinfo_userid`,`usrinfo_password`,`usrinfo_field1`,`usrinfo_field2`,`interface_fk`,`token`,`name`,`domain_id`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')", cfg.srvinfo_ipaddress, cfg.srvinfo_port, cfg.srvinfo_field1, cfg.srvinfo_field2, cfg.srvinfo_field3, cfg.usrinfo_userid, cfg.usrinfo_password, cfg.usrinfo_field1, cfg.usrinfo_field2, cfg.interface_fk, cfg.Token, cfg.Name,cfg.Domain.ID);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => cfg.ID = id, "id", "connector_broker_config");
            }
        }
    }
}
