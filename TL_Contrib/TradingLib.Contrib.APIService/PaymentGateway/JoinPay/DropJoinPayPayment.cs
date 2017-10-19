using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.JoinPay
{
    public class DropJoinPayPayment : Drop
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

        public string p1_MerchantNo { get; set; }

        public string p2_OrderNo { get; set; }

        public string p3_Amount { get; set; }

        public string p4_Cur { get; set; }

        public string p5_ProductName { get; set; }

        public string p6_Mp { get; set; }

        public string p7_ReturnUrl { get; set; }

        public string p8_NotifyUrl { get; set; }

        public string p9_FrpCode { get; set; }

        public string pa_OrderPeriod { get; set; }

        public string hmac { get; set; }


    }
}
