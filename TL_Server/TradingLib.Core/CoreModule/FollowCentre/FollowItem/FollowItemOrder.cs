using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

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
}
