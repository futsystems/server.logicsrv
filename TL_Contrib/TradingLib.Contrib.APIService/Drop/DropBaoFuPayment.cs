using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.APIService
{
    /// <summary>
    /// 用于填充宝付通道的支付数据
    /// </summary>
    public class DropBaoFuPayment:Drop
    {
        /// <summary>
        /// 商户编号
        /// </summary>
        public string  MemberID {get;set;}

        /// <summary>
        /// 终端编号
        /// </summary>
        public string TerminalID { get; set; }

        /// <summary>
        /// 版本编号
        /// </summary>
        public string InterfaceVersion { get; set; }

        public string KeyType { get; set; }

        public string PayID { get; set; }

        public string TradeDate { get; set; }

        public string TransID { get; set; }

        public string OrderMoney { get; set; }

        public string NoticeType { get; set; }

        public string PageUrl { get; set; }

        public string ReturnUrl { get; set; }

        public string Signature { get; set; }

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
