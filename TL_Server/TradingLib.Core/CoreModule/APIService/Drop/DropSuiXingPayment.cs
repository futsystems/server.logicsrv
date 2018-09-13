using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib
{
    public class DropSuiXingPayment:Drop
    {
        public string mercNo { get; set; }

        public string tranCd { get; set; }

        public string version { get; set; }

        public string reqData { get; set; }

        public string ip { get; set; }

        public string encodeType { get; set; }

        public string sign { get; set; }

        public string type { get; set; }

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
