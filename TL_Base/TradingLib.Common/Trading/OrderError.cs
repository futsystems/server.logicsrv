using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
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
}
