using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.ETone
{
    public class DropETonePayment : Drop
    {
        public string version { get; set; }

        public string transCode { get; set; }

        public string merchantId { get; set; }

        public string merOrderNum { get; set; }

        public string bussId { get; set; }

        public string tranAmt { get; set; }

        public string sysTraceNum { get; set; }

        public string tranDateTime { get; set; }

        public string currencyType { get; set; }

        public string merURL { get; set; }

        public string orderInfo { get; set; }

        //public string bankId { get; set; }

        //public string stlmId { get; set; }

        public string entryType { get; set; }

        //public string userId { get; set; }

        //public string userIp { get; set; }

        public string backURL { get; set; }

        //public string reserver1 { get; set; }

        //public string reserver2 { get; set; }

        //public string reserver3 { get; set; }

        //public string reserver4 { get; set; }

        public string signValue { get; set; }

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
