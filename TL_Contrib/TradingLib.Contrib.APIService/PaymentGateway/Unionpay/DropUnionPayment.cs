using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.UnionPay
{
    public class DropUnionPayment : Drop
    {
        /// <summary>
        /// 支付网关地址
        /// </summary>
        public string PayUrl { get; set; }

        /// <summary>
        /// 交易账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get; set; }

        public string version { get; set; }

        public string encoding { get; set; }

        public string txnType { get; set; }

        public string txnSubType { get; set; }

        public string bizType { get; set; }

        public string signMethod { get; set; }

        public string channelType { get; set; }

        public string accessType { get; set; }

        public string backUrl { get; set; }

        public string frontUrl { get; set; }

        public string currencyCode { get; set; }

        public string payTimeout { get; set; }

        public string merId { get; set; }

        public string orderId { get; set; }

        public string txnTime { get; set; }

        public string txnAmt { get; set; }

        public string certId { get; set; }

        public string signature { get; set; }
    }
}
