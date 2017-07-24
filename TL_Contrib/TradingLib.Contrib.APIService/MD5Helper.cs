using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;


namespace TradingLib.Contrib.APIService
{
    public class MD5Helper
    {
        public static string MD5Sign(string strToBeEncrypt,string md5Key)
        {
            string strSrc = strToBeEncrypt + md5Key;

            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] FromData = System.Text.Encoding.GetEncoding("utf-8").GetBytes(strSrc);
            Byte[] TargetData = md5.ComputeHash(FromData);
            string Byte2String = "";
            for (int i = 0; i < TargetData.Length; i++)
            {
                Byte2String += TargetData[i].ToString("x2");
            }
            return Byte2String.ToLower();
        }


        /// <summary>
        /// 1.除去数组中的空值和签名参数并以字母a到z的顺序排序
        /// 2.把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        public static string CreateLinkString(Dictionary<string, string> dicArrayPre)
        {
            SortedDictionary<string, string> dicArray = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                if (temp.Key.ToLower() != "md5sign" && !string.IsNullOrEmpty(temp.Value))
                {
                    dicArray.Add(temp.Key, temp.Value);
                }
            }

            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }


    }
}
