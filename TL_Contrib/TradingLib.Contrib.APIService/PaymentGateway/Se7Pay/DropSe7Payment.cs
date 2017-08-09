using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.Se7Pay
{
    public class DropSe7Payment:Drop
    {

        public string company_oid { get; set; }

        public string order_id { get; set; }

        public string item_name { get; set; }

        public string item_info { get; set; }

        public string money_num { get; set; }

        public string notify_url { get; set; }

        public string return_url { get; set; }

        public string pay_type { get; set; }

        public string sign { get; set; }


        public string bank_id { get; set; }

        public string card_id { get; set; }

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
