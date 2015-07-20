using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /// <summary>
    /// CTP交易通道信息
    /// 期货公司BrokerID 交易前置地址
    /// </summary>
    public class CTPBrokerInfo
    {
        /// <summary>
        /// 数据库ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 期货经纪公司名称
        /// </summary>
        public string BrokerName { get; set; }


        /// <summary>
        /// 交易IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 交易端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 经纪编号
        /// </summary>
        public int BrokerID { get; set; }

        /// <summary>
        /// 域ID
        /// </summary>
        public int Domain_ID { get; set; }
    }
}
