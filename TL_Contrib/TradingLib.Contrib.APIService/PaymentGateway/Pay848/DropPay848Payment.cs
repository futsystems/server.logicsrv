using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.Pay848
{
    public class DropPay848Payment : Drop
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


        public string parter { get; set; }

        public string type { get; set; }

        public string value { get; set; }

        public string orderid { get; set; }

        public string callbackurl { get; set; }

        public string hrefbackurl { get; set; }

        public string payerIp { get; set; }

        public string attach { get; set; }

        public string sign { get; set; }

       
    }
}
