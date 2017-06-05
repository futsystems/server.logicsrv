using System.Collections.Generic;
using System.Text;
using System;
using System.Web;
using System.IO;
using TradingLib.Contrib.APIService;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ielpm_merchant_code_demo.com.ielpm.merchant.code.sdk
{
    public class IELPMUtil
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="dataStr"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static string Sign(SortedDictionary<string, string> data, Encoding encoding)
        {
            //将Dictionary信息转换成key1=value1&key2=value2的形式
            string stringData = CreateLinkString(data, true, false);

            string stringSign = null;

            byte[] byteSign = SecurityUtil.SignBySoft(CertUtil.GetSignProviderFromPfx(), encoding.GetBytes(stringData));

            stringSign = Convert.ToBase64String(byteSign);

            return stringSign;
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static bool Validate(SortedDictionary<string, string> data, Encoding encoding)
        {
            //获取签名
            string signValue = data["sign"];
            byte[] signByte = Convert.FromBase64String(signValue);
            data.Remove("sign");
            string stringData = CreateLinkString(data, true, false);
            RSACryptoServiceProvider provider = CertUtil.GetValidateProviderFromPath();
            if (null == provider)
            {
                return false;
            }
            return SecurityUtil.ValidateBySoft(provider, signByte, encoding.GetBytes(stringData));
        }

        /// <summary>
        /// 将字符串key1=value1&key2=value2转换为Dictionary数据结构
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CoverStringToDictionary(string data)
        {
            if (null == data || 0 == data.Length)
            {
                return null;
            }
            string[] arrray = data.Split(new char[] { '&' });
            Dictionary<string, string> res = new Dictionary<string, string>();
            foreach (string s in arrray)
            {
                int n = s.IndexOf("=");
                string key = s.Substring(0, n);
                string value = s.Substring(n + 1);
                Console.WriteLine(key + "=" + value);
                res.Add(key, value);
            }
            return res;
        }


        public static string CreateAutoSubmitForm(string url, SortedDictionary<string, string> data, Encoding encoding)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendFormat("<meta http-equiv=\"Content-Type\" content=\"text/html; charset={0}\" />", encoding);
            html.AppendLine("</head>");
            html.AppendLine("<body onload=\"OnLoadSubmit();\">");
            html.AppendFormat("<form id=\"pay_form\" action=\"{0}\" method=\"post\">", url);
            foreach (KeyValuePair<string, string> kvp in data)
            {
                html.AppendFormat("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\" />", kvp.Key, kvp.Value);
            }
            html.AppendLine("</form>");
            html.AppendLine("<script type=\"text/javascript\">");
            html.AppendLine("<!--");
            html.AppendLine("function OnLoadSubmit()");
            html.AppendLine("{");
            html.AppendLine("document.getElementById(\"pay_form\").submit();");
            html.AppendLine("}");
            html.AppendLine("//-->");
            html.AppendLine("</script>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }


        /// <summary>
        /// 把请求要素按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="para">请求要素</param>
        /// <param name="sort">是否需要根据key值作升序排列</param>
        /// <param name="encode">是否需要URL编码</param>
        /// <returns>拼接成的字符串</returns>
        public static String CreateLinkString(SortedDictionary<String, String> para, bool sort, bool encode)
        {

            StringBuilder sb = new StringBuilder();
            foreach (String key in para.Keys)
            {
                String value = para[key];
                if (encode && value != null)
                {
                    try
                    {
                        value = HttpUtils.UrlEncode(value,Encoding.UTF8);
                    }
                    catch (Exception ex)
                    {
                        return "#ERROR: HttpUtility.UrlEncode Error!" + ex.Message;
                    }
                }

                sb.Append(key).Append("=").Append(value).Append("&");

            }

            return sb.Remove(sb.Length - 1, 1).ToString();
        }



        /// <summary>
        /// pinblock 16进制计算
        /// </summary>
        /// <param name="encoder"></param>
        /// <returns></returns>

        public static string PrintHexString(byte[] b)
        {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < b.Length; i++)
            {
                string hex = Convert.ToString(b[i] & 0xFF, 16);
                if (hex.Length == 1)
                {
                    hex = '0' + hex;
                }
                sb.Append("0x");
                sb.Append(hex + " ");
            }
            sb.Append("");
            return sb.ToString();
        }



        /// <summary>
        /// 数据加密
        /// </summary>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static string EncryptData(string data, Encoding encoding)
        {

            if (null == data || "".Equals(data))
            {
                return "";
            }

            return Convert.ToBase64String(SecurityUtil.encryptedData(encoding.GetBytes(data)));
        }



        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="dataString">原字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>解密结果</returns>
        public static string DecryptData(string dataString, Encoding encoding)
        {
            /** 使用公钥对数据加密 **/
            byte[] data = null;
            try
            {
                data = Convert.FromBase64String(dataString);
                RSACryptoServiceProvider p = CertUtil.GetSignProviderFromPfx();
                data = p.Decrypt(data, false);
                return encoding.GetString(data);
            }
            catch (Exception e)
            {
                //TODO 请改为讲异常输出日志
                Console.WriteLine(e.Message);
                return "";
            }
        }

    }
}