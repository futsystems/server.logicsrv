using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.NewPay
{
    public class DropNewPayPayment : Drop
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

        public string partnerId { get; set; }

        public string orderId { get; set; }

        public string goods { get; set; }

        public string iamount { get; set; }


        public string expTime { get; set; }

        public string notifyUrl { get; set; }

        public string pageUrl { get; set; }

        public string reserve { get; set; }

        public string extendInfo { get; set; }

        public string payMode { get; set; }

        public string bankId { get; set; }

        public string creditType { get; set; }

        public string sign { get; set; }

       

    }
}
