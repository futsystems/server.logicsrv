﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;


namespace TradingLib.ServiceManager
{
    public partial class ConnectorManager
    {
        /// <summary>
        /// 重置通道连接
        /// </summary>
        [TaskAttr("重置行情与交易通道", 8, 30,0, "日盘前重置行情与交易通道")]
        [TaskAttr("重置行情与交易通道", 20, 30,0, "日盘前重置行情与交易通道")]
        public void Task_ResetConnector()
        {
            logger.Info("正常交易日,重置所有路由通道");
            this.Reset();
        }



        void StartConnector()
        {
            //1.启动默认通道
            logger.Info("Start Default Connector");
            StartDataFeedViaToken(_defaultDataFeedToken);
            Util.sleep(500);
            StartBrokerViaToken(_defaultSimBrokerToken);
            Util.sleep(500);

            //2.启动实盘通道
            logger.Info("Start Broker Connector");
            //把有效域内绑定的实盘帐户对应的通道启动起来
            foreach (Domain domain in BasicTracker.DomainTracker.Domains)
            {
                if (domain.IsExpired())//过期通道不启动
                    continue;

                //遍历该域名下所有路由组
                foreach (RouterGroup rg in domain.GetRouterGroups())
                {
                    foreach (RouterItem item in rg.RouterItems)
                    {
                        //如果Vendor无效或者没有对应的broker则跳过
                        if (item.Broker == null)
                            continue;
                        StartBrokerViaToken(item.Broker.Token);
                        Util.sleep(500);
                    }
                }
            }
        }

        void StopConnector()
        {
            logger.Info("Stop Default Connector");
            //1.停止默认通道
            StopDataFeedViaToken(_defaultDataFeedToken);
            Util.sleep(500);
            StopBrokerViaToken(_defaultSimBrokerToken);
            Util.sleep(500);

            logger.Info("Stop Broker Connector");
            //2.停止实盘通道
            foreach (IBroker b in brokerInstList.Values)
            {
                if (b.IsLive)
                {
                    StopBrokerViaToken(b.Token);
                    Util.sleep(500);
                }
            }
        }
    }
}
