using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public delegate void ErrorOrderDel(ErrorOrder error);
    /// <summary>
    /// 定义委托错误对象
    /// 用于封装委托和对应的委托 
    /// 在服务端组件之间进行传递
    /// </summary>
    public class ErrorOrder
    {

        public ErrorOrder(Order o, RspInfo info)
        {
            Order = o;
            RspInfo = info;
        }
        /// <summary>
        /// 对应的委托
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public RspInfo RspInfo { get; set; }

        public void Fill(ErrorOrderNotify notify)
        {
            notify.Order = this.Order;
            notify.RspInfo = this.RspInfo;
        }
    }
}
