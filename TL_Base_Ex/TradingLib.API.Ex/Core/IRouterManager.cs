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

        ///// <summary>
        ///// 获得所有成交路由
        ///// </summary>
        IEnumerable<IBroker> Brokers { get; }


        ///// <summary>
        ///// 获得所有行情路由
        ///// </summary>
        IEnumerable<IDataFeed> DataFeeds { get; }


        #region 行情类数据

        /// <summary>
        /// 获得某个合约当前有效价格
        /// 通过DataRouter进行获取
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //decimal GetAvabilePrice(string symbol);

        /// <summary>
        /// 获得市场快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //Tick GetTickSnapshot(string symbol);


        /// <summary>
        /// 判定合约行情是否处于live状态
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //bool IsSymbolTickLive(string symbol);

        #endregion

    }
}
