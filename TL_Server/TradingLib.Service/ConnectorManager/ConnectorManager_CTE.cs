using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;
using TradingLib.Core;

namespace TradingLib.ServiceManager
{
    public partial class ConnectorManager
    {
        [ContribCommandAttr(QSEnumCommandSource.CLI, "PrintCI", "Print Connector Interface - 输出当前成交接口类型", "输出当前成交接口类型")]
        public string PrintBrokerInterface()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-----------BrokerInterface List--------------" + Environment.NewLine);
            foreach (ConnectorInterface itface in BasicTracker.ConnectorConfigTracker.BrokerInterfaces)
            {
                sb.Append("Type:" + itface.type_name + " XAPI:" + itface.IsXAPI.ToString() + Environment.NewLine);
            }
            sb.Append("-----------DataFeedInterface List--------------" + Environment.NewLine);
            foreach (ConnectorInterface itface in BasicTracker.ConnectorConfigTracker.DataFeedInterfaces)
            {
                sb.Append("Type:" + itface.type_name + " XAPI:" + itface.IsXAPI.ToString() + Environment.NewLine);
            }
            return sb.ToString();
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "PrintConnector", "PrintConnector - 输出当前加载的数据与成交通道接口", "输出加载的成交与行情通道接口")]
        public string PrintConnector()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-----------Brokers List------------" + "\r\n");
            foreach (IBroker b in brokerInstList.Values)
            {
                sb.Append(b.Token.PadRight(20, ' ') + "Status:" + b.IsLive.ToString() + "\r\n");
            }
            sb.Append("-----------DataFeeds List---------" + "\r\n");
            foreach (IDataFeed f in datafeedInstList.Values)
            {
                sb.Append(f.Token.PadRight(20, ' ') + "Status:" + f.IsLive.ToString() + "\r\n");
            }
            return sb.ToString();
        }


        

        [ContribCommandAttr(QSEnumCommandSource.CLI, "startbroker", "startbroker - 启动某个成交通道", "启动某个成交通道")]
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "startbroker", "startbroker - 启动某个成交通道", "用于Web端停止某个某个交易通道")]
        public void StartBrokerViaToken(string name)
        {
            try
            {
                StartBroker(name);
            }
            catch (Exception ex)
            {
                debug("start broker error:" + ex.ToString(),QSEnumDebugLevel.ERROR);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "stopbroker", "stopbroker - 停止某个成交通道", "用于Web端停止某个交易通道")]
        public void StopBrokerViaToken(string name)
        {
            try
            {
                StopBroker(name);
            }
            catch (Exception ex)
            {
                debug("start broker error:" + ex.ToString(),QSEnumDebugLevel.ERROR);
            }
        }
        
        [ContribCommandAttr(QSEnumCommandSource.CLI, "startdatafeed", "startdatafeed - 启动某个数据通道", "用于Web端启动某个数据通道")]
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "startdatafeed", "startdatafeed - 启动某个数据通道", "用于Web端启动某个数据通道")]
        public void StartDataFeedViaToken(string name)
        {
            debug("启动数据通道:"+name, QSEnumDebugLevel.INFO);
            try
            {
                StartDataFeed(name);
            }
            catch (Exception ex)
            {
                debug("start datafeed error:" + ex.ToString());
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "stopdatafeed", "stopdatafeed - 停止某个数据通道", "用于Web端停止某个数据通道")]
        public void StopDataFeedViaToken(string name)
        {
            debug("停止数据通道:" + name, QSEnumDebugLevel.INFO);
            try
            {
                StopDataFeed(name);
            }
            catch (Exception ex)
            {
                debug("stop datafeed error:" + ex.ToString());
            }
        }
        
        #region Vendor RouterGroup
        [ContribCommandAttr(QSEnumCommandSource.CLI, "vendorstatus", "vendorstatus - print status of vendor", "输出某个实盘帐户状态")]
        public string PrintConnector(int vid)
        {
            StringBuilder sb = new StringBuilder();
            Vendor vendor = BasicTracker.VendorTracker[vid];
            if (vendor == null)
            {
                return "vendor:" + vid.ToString() + " do not exist";
            }
            sb.Append(string.Format("ID:{0} Name:{1} FutCompany:{2} LastEquity:{3}", vendor.ID, vendor.Name, vendor.FutCompany, vendor.LastEquity)+Environment.NewLine);
            if(vendor.Broker != null)
            {
                IBroker broker = vendor.Broker;
                sb.Append(string.Format("Broker:{0} Margin:{1} RealizedPL:{2} UnRealizedPL:{3}", broker.Token, vendor.CalMargin(), vendor.CalRealizedPL(), vendor.CalUnRealizedPL()));
            
            }
            return sb.ToString();
        }
        #endregion
    }
}
