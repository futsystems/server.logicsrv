using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;
using TradingLib.BrokerXAPI;

namespace TradingLib.ServiceManager
{
    public partial class ConnectorManager
    {

        //接口对象映射表
        Dictionary<string, IBroker> brokerInstList = new Dictionary<string, IBroker>();
        Dictionary<string, IDataFeed> datafeedInstList = new Dictionary<string, IDataFeed>();

        /// <summary>
        /// 验证成交接口设置
        /// </summary>
        void ValidInterface()
        {
            /*验证接口
             * 
             * 数据库设置的接口 需要检查接口的类型 
             * 目前XAPI体系的接口 通过实现一个具体的TLBrokerBase 填充具体的功能操作 同时底层通过XAPI Proxy加载对应的c++dll实现 因此
             * 接口验证需要检查c# plugin是否存在 同时具体的c++ dll是否加载成功 如果失败则不加载对应的BrokerConfig
             **/
            debug("Valid connecotr config(interface and config)", QSEnumDebugLevel.INFO);
            foreach (ConnectorInterface itface in BasicTracker.ConnectorConfigTracker.BrokerInterfaces)
            {
                bool cs_success = false;
                cs_success = xapibrokermodule.Keys.Contains(itface.type_name);
                bool cpp_success = itface.IsXAPI?BrokerXAPIHelper.ValidBrokerInterface(itface):true;
                bool ret = cs_success && cpp_success;
                //通过加载测试
                if (ret)
                    itface.IsValid = true;

                debug(string.Format("Broker Interface[{0}] C# Plugin[{1}]:{2} C++ Dll:{3} Valid:{4}", itface.Name, itface.type_name, cs_success, cpp_success, ret), QSEnumDebugLevel.INFO);
            }
            foreach (ConnectorConfig cfg in BasicTracker.ConnectorConfigTracker.BrokerConfigs)
            {
                if (cfg.Interface == null)
                    continue;
                if (!cfg.Interface.IsValid)
                    continue;
                debug(string.Format("Broker Config[{0}] Name:{1} SrvIP:{2} LoginID{3}", cfg.Token, cfg.Name, cfg.srvinfo_ipaddress, cfg.usrinfo_userid), QSEnumDebugLevel.INFO);
            }

            foreach (ConnectorInterface itface in BasicTracker.ConnectorConfigTracker.DataFeedInterfaces)
            {
                bool cs_success = false;
                cs_success = xapidatafeedmodule.Keys.Contains(itface.type_name);
                bool cpp_success = itface.IsXAPI ? BrokerXAPIHelper.ValidBrokerInterface(itface) : true;
                bool ret = cs_success && cpp_success;
                //通过加载测试
                if (ret)
                    itface.IsValid = true;

                debug(string.Format("DataFeed Interface[{0}] C# Plugin[{1}]:{2} C++ Dll:{3} Valid:{4}", itface.Name, itface.type_name, cs_success, cpp_success, ret), QSEnumDebugLevel.INFO);
            }
            foreach (ConnectorConfig cfg in BasicTracker.ConnectorConfigTracker.DataFeedConfigs)
            {
                if (cfg.Interface == null)
                    continue;
                if (!cfg.Interface.IsValid)
                    continue;
                debug(string.Format("DataFeed Config[{0}] Name:{1} SrvIP:{2} LoginID{3}", cfg.Token, cfg.Name, cfg.srvinfo_ipaddress, cfg.usrinfo_userid), QSEnumDebugLevel.INFO);
            }

        }

        /// <summary>
        /// 从某个接口配置文件创建TLBrokerBase
        /// 注 TLBrokerBase实现了IBroker接口,可以加载到系统 供调用
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        TLBrokerBase CreateBroker(ConnectorConfig cfg)
        {
            if ((cfg.Interface == null) || (!cfg.Interface.IsValid) || (cfg.Interface.Type == QSEnumConnectorType.DataFeed))
            { 
                Util.Debug(string.Format("Broker Config[{0}] is not valid,can not load that",cfg.Token),QSEnumDebugLevel.WARNING);
                return null;
            }
            Type t = xapibrokermodule[cfg.Interface.type_name];
            TLBrokerBase broker = (TLBrokerBase)Activator.CreateInstance(t);
            return broker;
        }

