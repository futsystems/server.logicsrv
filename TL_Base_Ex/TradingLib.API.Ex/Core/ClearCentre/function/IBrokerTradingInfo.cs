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
        /// 获得某个交易通道当天的委托数据 用于从数据库恢复委托数据
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        IList<Order> getOrders(IBroker broker);

    }
}
