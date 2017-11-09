using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.UUOPay
{
    public class DropUUOPayPayment : Drop
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

        public string apiName { get; set; }
        public string apiVersion { get; set; }
        public string platformID { get; set; }
        public string merchNo { get; set; }
        public string orderNo { get; set; }
        public string tradeDate { get; set; }
        public string amt { get; set; }
        public string notifyUrl { get; set; }
        public string returnUrl { get; set; }
        public string merchParam { get; set; }
        public string tradeSummary { get; set; }
        public string signMsg { get; set; }
        public string bankCode { get; set; }
        public string choosePayType { get; set; }

    }
}
