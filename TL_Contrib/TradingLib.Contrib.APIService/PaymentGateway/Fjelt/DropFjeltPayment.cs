using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.Fjelt
{
    public class DropFjeltPayment : Drop
    {

        public string appid { get; set; }

        public string method { get; set; }

        public string format { get; set; }

        public string data { get; set; }

        public string timestamp { get; set; }

        public string session { get; set; }

        public string sign { get; set; }

        public string v { get; set; }

        public string url { get; set; }

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

    }
}
