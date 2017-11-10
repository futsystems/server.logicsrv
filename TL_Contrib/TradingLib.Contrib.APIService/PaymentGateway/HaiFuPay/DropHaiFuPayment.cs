using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.HaiFu
{
    public class DropHaiFuPayment : Drop
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


        public string body { get; set; }
        public string total_fee { get; set; }
        public string product_id { get; set; }
        public string goods_tag { get; set; }
        public string op_user_id { get; set; }
        public string nonce_str { get; set; }
        public string spbill_create_ip { get; set; }
        public string notify_url { get; set; }
        public string front_notify_url { get; set; }
        public string wx_app_id { get; set; }
        public string sub_openid { get; set; }
        public string pay_type { get; set; }
        public string bank_id { get; set; }
        public string sign { get; set; }
        public string link { get; set; }

    }
}
