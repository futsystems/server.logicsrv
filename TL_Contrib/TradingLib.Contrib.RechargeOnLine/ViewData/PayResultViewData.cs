using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.RechargeOnLine
{
    public class PayResultViewData : Drop
    {

        /// <summary>
        /// 支付结果 成功 失败
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 对应的 操作编号
        /// </summary>
        public string OperationRef { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }
    }
}
