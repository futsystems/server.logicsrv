using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.OpenEPay
{
    public class DropOpenEPayPayment : Drop
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

        public string inputCharset { get; set; }


        public string pickupUrl { get; set; }

        public string receiveUrl { get; set; }

        public string version { get; set; }

        public string language { get; set; }

        public string signType { get; set; }

        public string merchantId { get; set; }

        public string orderNo { get; set; }

        public string orderAmount { get; set; }

        public string orderCurrency { get; set; }

        public string orderDatetime { get; set; }

        public string productName { get; set; }

        public string payType { get; set; }

        public string signMsg { get; set; }



    }
}
