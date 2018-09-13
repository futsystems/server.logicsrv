using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.PlugPay
{
    public class DropPlugPayPayment : Drop
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

        public string p1_app_id { get; set; }

        public string p2_channel { get; set; }

        public string p3_bank_code { get; set; }

        public string p4_bill_no { get; set; }

        public string p5_total_fee { get; set; }

        public string p6_goods_title { get; set; }

        public string p7_goods_desc { get; set; }

        public string p8_goods_extended { get; set; }

        public string p9_timestamp { get; set; }

        public string p10_return_url { get; set; }

        public string p11_notify_url { get; set; }

        public string p12_other { get; set; }

        public string p0_sign { get; set; }    

    }
}
