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
            this.InterfaceVersion = "4.0";
            this.KeyType = "1";
            this.Md5Key = "abcdefg";
        }
        public string PayURL { get; set; }

        public string MemberID { get; set; }

        public string TerminalID { get; set; }

        public string InterfaceVersion { get; set; }

        public string KeyType { get; set; }

        public string Md5Key { get; set; }
    }
}
