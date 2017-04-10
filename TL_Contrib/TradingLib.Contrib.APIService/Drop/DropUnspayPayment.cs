using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib
{
    public class DropUnspayPayment:Drop
    {
        public string MerchantID { get; set; }

        public string MerchantUrl { get; set; }

        public string ResponseMode { get; set; }

        public string OrderID { get; set; }

        public string CurrencyType { get; set; }

        public string OrderAmount { get; set; }

        public string AssuredPay { get; set; }

        public string Time { get; set; }

        public string Remark { get; set; }

        public string MAC { get; set; }


        public string MerchantKey { get; set; }


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
