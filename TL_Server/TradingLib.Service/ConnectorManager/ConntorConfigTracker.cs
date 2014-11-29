using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.ServiceManager
{
    public class ConnectorConfigTracker
    {

        Dictionary<int, ConnectorInterface> interfacemap = new Dictionary<int, ConnectorInterface>();
        Dictionary<string, ConnectorConfig> configmap = new Dictionary<string, ConnectorConfig>();
        Dictionary<int, ConnectorConfig> configidxmap = new Dictionary<int, ConnectorConfig>();

        static ConnectorConfigTracker _defaultinstance = null;
        static ConnectorConfigTracker()
        {
            _defaultinstance = new ConnectorConfigTracker();
        }

        public void UpdateInterface(ConnectorInterface itface)
        {
            ConnectorInterface target = null;
            if (interfacemap.TryGetValue(itface.ID, out target))
            {
                //更新
                target.IsXAPI = itface.IsXAPI;
                target.libname_broker = itface.libname_broker;
                target.libname_wrapper = itface.libname_wrapper;
                target.libpath_broker = itface.libpath_broker;
                target.libpath_wrapper = itface.libpath_wrapper;
                target.Name = itface.Name;
                target.Type = itface.Type;
                target.type_name = itface.type_name;

            }
            else
            { 
            
            }
        }

        private ConnectorConfigTracker()
        {
            IEnumerable<ConnectorInterface> interfacelist = ORM.MConnector.SelectBrokerInterface();
            foreach (ConnectorInterface itface in interfacelist)
            {
                interfacemap.Add(itface.ID, itface);
            }

            IEnumerable<ConnectorConfig> confglist = ORM.MConnector.SelectBrokerConfig();
            foreach (ConnectorConfig cfg in confglist)
            {
                int itfaceidx = cfg.interface_fk;
                ConnectorInterface itface = getBrokerInterface(itfaceidx);
                if (itface == null)
                    continue;
                itface.IsValid = false;//默认设置为false,需要后段程序通过验证加载成功然后再设置成True
                cfg.Interface = itface;
                configidxmap.Add(cfg.ID, cfg);
                configmap.Add(cfg.Token, cfg);
            }
        }

        ConnectorInterface getBrokerInterface(int id)
        {
            ConnectorInterface itface = null;
            if (interfacemap.TryGetValue(id, out itface))
            {
                return itface;
            }
            return null;
        }
        /// <summary>
        /// 通过ID查找获得对应的成交接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ConnectorInterface GetBrokerInterface(int id)
        {
            ConnectorInterface itface = null;
            if (_defaultinstance.interfacemap.TryGetValue(id, out itface))
            {
                return itface;
            }
            return null;
        }

        ConnectorConfig getBrokerConfig(int id)
        {
            ConnectorConfig cfg = null;
            if (configidxmap.TryGetValue(id, out cfg))
            {
                return cfg;
            }
            return null;
            
        }
        public static ConnectorConfig GetBrokerConfig(int id)
        {
            return _defaultinstance.getBrokerConfig(id);
        }

        ConnectorConfig getBrokerConfig(string token)
        {
            ConnectorConfig cfg = null;
            if (configmap.TryGetValue(token, out cfg))
            {
                return cfg;
            }
            return null;
        }
        public static ConnectorConfig GetBrokerConfig(string token)
        {
            return _defaultinstance.getBrokerConfig(token);
        }

        /// <summary>
        /// 返回所有交易接口设置
        /// </summary>
        public static IEnumerable<ConnectorConfig> BrokerConfigs
        {
            get
            {
                return _defaultinstance.configmap.Values.Where(cfg=>cfg.Interface !=null).Where(cfg=>cfg.Interface.Type == QSEnumConnectorType.Broker);
            }
        }

        public static IEnumerable<ConnectorConfig> DataFeedConfigs
        {
            get
            {
                return _defaultinstance.configmap.Values.Where(cfg => cfg.Interface != null).Where(cfg => cfg.Interface.Type == QSEnumConnectorType.DataFeed);
            }
        }

        public static IEnumerable<ConnectorConfig> ConnecotrConfigs
        {
            get
            {
                return _defaultinstance.configidxmap.Values;
            }
        }
        /// <summary>
        /// 返回所有接口
        /// </summary>
        public static IEnumerable<ConnectorInterface> BrokerInterfaces
        {
            get
            {
                return _defaultinstance.interfacemap.Values.Where(itf => itf.Type == QSEnumConnectorType.Broker);
            }
        }

        public static IEnumerable<ConnectorInterface> DataFeedInterfaces
        {
            get
            {
                return _defaultinstance.interfacemap.Values.Where(itf => itf.Type == QSEnumConnectorType.DataFeed);
            }
        }

        /// <summary>
        /// 获得所有接口设置
        /// </summary>
        public static IEnumerable<ConnectorInterface> Interfaces
        {
            get
            {
                return _defaultinstance.interfacemap.Values;
            }
        }


        #region 添加或更新通道
        public static void UpdateConnectorConfig(ConnectorConfig cfg)
        {
            ConnectorConfig target = null;
            //存在该ID则更新
            if (_defaultinstance.configidxmap.TryGetValue(cfg.ID,out target))
            {
                target.Name = cfg.Name;
                target.srvinfo_ipaddress = cfg.srvinfo_ipaddress;
                target.srvinfo_port = cfg.srvinfo_port;
                target.srvinfo_field1 = cfg.srvinfo_field1;
                target.srvinfo_field2 = cfg.srvinfo_field2;
                target.srvinfo_field3 = cfg.srvinfo_field3;

                target.usrinfo_userid = cfg.usrinfo_userid;
                target.usrinfo_password = cfg.usrinfo_password;
                target.usrinfo_field1 = cfg.usrinfo_field1;
                target.usrinfo_field2 = cfg.usrinfo_field2;
                target.Name = cfg.Name;
                ORM.MConnector.UpdateConnectorConfig(target);
            }
            //添加
            else
            {

                target = new ConnectorConfig();
                target.srvinfo_ipaddress = cfg.srvinfo_ipaddress;
                target.srvinfo_port = cfg.srvinfo_port;
                target.srvinfo_field1 = cfg.srvinfo_field1;
                target.srvinfo_field2 = cfg.srvinfo_field2;
                target.srvinfo_field3 = cfg.srvinfo_field3;
                target.usrinfo_userid = cfg.usrinfo_userid;
                target.usrinfo_password = cfg.usrinfo_password;

                target.usrinfo_field1 = cfg.usrinfo_field1;
                target.usrinfo_field2 = cfg.usrinfo_field2;

                target.Name = cfg.Name;

                target.interface_fk = cfg.interface_fk;
                target.Token = cfg.Token;
                ORM.MConnector.InsertConnectorConfig(target);

                int itfaceidx = cfg.interface_fk;
                ConnectorInterface itface = _defaultinstance.getBrokerInterface(itfaceidx);
                if (itface == null)
                    return;
                //itface.IsValid = false;//默认设置为false,需要后段程序通过验证加载成功然后再设置成True
                cfg.Interface = itface;
                _defaultinstance.configidxmap.Add(target.ID, target);
                _defaultinstance.configmap.Add(target.Token, target);
                

            }
            
        }

        #endregion

    }
}
