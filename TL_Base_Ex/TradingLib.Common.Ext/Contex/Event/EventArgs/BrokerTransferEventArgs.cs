using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    public class BrokerTransferEventArgs : EventArgs
    {

        /// <summary>
        /// 通道编号
        /// </summary>
        public string BrokerToken { get; set; }

        /// <summary>
        /// 出金或入金
        /// </summary>
        public QSEnumCashOperation TransType { get; set; }
        /// <summary>
        /// 错误ID
        /// </summary>
        public int ErrorID { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
    }
}
