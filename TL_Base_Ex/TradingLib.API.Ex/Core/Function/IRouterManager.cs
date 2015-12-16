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
        /// <param name="id"></param>
        /// <returns></returns>
        IBroker FindBroker(int id);

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

        ///// <summary>
        ///// 获得所有成交路由
        ///// </summary>
        IEnumerable<IBroker> Brokers { get; }


        ///// <summary>
        ///// 获得所有行情路由
        ///// </summary>
        IEnumerable<IDataFeed> DataFeeds { get; }

    }
}
