using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.APIService
{
    public class DropGoPayPayment:Drop
    {
        public string Version { get; set; }

        public string Charset { get; set; }

        public string Language { get; set; }

        public string SignType { get; set; }

        public string TranCode { get; set; }

        public string MerchantID { get; set; }

        public string MerOrderNum { get; set; }

        public string TranAmt { get; set; }

        public string CurrencyType { get; set; }

        public string FrontMerUrl { get; set; }

        public string BackgroundMerUrl { get; set; }

        public string TranDateTime { get; set; }

        public string VirCardNoIn { get; set; }

        public string TranIP { get; set; }

        public string SignValue { get; set; }


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
