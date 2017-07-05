using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;

namespace TradingLib.Contrib.Payment.FZPay
{
    public class FZPayHelper
    {
        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="half">为真则为16位,否则32位</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str, bool half)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            if (half)
            {
                ret = ret.Substring(8, 16);
            }
            return ret;
        }
    }
}