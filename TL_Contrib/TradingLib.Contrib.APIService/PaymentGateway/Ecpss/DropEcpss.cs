using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.Ecpss
{
    public class DropEcpssPayment : Drop
    {

        public string MerNo { get; set; }

        public string BillNo { get; set; }

        public string VAmount { get; set; }

        public string ReturnURL { get; set; }

        public string AdviceURL { get; set; }

        public string SignInfo { get; set; }

        public string orderTime { get; set; }

        public string defaultBankNumber { get; set; }

        public string Remark { get; set; }

        public string products { get; set; }



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
