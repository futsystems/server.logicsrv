using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class BankCardInfo
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public string BankID { get; set; }

        /// <summary>
        /// 开户行
        /// </summary>
        public string BankBrch { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string CertCode { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string MobilePhone { get; set; }


        public static string Serialize(BankCardInfo info)
        {
            if(info==null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(info.Name);
            sb.Append(d);
            sb.Append(info.BankID);
            sb.Append(d);
            sb.Append(info.BankBrch);
            sb.Append(d);
            sb.Append(info.BankAccount);
            sb.Append(d);
            sb.Append(info.CertCode);
            sb.Append(d);
            sb.Append(info.MobilePhone);
            return sb.ToString();
        }

        public static BankCardInfo Deserialize(string content)
        {
            if (string.IsNullOrEmpty(content)) return null;
            string[] rec = content.Split(',');
            BankCardInfo info = new BankCardInfo();
            info.Name = rec[0];
            info.BankID = rec[1];
            info.BankBrch = rec[2];
            info.BankAccount = rec[3];
            info.CertCode = rec[4];
            info.MobilePhone = rec[5];
            return info;
        }
    }
}
