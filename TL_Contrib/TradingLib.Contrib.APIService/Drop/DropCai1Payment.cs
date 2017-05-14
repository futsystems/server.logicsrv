using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.APIService
{
    public class DropCai1Payment:Drop
    {
        public string MerCode { get; set; }

        public string MerOrderNo { get; set; }

        public string OrderAmount { get; set; }

        public string OrderDate { get; set; }

        public string Currency { get; set; }

        public string GatewayType { get; set; }

        public string Language { get; set; }

        public string ReturnUrl { get; set; }

        public string Attach { get; set; }

        public string OrderEncodeType { get; set; }

        public string RetEncodeType { get; set; }

        public string RetType { get; set; }

        public string ServerUrl { get; set; }

        public string Sign { get; set; }

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
