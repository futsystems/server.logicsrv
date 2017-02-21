using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.APIService
{
    public class PaymentGateWaySetting
    {
        /// <summary>
        /// 分区编号
        /// </summary>
        public int Domain_ID { get; set; }

        /// <summary>
        /// 支付网关类别
        /// </summary>
        public QSEnumGateWayType GateWayType { get; set; }

        /// <summary>
        /// 支付网关设置参数
        /// </summary>
        public string GateWayConfig { get; set; }

    }
}
