using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.Shopping98
{
    public class DropShopping98Payment : Drop
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



        public string service { get; set; }

        public string version { get; set; }

        public string mch_no { get; set; }

        public string charset { get; set; }

        public string req_time { get; set; }

        public string sign { get; set; }

        public string sign_type { get; set; }

        public string nonce_str { get; set; }

        public string out_trade_no { get; set; }

        public string order_subject { get; set; }

        public string order_desc { get; set; }

        public string acquirer_type { get; set; }

        public string total_fee { get; set; }

        public string notify_url { get; set; }

        public string return_url { get; set; }

        public string currency { get; set; }

        public string client_ip { get; set; }

        public string order_time { get; set; }

        public string time_expire { get; set; }

        public string device_info { get; set; }

        public string attach { get; set; }

        public string extend { get; set; }

        public string url { get; set; }

    }
}
