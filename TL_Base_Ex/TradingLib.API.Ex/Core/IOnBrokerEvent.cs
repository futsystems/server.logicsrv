using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 响应BrokerRouter汇聚上来的成交接口事件接口
    /// BrokerRouter的数据回报通过IOnBrokerEvent接口进行处理
    /// </summary>
    public interface IOnBrokerEvent
    {
        void OnOrderEvent(Order o);
        void OnFillEvent(Trade f);
        void OnCancelEvent(long oid);

        void OnOrderErrorEvent(Order o, RspInfo e);
        void OnOrderActionErrorEvent(OrderAction action, RspInfo e);
    }
}
