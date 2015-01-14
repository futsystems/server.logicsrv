using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    
    /// <summary>
    /// broker对应的交易信息
    /// </summary>
    public interface IBrokerTradingInfo
    {
        /// <summary>
        /// 获得日内成交接口的所有委托
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<Order> SelectBrokerOrders(string token);

        /// <summary>
        /// 获得日内成交接口的所有成交
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<Trade> SelectBrokerTrades(string token);

        /// <summary>
        /// 获得成交接口上个结算日所有持仓数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token);


        /// <summary>
        /// 获得路右侧委托
        /// </summary>
        /// <returns></returns>
        IEnumerable<Order> SelectRouterOrders();
    }
}
