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
            foreach (ConnectorInterface itface in ConnectorConfigTracker.BrokerInterfaces)
            {
                sb.Append("Type:" + itface.type_name + " XAPI:" + itface.IsXAPI.ToString() + Environment.NewLine);
            }
            sb.Append("-----------DataFeedInterface List--------------" + Environment.NewLine);
            foreach (ConnectorInterface itface in ConnectorConfigTracker.DataFeedInterfaces)
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


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryInterface", "QryInterface - query interface setted in system", "查询所有接口设置")]
        public void QueryInterface(ISession session)
        {
            debug("查询所有接口设置", QSEnumDebugLevel.INFO);
            //查询所有代理出入金记录
            Manager manger = session.GetManager();
            if (manger != null)
            {
                ConnectorInterface[] ops = ConnectorConfigTracker.Interfaces.ToArray();
                session.SendJsonReplyMgr(ops);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "startbroker", "startbroker - 启动某个成交通道", "启动某个成交通道")]
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "startbroker", "startbroker - 启动某个成交通道", "用于Web端停止某个某个交易通道")]
        public void StartBrokerViaToken(string name)
        {
            try
            {
                debug("启动成交通道:" + name, QSEnumDebugLevel.INFO);
                IBroker b = FindBroker(name);
                if (b == null)
                {
                    debug("can not find broker:" + name, QSEnumDebugLevel.WARNING);
                    return;
                }
                if (b.IsLive)
                {
                    debug("Broker " + name + " already started", QSEnumDebugLevel.WARNING);
                    return;
                }
                
                b.Start();
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
                debug("停止成交通道:" + name, QSEnumDebugLevel.INFO);
                IBroker b = FindBroker(name);
                if (b == null)
                {
                    debug("can not find broker:" + name, QSEnumDebugLevel.WARNING);
                    return;
                }
                if (!b.IsLive)
                {
                    debug("Broker " + name + " already stopped", QSEnumDebugLevel.WARNING);
                    return;
                }
                b.Stop();
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
                IDataFeed d = FindDataFeed(name);
                if (d == null)
                {
                    debug("can not find datafeed:" + name, QSEnumDebugLevel.WARNING);
                    return;
                }
                if (d.IsLive)
                {
                    debug("DataFeed " + name + " already started", QSEnumDebugLevel.WARNING);
                    return;
                }
                d.Start();
               
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
                string fullname = "DataFeed.FastTick.FastTick";

                //debug("xxxxxxxxxxxxxxxxx", QSEnumDebugLevel.INFO);
                IDataFeed d = FindDataFeed(name);
                if (d != null && d.IsLive)
                {
                    d.Stop();
                }

            }
            catch (Exception ex)
            {
                debug("stop datafeed error:" + ex.ToString());
            }
        }
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qrybrokerstatus", "qrybrokerstatus - 查询某个成交通道状态", "用于Web端查询某个成交通道状态")]
        public void QryBrokerStatus(string name)
        {
            IBroker b = FindBroker(name);
            if (b == null)
            {
                //ReplyHelper.GenericError(ReplyType.BrokerNotFound);
            }
            else
            { 
                
            }
            
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qrydatafeedstatus", "qrydatafeedstatus - 查询某个行情通道状态", "用于Web端查询某个行情通道状态")]
        public void QryDataFeedStatus(string name)
        {
            IDataFeed d = FindDataFeed(name);
            if (d == null)
            {

            }
            else
            { 
            
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qryconnector", "qryconnector - 查询所有通道信息与状态", "用于Web端查询查询所有通道信息与状态")]
        public string QryConnector()
        {
            List<ConnectorWrapper> list = new List<ConnectorWrapper>();
            foreach (IBroker b in brokerInstList.Values)
            {
                list.Add(new ConnectorWrapper(b as IConnecter));
            }

            foreach (IDataFeed d in datafeedInstList.Values)
            {
                list.Add(new ConnectorWrapper(d as IConnecter));
            }

            JsonWriter w = ReplyHelper.NewJWriterSuccess();
            ReplyHelper.FillJWriter(list.ToArray(), w);
            ReplyHelper.EndWriter(w);

            return w.ToString();
            
        }

       

        internal class ConnectorWrapper
        {
            IConnecter connector;
            public ConnectorWrapper(IConnecter c)
            {
                connector = c;
            }

            public string Name { get { return connector.Token; } }
            public string ClassName { get { return connector.GetType().FullName; } }
            public bool Status { get { return connector.IsLive; } }
            public string Type { get { return connector is IBroker ? "Broker" : "DataFeed"; } }
              
        }

    }
}
