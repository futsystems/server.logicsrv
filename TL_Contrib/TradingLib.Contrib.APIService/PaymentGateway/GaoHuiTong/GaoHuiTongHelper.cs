using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.Payment.GaoHuiTong
{
    public class GaoHuiTongHelper
    {
        public static string MD5Key { get; set; }

        public static string PublicKey { get; set; }

        public static string PrivateKey { get; set; }

        /// <summary>
        /// 除去数组中的空值和签名参数并以字母a到z的顺序排序
        /// </summary>
        /// <param name="dicArrayPre">过滤前的参数组</param>
        /// <returns>过滤后的参数组</returns>
        public static Dictionary<string, string> FilterPara(Dictionary<string, string> dicArrayPre)
        {
            Dictionary<string, string> dicArray = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                //if (temp.Key.ToLower() != "sign" && temp.Key.ToLower() != "signtype" && temp.Value != "" && temp.Value != null)
                //{
                //    dicArray.Add(temp.Key, temp.Value);
                //}
                string key = temp.Key;
                string value = temp.Value;
                if (value == null && value == "")
                {
                    continue;
                }
                dicArray.Add(key, value);
            }

            return dicArray;

        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string CreateLinkString1(Dictionary<string, string> dicArray)
        {
            Dictionary<string, string> sortedDic = SortDictionaryAsc(dicArray);
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in sortedDic)
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");

                //prestr.Append(temp.Key + "=" + HttpUtility.UrlEncode(temp.Value,Encoding.UTF8) + "&");
            }

            //去掉最后一个&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }
        public static string CreateLinkString(Dictionary<string, string> para)
        {
            //for循环遍历
            List<string> test = new List<string>(para.Keys);

            var ascList = test.OrderBy(o => o);
            var ascList1 = (from l in test
                            orderby l ascending
                            select l).ToList();

            string prestr = "";
            for (int i = 0; i < para.Count; i++)
            {
                string value = para[ascList1[i]];
                if (value != null && value != "")
                {
                    if (i == para.Count - 1)//拼接时，不包括最后一个&字符
                    {
                        prestr = prestr + ascList1[i] + "=" + para[ascList1[i]];
                    }
                    else
                    {
                        prestr = prestr + ascList1[i] + "=" + para[ascList1[i]] + "&";
                    }
                }
            }
            return prestr;
        }

        protected static Dictionary<string, string> SortDictionaryAsc(Dictionary<string, string> dic)
        {


            List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>(dic);
            myList.Sort(delegate(KeyValuePair<string, string> s1, KeyValuePair<string, string> s2)
            {
                return s1.Key.CompareTo(s2.Key);
            });

            var dicSort = from objDic in dic orderby objDic.Key ascending select objDic;
            

            string[] tArray = new string[dic.Keys.Count];
            dic.Keys.CopyTo(tArray, 0);

            Array.Sort(tArray);

            dic.Clear();
            foreach (KeyValuePair<string, string> pair in myList)
            {
                dic.Add(pair.Key, pair.Value);
            }

            return dic;
        }

        //验签测试方法
        //toSignStr
        //signedData
        public static bool VerifyMD5(string toSignStr, string signedData)
        {
            string contentSignStr = MD5sign(toSignStr, MD5Key, "UTF-8");

            // 判断两边签名的数据是否一致
            if (contentSignStr == signedData)
                return true;
            else
                return false;
        }
        /// 生成签名结果
        /// </summary>
        /// <param name="sArray">要签名的数组</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>签名结果字符串</returns>
        public static string BuildMysignRSA(Dictionary<string, string> dicArray, string _input_charset)
        {
            string prestr = CreateLinkString(dicArray);  //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串

            String mysign = "";

            mysign = RSAUtil.sign(prestr, PrivateKey, _input_charset);

            return mysign;
        }
        /// 生成签名结果
        /// </summary>
        /// <param name="sArray">要签名的数组</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>签名结果字符串</returns>
        public static string BuildMysignMD5(Dictionary<string, string> dicArray, string _input_charset)
        {
            string prestr = CreateLinkString(dicArray);  //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            String mysign = "";
            mysign = MD5sign(prestr, MD5Key, _input_charset);

            return mysign;
        }



        //验签测试方法
        //toSignStr
        //signedDataB64
        public static bool VerifyRSA(string toSignStr, string signedDataB64, string input_charset)
        {
            bool result = false;
            result = RSAUtil.verify(toSignStr, signedDataB64, PublicKey, input_charset);

            return result;
        }

        #region MD5
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="content">待签名字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>签名后字符串</returns>
        public static string MD5sign(string content, string Key, string input_charset)
        {

            string text = content + Key;

            //将要加密的字符串转化为byte类型
            byte[] data = System.Text.Encoding.UTF8.GetBytes(text.ToArray());
            //创建一个Md5对象
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            //加密Byte[]数组
            byte[] result = md5.ComputeHash(data);

            //将加密后的数组转化为字段
            string sResult = System.Text.Encoding.Unicode.GetString(result);


            return new String(encodeHex(result));
        }


        /// <summary> 
        /// 字节数组转16进制字符串 
        /// </summary> 
        /// <param name="bytes"></param> 
        /// <returns></returns> 
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /**
       * Used building output as Hex
       */
        private static char[] DIGITS = { '0', '1', '2', '3', '4', '5', '6',
                                         '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        public static char[] encodeHex(byte[] data)
        {

            int l = data.Length;

            char[] outc = new char[l << 1];

            // two characters form the hex value.
            for (int i = 0, j = 0; i < l; i++)
            {
                outc[j++] = DIGITS[(0xF0 & data[i]) >> 4];
                outc[j++] = DIGITS[0x0F & data[i]];
            }

            return outc;
        }

        #endregion
    }

}
