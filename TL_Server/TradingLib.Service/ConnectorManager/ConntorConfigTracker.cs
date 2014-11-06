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

        Dictionary<int, BrokerInterface> interfacemap = new Dictionary<int, BrokerInterface>();
        Dictionary<string, BrokerConfig> configmap = new Dictionary<string, BrokerConfig>();
        Dictionary<int, BrokerConfig> configidxmap = new Dictionary<int, BrokerConfig>();

        static ConnectorConfigTracker _defaultinstance = null;
        static ConnectorConfigTracker()
        {
            _defaultinstance = new ConnectorConfigTracker();
        }


        private ConnectorConfigTracker()
        {
            IEnumerable<BrokerInterface> interfacelist = ORM.MConnector.SelectBrokerInterface();
            foreach (BrokerInterface itface in interfacelist)
            {
                interfacemap.Add(itface.ID, itface);
            }

            IEnumerable<BrokerConfig> confglist = ORM.MConnector.SelectBrokerConfig();
            foreach (BrokerConfig cfg in confglist)
            {
                int itfaceidx = cfg.interface_fk;
                BrokerInterface itface = getBrokerInterface(itfaceidx);
                if (itface == null)
                    continue;
                itface.IsValid = false;//默认设置为false,需要后段程序通过验证加载成功然后再设置成True
                cfg.Interface = itface;
                configidxmap.Add(cfg.ID, cfg);
                configmap.Add(cfg.Token, cfg);
            }
        }

        BrokerInterface getBrokerInterface(int id)
        {
            BrokerInterface itface = null;
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
        public static BrokerInterface GetBrokerInterface(int id)
        {
            BrokerInterface itface = null;
            if (_defaultinstance.interfacemap.TryGetValue(id, out itface))
            {
                return itface;
            }
            return null;
        }

        BrokerConfig getBrokerConfig(int id)
        {
            BrokerConfig cfg = null;
            if (configidxmap.TryGetValue(id, out cfg))
            {
                return cfg;
            }
            return null;
            
        }
        public static BrokerConfig GetBrokerConfig(int id)
        {
            return _defaultinstance.getBrokerConfig(id);
        }

        BrokerConfig getBrokerConfig(string token)
        {
            BrokerConfig cfg = null;
            if (configmap.TryGetValue(token, out cfg))
            {
                return cfg;
            }
            return null;
        }
        public static BrokerConfig GetBrokerConfig(string token)
        {
            return _defaultinstance.getBrokerConfig(token);
        }

        /// <summary>
        /// 返回所有交易接口设置
        /// </summary>
        public static IEnumerable<BrokerConfig> BrokerConfigs
        {
            get
            {
                return _defaultinstance.configmap.Values;
            }
        }

        /// <summary>
        /// 返回所有接口
        /// </summary>
        public static IEnumerable<BrokerInterface> BrokerInterfaces
        {
            get
            {
                return _defaultinstance.interfacemap.Values;
            }
        }
        
    }
}
