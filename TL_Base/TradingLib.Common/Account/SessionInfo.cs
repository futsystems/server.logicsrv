using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class SessionInfo
    {
        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 前置编号
        /// </summary>
        public string FrontID { get; set; }

        /// <summary>
        /// 客户端编号
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 终端
        /// </summary>
        public string ProductInfo { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 物理位置
        /// </summary>
        public string Geography { get; set; }

    }
}
