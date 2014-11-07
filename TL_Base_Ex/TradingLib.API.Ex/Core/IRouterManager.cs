using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IRouterManager
    {

        /// <summary>
        /// 查找成交路由
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        IBroker FindBroker(string fullname);

        /// <summary>
        /// 查找行情路由
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        IDataFeed FindDataFeed(string fullname);

        /// <summary>
        /// 默认的模拟成交接口
        /// </summary>
        IBroker DefaultSimBroker { get; }

        /// <summary>
        /// 默认行情通道
        /// </summary>
        IDataFeed DefaultDataFeed { get; }

        /// <summary>
        /// 获得某个合约的行情快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Tick GetTickSnapshot(string symbol);

        /// <summary>
        /// 获得所有市场数据行情快照
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tick> GetTickSnapshot(); 

        /// <summary>
        /// 获得所有成交路由
        /// </summary>
        IBroker[] Brokers { get;}


        /// <summary>
        /// 获得所有行情路由
        /// </summary>
        IDataFeed[] DataFeeds { get; }


    }
}
