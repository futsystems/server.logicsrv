using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;


namespace APIClient
{
    partial class Form1
    {


        public string SendPostHttpRequest(Dictionary<string, string> requestData)
        {
            //string url = string.Format("http://127.0.0.1:8080/api/{0}/", action);
            string url = "http://127.0.0.1:8080/api/";

            StringBuilder sb = new StringBuilder();
            foreach (var i in requestData.Keys)
            {
                if (sb.Length != 0)
                    sb.Append("&");
                sb.AppendFormat("{0}={1}", i, requestData[i]);
            }
            WebRequest request = (WebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            byte[] postBytes = null;
            postBytes = Encoding.UTF8.GetBytes(sb.ToString());
            request.ContentType = "application/x-www-form-urlencoded; encoding=utf-8";
            request.ContentLength = postBytes.Length;
            using (Stream outstream = request.GetRequestStream())
            {
                outstream.Write(postBytes, 0, postBytes.Length);
            }
            string result = string.Empty;
            using (WebResponse response = request.GetResponse())
            {
                if (response != null)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        result = reader.ReadToEnd();
                    }

                }
            }
            return result;
        }

        void WireEvent_Web()
        {
            webAddAccount.Click += new EventHandler(webAddAccount_Click);
        }

        public static string MD5Sign(string strToBeEncrypt, string md5Key)
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


        void webAddAccount_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            
            request.Add("method", "add_user");
            request.Add("domain_id", web_domainId.Text);
            request.Add("user_id",web_userID.Text);
            request.Add("agent_id",web_agentID.Text);
            request.Add("currency", web_currency.Text);
            

            string waitSign = CreateLinkString(request);
            var sign = MD5Sign(waitSign, md5key.Text);
            request.Add("md5sign", sign);

            string ret = SendPostHttpRequest(request);
            logger.Info("request:" + JsonConvert.SerializeObject(request));
            logger.Info("resoult:" + ret);
        }
    }
}
