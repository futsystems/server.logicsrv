using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib
{
    public class DropChinagPayment : Drop
    {


        public string SignMethod { get; set; }

        public string Signature { get; set; }

        public string Version { get; set; }

        public string TxnType { get; set; }

        public string TxnSubType { get; set; }

        public string BizType { get; set; }

        public string AccessType { get; set; }

        public string AccessMode { get; set; }

        public string MerId { get; set; }

        public string MerOrderId { get; set; }

        public string TxnTime { get; set; }

        public string TxnAmt { get; set; }

        public string Currency { get; set; }

        public string FrontUrl { get; set; }

        public string BackUrl { get; set; }

        public string PayType { get; set; }

        /// <summary>
        /// 支付地址
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
