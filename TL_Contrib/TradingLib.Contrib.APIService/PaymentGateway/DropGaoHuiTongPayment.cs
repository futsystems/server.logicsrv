﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.GaoHuiTong
{
    public class DropGaoHuiTongPayment : Drop
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


        public string serviceName { get; set; }

        public string version { get; set; }

        public string platform { get; set; }

        public string merchantId { get; set; }

        public string sign { get; set; }

        public string signType { get; set; }

        public string payType { get; set; }

        public string charset { get; set; }

        public string merOrderId { get; set; }

        public string currency { get; set; }

        public string notifyUrl { get; set; }

        public string returnUrl { get; set; }

        public string productName { get; set; }

        public string productDesc { get; set; }

        public string tranAmt { get; set; }

        public string tranTime { get; set; }

        public string bankCardType { get; set; }

        public string bankCode { get; set; }

        public string clientIp { get; set; }

        public string payerId { get; set; }
    }
}
