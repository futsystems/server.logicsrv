using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace TradingLib.Contrib.Payment.ETone
{
    public class ETonePayUtils
    {

        public static string StringToHexString(string s)
        {
            Encoding encode = Encoding.UTF8;
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
            {
                result += Convert.ToString(b[i], 16);
            }
            return result;
        }

        /// 16进制转换成字符串 
        public static string HexStringToString(string hs, Encoding encode)
        {
            //以%分割字符串，并去掉空字符
            string[] chars = hs.Split(new char[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] b = new byte[chars.Length];
            //逐个字符变为16进制字节数据
            for (int i = 0; i < chars.Length; i++)
            {
                b[i] = Convert.ToByte(chars[i], 16);
            }
            //按照指定编码将字节数组变为字符串
            return encode.GetString(b);
        }

        public static string StrToHex(string mStr) //返回处理后的十六进制字符串 
        {
            return BitConverter.ToString(
            ASCIIEncoding.Default.GetBytes(mStr)).Replace("-", " ");
        } /* StrToHex */

        public static string HexToStr(string mHex) // 返回十六进制代表的字符串 
        {
            mHex = mHex.Replace(" ", "");
            if (mHex.Length <= 0) return "";
            byte[] vBytes = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
                if (!byte.TryParse(mHex.Substring(i, 2), NumberStyles.HexNumber, null, out vBytes[i / 2]))
                    vBytes[i / 2] = 0;
            return ASCIIEncoding.Default.GetString(vBytes);
        } /* HexToStr */

        /// <summary>
        /// get the md5 hash of a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string md5(string str)
        {
            if (str == null)
            {
                return null;
            }
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            System.Text.Encoding e = System.Text.Encoding.GetEncoding("UTF-8");
            byte[] fromData = e.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                string strHex = targetData[i].ToString("X2").ToLower();
                byte2String += strHex;
            }
            return byte2String;
        }

        //解码
        public static Tuple<string, IEnumerable<KeyValuePair<string, string>>> UrlToData(string url)
        {

            if (url == null)

                throw new ArgumentNullException("url");



            url = url.Trim();

            try
            {

                var split = url.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 1)

                    return new Tuple<string, IEnumerable<KeyValuePair<string, string>>>(url, null);



                //获取前面的URL地址

                var host = split[0];



                var pairs = split.Select(s =>
                {

                    //没有用String.Split防止某些少见Query String中出现多个=，要把后面的无法处理的=全部显示出来

                    var idx = s.IndexOf('=');

                    return new KeyValuePair<string, string>(Uri.UnescapeDataString(s.Substring(0, idx)), Uri.UnescapeDataString(s.Substring(idx + 1)));

                }).ToList();



                return new Tuple<string, IEnumerable<KeyValuePair<string, string>>>(host, pairs);

            }

            catch (Exception ex)
            {

                throw new FormatException("URL格式错误", ex);

            }

        }

    }
}
