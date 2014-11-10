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
        IBroker _defaultsimbroker = null;
        public IBroker DefaultSimBroker { get { return _defaultsimbroker; } }


        IDataFeed _defaultdatafeed = null;
        public IDataFeed DefaultDataFeed { get { return _defaultdatafeed; } }

        /// <summary>
        /// 查找某个交易路由
        /// </summary>
        /// <param name="fullname"></param>
        public IBroker FindBroker(string fullname)
        {
            debug("查找成交路由:" + fullname, QSEnumDebugLevel.INFO);
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
            debug("查找数据路由:" + fullname, QSEnumDebugLevel.INFO);
            if (datafeedInstList.Keys.Contains(fullname))
            {
                return datafeedInstList[fullname];
            }
            return null;

        }
        public Tick GetTickSnapshot(string symbol)
        {
            return _datafeedrouter.GetTickSnapshot(symbol);
        }

        public IEnumerable<Tick> GetTickSnapshot()
        {
            return _datafeedrouter.GetTickSnapshot();
        }


        /// <summary>
        /// 获得所有成交路由
        /// </summary>
        public IBroker[] Brokers { get { return brokerInstList.Values.ToArray(); } }

        /// <summary>
        /// 获得所有行情路由
        /// </summary>
        public IDataFeed[] DataFeeds { get { return datafeedInstList.Values.ToArray(); } }
    }
}
