using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.ResponseHost
{
    /// <summary>
    /// 策略接口
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// 获得外部市场行情
        /// </summary>
        /// <param name="tick"></param>
        void OnTick(Tick tick);

        /// <summary>
        /// 获得委托回报
        /// </summary>
        /// <param name="order"></param>
        void OnOrder(Order order);

        /// <summary>
        /// 获得成交回报
        /// </summary>
        /// <param name="fill"></param>
        void OnFill(Trade fill);

        /// <summary>
        /// 获得委托取消
        /// </summary>
        /// <param name="oid"></param>
        void OnCancel(long oid);

    }
}
