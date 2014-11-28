using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
                //IEnumerable<RouterItem> map = BasicTracker.RouterGroupTracker.GetRouterItem(rg.ID);
                //foreach (RouterItem rm in map)
                //{
                //    if (!rm.Active)
                //        continue;
                //    ConnectorConfig cfg = ConnectorConfigTracker.GetBrokerConfig(rm.connector_id);
                //    IBroker broker = FindBroker(cfg.Token);
                //    if (broker != null)
                //    {
                //        debug("append broker:"+broker.Token +" to routergroup:"+rg.Name,QSEnumDebugLevel.INFO);
                //        //将
                //        rg.AppendBroker(broker,rm.priority);
                //    }

                //}
                //需要在BrokerRouter启动后才可以启动通道 否则由于Router侧委托没有正常加载,导致成交侧交易数据加载不正常
                //rg.Start();
            }
        }


    }
}
