using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.Payment.YeePay
{
    public class DropYeePayPayment : Drop
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

        public string p0_Cmd { get; set; }

        public string p1_MerId { get; set; }

        public string p2_Order { get; set; }

        public string p3_Amt { get; set; }

        public string p4_Cur { get; set; }

        public string p5_Pid { get; set; }

        public string p6_Pcat { get; set; }

        public string p7_Pdesc { get; set; }

        public string p8_Url { get; set; }

        public string p9_SAF { get; set; }

        public string pa_MP { get; set; }

        public string pb_ServerNotifyUrl { get; set; }

        public string pd_FrpId { get; set; }

        public string pm_Period { get; set; }

        public string pn_Unit { get; set; }

        public string pr_NeedResponse { get; set; }

        public string hmac { get; set; }
    }
}