        TLDataFeedBase CreateDataFeed(ConnectorConfig cfg)
        {
            if ((cfg.Interface == null) || (!cfg.Interface.IsValid) || (cfg.Interface.Type == QSEnumConnectorType.Broker))
            {
                Util.Debug(string.Format("DataFeed Config[{0}] is not valid,can not load that", cfg.Token), QSEnumDebugLevel.WARNING);
                return null;
            }
            Type t = xapidatafeedmodule[cfg.Interface.type_name];
            TLDataFeedBase datafeed = (TLDataFeedBase)Activator.CreateInstance(t);
            return datafeed;
        }

        //void BindConnectorWithVendor(ConnectorConfig cfg)
        //{ 
            
        //}
        /// <summary>
        /// 从ConnectorConfig加载Broker通道
        /// </summary>
        /// <param name="cfg"></param>
        void LoadBrokerConnector(ConnectorConfig cfg)
        {
            debug(string.Format("Load broker connector[{0}] name:{1}", cfg.Token, cfg.Name), QSEnumDebugLevel.INFO);
            //1.生成BrokerBase
            TLBrokerBase broker = CreateBroker(cfg);
            if (broker == null)
                return;

            //2.绑定底层事件和设定Broker设置
            //绑定日志事件
            broker.SendLogItemEvent += new ILogItemDel(Util.Log);
            //设定brokerconfg
            broker.SetBrokerConfig(cfg);

            //3.转换成Broker 注接口验证时已经保证了 broker对应的interface类型是实现IBroker接口的
            IBroker brokerinterface = broker as IBroker;
            brokerInstList.Add(cfg.Token, brokerinterface);

            //4.绑定Broker
            Vendor vendor = BasicTracker.VendorTracker[broker.VendorID];//获得该通道设定的VendorID
            if (vendor != null)
                (vendor as VendorImpl).BindBroker(brokerinterface);

            //5.绑定状态事件
            broker.Connected += (string b) =>
            {
                debug("Broker[" + b + "] Connected", QSEnumDebugLevel.INFO);
                if (BrokerConnectedEvent != null)
                    BrokerConnectedEvent(b);
            };
            broker.Disconnected += (string b) =>
            {
                debug("Broker[" + b + "] Disconnected", QSEnumDebugLevel.WARNING);
                if (BrokerDisconnectedEvent != null)
                    BrokerDisconnectedEvent(b);
            };
            //6.将broker的交易类事件绑定到路由内 然后通过路由转发到交易消息服务
            _brokerrouter.LoadBroker(brokerinterface);
        }
        /// <summary>
        /// 加载BrokerXAPI底层成交接口
        /// </summary>
        void LoadXAPI()
        {
            debug("Load XAPI Connector into system...", QSEnumDebugLevel.INFO);
            foreach (ConnectorConfig cfg in BasicTracker.ConnectorConfigTracker.BrokerConfigs)
            {
                LoadBrokerConnector(cfg);
            }

            foreach (ConnectorConfig cfg in BasicTracker.ConnectorConfigTracker.DataFeedConfigs)
            {
                TLDataFeedBase datafeed = CreateDataFeed(cfg);
                if (datafeed == null)
                    continue;

                datafeed.SendLogItemEvent += new ILogItemDel(Util.Log);
                datafeed.SetDataFeedConfig(cfg);
                IDataFeed datafeedinterface = datafeed as IDataFeed;
                datafeedInstList.Add(cfg.Token, datafeedinterface);

                datafeed.Connected += (string d) =>
                {
                    debug("DataFeed[" + d + "] Connected",QSEnumDebugLevel.INFO);
                    if (DataFeedConnectedEvent != null)
                        DataFeedConnectedEvent(d);
                };
                datafeed.Disconnected += (string d) =>
                {
                    debug("DataFeed[" + d + "] Disonnected",QSEnumDebugLevel.WARNING);
                    if (DataFeedDisconnectedEvent != null)
                        DataFeedDisconnectedEvent(d);
                };
                _datafeedrouter.LoadDataFeed(datafeedinterface);
            }
        
        }
    }
}
