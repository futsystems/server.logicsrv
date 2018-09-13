using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.APIService
{
    public class DropIelpmPayment:Drop
    {
        public string merchantNo { get; set; }

        public string version { get; set; }

        public string channelNo { get; set; }

        public string tranSerialNum { get; set; }

        public string tranTime { get; set; }

        public string currency { get; set; }

        public string amt { get; set; }

        public string bizType { get; set; }

        public string goodsName { get; set;}

        public string notifyUrl { get; set; }

        public string returnUrl {get;set;}


        public string buyerName { get; set; }

        public string buyerId { get; set; }

        public string ip { get; set; }

        public string sign { get; set; }

        public string goodsInfo { get; set; }

        public string remark { get; set; }

        public string contact { get; set; }

        public string goodsNum { get; set; }

        public string YUL1 { get; set; }

        public string referer { get; set; }


        public string valid { get; set; }

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
