using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.APIService
{
    public class DropMoBoPayment:Drop
    {

        public string apiName { get; set; }

        public string apiVersion { get; set; }

        public string platformID { get; set; }

        public string merchNo { get; set; }

        public string orderNo { get; set; }

        public string tradeDate { get; set; }

        public string amt { get; set; }

        public string merchUrl { get; set; }

        public string signMsg { get; set; }

        public string PayUrl { get; set; }

        public string tradeSummary { get; set; }
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
