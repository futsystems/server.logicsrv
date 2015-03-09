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
        /// 查找某个交易路由
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


        /// <summary>
        /// 获得某个合约的市场快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //public Tick GetTickSnapshot(string symbol)
        //{
        //    return _datafeedrouter.GetTickSnapshot(symbol);
        //}

        /// <summary>
        /// 判断某个合约当前行情是否处于live状态
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //public bool IsSymbolTickLive(string symbol)
        //{
        //    return _datafeedrouter.IsTickLive(symbol);
        //}


        /// <summary>
        /// 获得某个合约的有效价格
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //public decimal GetAvabilePrice(string symbol)
        //{
        //    return _datafeedrouter.GetAvabilePrice(symbol);
        //}

    }
}
