using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 路由事件
    /// 汇聚从行情路由与交易路由返回的数据回报
    /// </summary>
    public class RouterEvent
    {
        /// <summary>
        /// 行情路由获得一个行情
        /// </summary>
        public event TickDelegate GotTickEvent;

        /// <summary>
        /// 成交路由-委托错误事件
        /// </summary>
        public event OrderErrorDelegate GotOrderErrorEvent;
        /// <summary>
        /// 成交路由-委托操作事件
        /// </summary>
        public event OrderActionErrorDelegate GotOrderActionErrorEvent;
        /// <summary>
        /// 成交路由-成交事件
        /// </summary>
        public event FillDelegate GotFillEvent;
        /// <summary>
        /// 成交路由-委托事件
        /// </summary>
        public event OrderDelegate GotOrderEvent;
        /// <summary>
        /// 委托取消事件
        /// </summary>
        public event LongDelegate GotCancelEvent;


        internal void FireTickEvent(Tick k)
        {
            if (GotTickEvent != null)
                GotTickEvent(k);
        }

        internal void FireOrderError(Order o, RspInfo e)
        {
            if (GotOrderErrorEvent != null)
                GotOrderErrorEvent(o, e);
        }

        internal void FireOrderActionErrorEvent(OrderAction a, RspInfo e)
        {
            if (GotOrderActionErrorEvent != null)
                GotOrderActionErrorEvent(a, e);
        }

        internal void FireFillEvent(Trade f)
        {
            if(GotFillEvent != null)
                GotFillEvent(f);
        }

        internal void FireOrderEvent(Order o)
        {
            if(GotOrderEvent != null)
                GotOrderEvent(o);
        }

        internal void FireCancelEvent(long oid)
        {
            if (GotCancelEvent != null)
                GotCancelEvent(oid);
        }
    }
}
