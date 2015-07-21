using System;
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
        [TaskAttr("日盘收盘后停止通道", 16, 40, 0, "每天下午16：40停止通道")]//判定当前交易日状态，系统很多其他事务是按结算状态来进行的
        [TaskAttr("夜盘收盘后停止通道", 2, 40, 0, "每天晚上2：40停止通道")]//判定当前交易日状态，系统很多其他事务是按结算状态来进行的
        public void Task_ResetTradingdayNieght()
        {
            debug("停止通道连接", QSEnumDebugLevel.INFO);
            StopConnector();
        }


        /// <summary>
        /// 重置通道连接
        /// </summary>
        [TaskAttr("重置行情与交易通道", 8, 30,0, "日盘前重置行情与交易通道")]
        [TaskAttr("重置行情与交易通道", 20, 30,0, "日盘前重置行情与交易通道")]
        public void Task_ResetConnector()
        {
            if (TLCtxHelper.Ctx.SettleCentre.IsTradingday)//如果是交易日则需要启动实盘通道
            {
                debug("正常交易日,重置所有路由通道", QSEnumDebugLevel.INFO);
                
                //1.停止所有通道
                StopConnector();

                //2.启动所有通道
                StartConnector();
            }
        }



        void StartConnector()
        {
            //1.启动默认通道
            StartDataFeedViaToken(_defaultDataFeedToken);
            Util.sleep(500);
            StartBrokerViaToken(_defaultSimBrokerToken);
            Util.sleep(500);
            //2.启动实盘通道
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
                        if (item.Vendor == null || item.Vendor.Broker == null)
                            continue;
                        StartBrokerViaToken(item.Vendor.Broker.Token);
                        Util.sleep(500);
                    }
                }
            }
        }

        void StopConnector()
        {
            //1.停止默认通道
            StopDataFeedViaToken(_defaultDataFeedToken);
            Util.sleep(500);
            StopBrokerViaToken(_defaultSimBrokerToken);
            Util.sleep(500);
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
