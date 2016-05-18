using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Core
{
    public class ErrorID
    {
        /// <summary>
        /// 结算中心处于结算状态
        /// </summary>
        public static string SETTLECENTRE_IN_SETTLE = "SETTLECENTRE_IN_SETTLE";

        /// <summary>
        /// 合约不存在
        /// </summary>
        public static string SYMBOL_NOT_EXISTED = "SYMBOL_NOT_EXISTED";


        /// <summary>
        /// 合约部可交易
        /// </summary>
        public static string SYMBOL_NOT_TRADEABLE = "SYMBOL_NOT_TRADEABLE";

        /// <summary>
        /// 合约已到期
        /// </summary>
        public static string SYMBOL_EXPIRED = "SYMBOL_EXPIRED";

        /// <summary>
        /// 当前时间合约不可交易
        /// </summary>
        public static string SYMBOL_NOT_MARKETTIME = "SYMBOL_NOT_MARKETTIME";


        /// <summary>
        /// 委托数量为零
        /// </summary>
        public static string ORDER_SIZE_ZERO = "ORDERSIZE_ZERO";

        /// <summary>
        /// 委托数量超过限制
        /// </summary>
        public static string ORDER_SIZE_LIMIT = "ORDER_SIZE_LIMIT";

        /// <summary>
        /// 委托价格超过限制
        /// </summary>
        public static string ORDER_PRICE_LIMIT = "ORDER_PRICE_LIMIT";


        /// <summary>
        /// 禁止做空
        /// </summary>
        public static string ORDER_SHORT_FORBIDDEN = "ORDER_SHORT_FORBIDDEN";


        /// <summary>
        /// 禁止锁仓
        /// </summary>
        public static string POSITION_LOCK_FORBIDDEN = "POSITION_LOCK_FORBIDDEN";
    }
}
