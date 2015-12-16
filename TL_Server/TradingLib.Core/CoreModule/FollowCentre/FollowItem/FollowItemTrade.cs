using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
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
}
