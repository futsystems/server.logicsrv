using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.RMTech
{
    public class DropRMTechPayment : Drop
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

        public string tradeType { get; set; }
        public string version { get; set; }
        public string channel { get; set; }
        public string mchNo { get; set; }
        public string sign { get; set; }
        public string body { get; set; }
        public string mchOrderNo { get; set; }
        public double oamount { get; set; }
        public double decAmount { get; set; }
        public string description { get; set; }
        public string currency { get; set; }
        public string timePaid { get; set; }
        public string timeExpire { get; set; }
        public string timeSettle { get; set; }
        public string subject { get; set; }
        public string extra { get; set; }
        public string innerOrderNo { get; set; }
        public string virAccNo { get; set; }
        public double oamount2 { get; set; }
        public int freezeDays { get; set; }
    }
}
