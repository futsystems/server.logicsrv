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
        
    }
}
