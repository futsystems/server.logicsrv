using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.GYPay
{
    public class DropGYPayment : Drop
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


        public string gymchtId { get; set; }

        public string tradeSn { get; set; }

        public string orderAmount { get; set; }

        public string goodsName { get; set; }

        public string bankSegment { get; set; }

        public string cardType { get; set; }

        public string notifyUrl { get; set; }

        public string callbackUrl { get; set; }

        public string expirySecond { get; set; }

        public string channelType { get; set; }

        public string nonce { get; set; }

        public string sign { get; set; }

        public string link { get; set; }
    }
}
