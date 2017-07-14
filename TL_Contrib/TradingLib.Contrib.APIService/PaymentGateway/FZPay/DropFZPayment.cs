﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.FZPay
{
    public class DropFZPayment : Drop
    {

        public string partner { get; set; }

        public string banktype { get; set; }

        public string paymoney { get; set; }

        public string ordernumber { get; set; }

        public string callbackurl { get; set; }

        public string hrefbackurl { get; set; }

        public string attach { get; set; }

        public string sign { get; set; }


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
    }
}