using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.ServiceManager
{
    public partial class ConnectorManager
    {
        IBroker _defaultsimbroker = null;
        public IBroker DefaultSimBroker { get { return _defaultsimbroker; } }

        IDataFeed _defaultdatafeed = null;
        public IDataFeed DefaultDataFeed { get { return _defaultdatafeed; } }

        /// <summary>
        /// 通过ConnectorID查找获得IBroker对象
        /// </summary>
        /// <param name="connector_id"></param>
        /// <returns></returns>
        public IBroker FindBroker(int connector_id)
        {
            return ID2Broker(connector_id);
        }
        /// <summary>
        /// 查找某个交易路由
        /// 通过交易通道的Token来查找对应的Broker
        /// </summary>
        /// <param name="fullname"></param>
        public IBroker FindBroker(string fullname)
        {
            if (brokerInstList.Keys.Contains(fullname))
            {
                return brokerInstList[fullname];
            }
            return null;
        }
        /// <summary>
        /// 查找某个数据路由
        /// </summary>
        /// <param name="fullname"></param>
        public IDataFeed FindDataFeed(string fullname)
        {
            if (datafeedInstList.Keys.Contains(fullname))
            {
                return datafeedInstList[fullname];
            }
            return null;

        }

        /// <summary>
        /// 获得所有成交路由
        /// </summary>
        public IEnumerable<IBroker> Brokers { get { return brokerInstList.Values; } }

        /// <summary>
        /// 获得所有行情路由
        /// </summary>
        public IEnumerable<IDataFeed> DataFeeds { get { return datafeedInstList.Values; } }
    }
}
