using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    public class FollowItemOrder
    {
        /// <summary>
        /// 跟单项FollowKey
        /// </summary>
        public string FollowKey { get; set; }

        /// <summary>
        /// 对应委托编号
        /// </summary>
        public long OrderID { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }
    }

    public class FollowItemTrade
    {
        /// <summary>
        /// 跟单项FollowKey
        /// </summary>
        public string FollowKey { get; set; }

        /// <summary>
        /// 对应成交编号
        /// </summary>
        public string TradeID { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }
    }

    /// <summary>
    /// 定义了某个跟单策略的信号项
    /// 单个信号可以被多个策略监听 按不同策略的参数出发对应的操作
    /// 策略信号项可以加入相关参数 比如启用则可以针对不同的策略对信号进行特殊的设定
    /// </summary>
    public class StrategySignalItem
    {
        public int ID { get; set; }

        /// <summary>
        /// 跟单策略数据库ID
        /// </summary>
        public int StrategyID { get; set; }

        /// <summary>
        /// 信号数据库ID
        /// </summary>
        public int SignalID { get; set; }
    }


}
