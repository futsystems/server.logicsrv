using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib
{
    public class DropAliPayPayment : Drop
    {


        /// <summary>
        /// 合作商户
        /// </summary>
        public string Partner { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string InputCharset { get; set; }

        /// <summary>
        /// 服务方式
        /// </summary>
        public string Service { get;set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// 服务端通知URL
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 客户跳转URL
        /// </summary>
        public string ReturnUrl { get; set; }


        /// <summary>
        /// 商户订单编号
        /// </summary>
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 订单标题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string TotalFee { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string SellerID { get; set; }

        public string Sign { get; set; }


        public string SignType { get; set; }

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
