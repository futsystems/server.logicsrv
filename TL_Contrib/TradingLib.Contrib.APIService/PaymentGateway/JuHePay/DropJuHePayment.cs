using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.JuHe
{
    public class DropJuHePayment : Drop
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


        public string order_no { get; set; }

        public string app_id { get; set; }

        public string channel { get; set; }

        public string client_ip { get; set; }

        public string subject { get; set; }

        public string body { get; set; }

        public string notify_url { get; set; }

        public string result_url { get; set; }

        public string url { get; set; }

    }
}
