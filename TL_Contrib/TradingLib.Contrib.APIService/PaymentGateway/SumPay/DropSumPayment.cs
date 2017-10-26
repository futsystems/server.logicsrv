using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.SumPay
{
    public class DroSumPayment : Drop
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


        public string requestId { get; set; }

        public string tradeProcess { get; set; }

        public string totalBizType { get; set; }

        public string totalPrice { get; set; }

        public string bankcode { get; set; }

        public string backurl { get; set; }

        public string returnurl { get; set; }

        public string noticeurl { get; set; }

        public string description { get; set; }

        public string mersignature { get; set; }


        public string productId { get; set; }

        public string productName { get; set; }

        public string fund { get; set; }

        public string productNumber { get; set; }

        public string merAcct { get; set; }

        public string bizType { get; set; }
    }
}
