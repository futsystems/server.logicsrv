using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.C9Pay
{
    public class DropC9Payment : Drop
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


        public string MerchantNo { get; set; }

        public string OrderNo { get; set; }

        public string AmountI { get; set; }

        public string GoodsInfo { get; set; }

        public string ReturnUrl { get; set; }

        public string NotifyUrl { get; set; }

        public string BankCode { get; set; }

        public string signMd5 { get; set; }

      
    }
}
