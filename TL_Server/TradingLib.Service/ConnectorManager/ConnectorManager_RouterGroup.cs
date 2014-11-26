using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;

namespace TradingLib.ServiceManager
{
    public partial class ConnectorManager
    {

        /// <summary>
        /// 初始化路由组
        /// </summary>
        void InitRouterGroup()
        {
            debug("Init RouterGroup,append brokers in to routergroups....", QSEnumDebugLevel.INFO);
            foreach (RouterGroup rg in BasicTracker.RouterGroupTracker.RouterGroups)
            {
                //获得该路由组的所有通道映射
                IEnumerable<RouterConnecterMap> map = BasicTracker.RouterGroupTracker.GetRouterConnectorMap(rg.ID);
                foreach(RouterConnecterMap rm in map)
                {
                    ConnectorConfig cfg = ConnectorConfigTracker.GetBrokerConfig(rm.connector_id);
                    IBroker broker = FindBroker(cfg.Token);
                    if (broker != null)
                    {
                        debug("append broker:"+broker.Token +" to routergroup:"+rg.Name,QSEnumDebugLevel.INFO);
                        //将
                        rg.AppendBroker(broker,rm.priority);
                    }

                }
                rg.Start();
            }
        }
    }
}
