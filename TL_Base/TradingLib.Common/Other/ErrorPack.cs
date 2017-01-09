using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// 委托错误消息体 用于缓存队列
    /// </summary>
    public class OrderErrorPack
    {
        public OrderErrorPack(Order o, RspInfo e)
        {
            this.Order = o;
            this.RspInfo = e;
        }
        /// <summary>
        /// 委托
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public RspInfo RspInfo { get; set; }
    }

    /// <summary>
    /// 委托操作错误消息体 用于缓存队列
    /// </summary>
    public class OrderActionErrorPack
    {
        public OrderActionErrorPack(OrderAction a, RspInfo e)
        {
            this.OrderAction = a;
            this.RspInfo = e;
        }

        /// <summary>
        /// 委托操作
        /// </summary>
        public OrderAction OrderAction { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public RspInfo RspInfo { get; set; }
    }
}
