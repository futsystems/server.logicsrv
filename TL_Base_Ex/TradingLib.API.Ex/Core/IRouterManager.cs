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
        /// 获得所有成交路由
        /// </summary>
        IBroker[] Brokers { get;}


        /// <summary>
        /// 获得所有行情路由
        /// </summary>
        IDataFeed[] DataFeeds { get; }


    }
}
