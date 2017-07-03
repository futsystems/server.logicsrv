using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.ZhiHPay
{
    public class DropZhiHPayment : Drop
    {
        public string sign { get; set; }

        public string merchant_code { get; set; }

        public string bank_code { get; set; }

        public string order_no { get; set; }

        public string order_amount { get; set; }

        public string service_type { get; set; }

        public string input_charset { get; set; }

        public string notify_url { get; set; }

        public string interface_version { get; set; }

        public string sign_type { get; set; }

        public string order_time { get; set; }

        public string product_name { get; set; }

        public string client_ip_check { get; set; }

        public string client_ip { get; set; }

        public string extend_param { get; set; }

        public string extra_return_param { get; set; }

        public string product_code { get; set; }

        public string product_desc { get; set; }

        public string product_num { get; set; }

        public string return_url { get; set; }

        public string show_url { get; set; }

        public string redo_flag { get; set; }

        public string pay_type { get; set; }

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
