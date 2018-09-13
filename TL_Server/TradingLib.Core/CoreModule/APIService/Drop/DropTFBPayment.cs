using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.APIService
{
    public class DropTFBPayment : Drop
    {
        public string spid { get; set; }

        public string sp_userid { get; set; }

        public string spbillno { get; set; }

        public string money { get; set; }

        public string cur_type { get; set; }

        public string return_url { get; set; }

        public string notify_url { get; set; }

        public string memo { get; set; }

        public string card_type { get; set; }

        public string bank_segment { get; set; }

        public string user_type { get; set; }

        public string channel { get; set; }

        public string encode_type { get; set; }

        public string sign { get; set; }

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
