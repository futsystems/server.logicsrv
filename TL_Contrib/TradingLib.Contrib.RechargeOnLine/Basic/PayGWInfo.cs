using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.RechargeOnLine
{
    /// <summary>
    /// 支付网关的基础信息
    /// 商户ID等相关数据
    /// </summary>
    public class PayGWInfo
    {
        public PayGWInfo()
        {
            this.PayURL = "http://vgw.baofoo.com/payindex";
            this.MemberID = "100000178";
            this.TerminalID = "10000001";
            this.Md5Key = "abcdefg";

            this.InterfaceVersion = "4.0";
            this.KeyType = "1";
            LocalURLInfo = null;
        }
        /// <summary>
        /// 本地访问信息
        /// </summary>
        public LocalURLInfo LocalURLInfo { get; set; }

        /// <summary>
        /// 支付网关地址
        /// </summary>
        public string PayURL { get; set; }

        /// <summary>
        /// 商户编号
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// 终端编号
        /// </summary>
        public string TerminalID { get; set; }

        /// <summary>
        /// 接口版本
        /// </summary>
        public string InterfaceVersion { get; set; }

        /// <summary>
        /// 加密类型
        /// </summary>
        public string KeyType { get; set; }

        /// <summary>
        /// MD5密钥
        /// </summary>
        public string Md5Key { get; set; }


        public string PageURL { get { return LocalURLInfo != null ? LocalURLInfo.PageURL : ""; } }

        public string NotifyURL { get { return LocalURLInfo != null ? LocalURLInfo.NotifyURL : ""; } }
    }
}
