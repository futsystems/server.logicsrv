using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace TradingLib.Contrib.APIService
{
    public class RequestCheck
    {

        /// <summary>
        /// Md5加密字符串 小写
        /// </summary>
        /// <param name="strToBeEncrypt"></param>
        /// <returns></returns>
        public static string Md5Encrypt(string strToBeEncrypt)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] FromData = System.Text.Encoding.GetEncoding("utf-8").GetBytes(strToBeEncrypt);
            Byte[] TargetData = md5.ComputeHash(FromData);
            string Byte2String = "";
            for (int i = 0; i < TargetData.Length; i++)
            {
                Byte2String += TargetData[i].ToString("x2");
            }
            return Byte2String.ToLower();
        }
        SortedDictionary<string, string> paramsMap = new SortedDictionary<string, string>();

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void AddParams(string key, string val)
        {
            paramsMap.Add(key, val);
        }

        const string MARK = "~|~";//分隔符
        /// <summary>
        /// 
        /// </summary>
        /// <param name="md5key"></param>
        public string  GetMd5Sign(string md5key)
        {
            string _WaitSign = string.Empty;
            foreach (var v in paramsMap)
            {
                _WaitSign += v.Key + "=" + v.Value + MARK;
            }
            _WaitSign = _WaitSign+"md5Key=" + md5key;

            return Md5Encrypt(_WaitSign);
        }
    }
}
