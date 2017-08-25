using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.ZhongWei
{
    public class DropZhongWeiPayment : Drop
    {
        public string account_no { get; set; }

        public string method { get; set; }

        public string productId { get; set; }

        public string version { get; set; }

        public string nonce_str { get; set; }

        public string pay_tool { get; set; }

        public string order_sn { get; set; }

        public string money { get; set; }

        public string ex_field { get; set; }

        public string body { get; set; }

        public string bankCode { get; set; }

        public string notify { get; set; }

        public string return_url { get; set; }

        public string signature { get; set; }



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
    }
}
