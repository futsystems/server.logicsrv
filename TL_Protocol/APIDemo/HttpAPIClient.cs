using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Security.Cryptography;


namespace APIClient
{
    public class HttpAPIClient
    {
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


        public static string SendPostHttpRequest(Dictionary<string, string> requestData,string url)
        {
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
            postBytes = Encoding.ASCII.GetBytes(sb.ToString());
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


        string _url = string.Empty;
        string _key = string.Empty;

        public HttpAPIClient(string url, string key)
        {
            _url = url;
            _key = key;
        }

        /// <summary>
        /// 创建交易账户
        /// </summary>
        /// <param name="domain_id">业务分区编号</param>
        /// <param name="user_id">用户ID</param>
        /// <param name="agent_id">代理编号</param>
        /// <param name="currency">帐号基础货币</param>
        /// <returns></returns>
        public string ReqAddAccount(string domain_id,string user_id,string agent_id,string currency)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();

            request.Add("method", "add_user");
            request.Add("domain_id", domain_id);
            request.Add("user_id", user_id);
            request.Add("agent_id", agent_id);
            request.Add("currency", currency);

            string waitSign = CreateLinkString(request);
            var sign = MD5Sign(waitSign, _key);
            request.Add("md5sign", sign);

            string ret = SendPostHttpRequest(request,_url);

            return ret;
        }

        public string ReqUpdateAccount(string domain_id,string account, string name, string qq, string mobile, string idcard, string bank, string branch, string bankac)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();

            request.Add("method", "update_user");
            request.Add("domain_id", domain_id);
            request.Add("account", account);
            request.Add("name", name);
            request.Add("qq", qq);
            request.Add("mobile", mobile);
            request.Add("idcard", idcard);
            request.Add("bank", bank);
            request.Add("branch", branch);
            request.Add("bankac", bankac);


            string waitSign = CreateLinkString(request);
            var sign = MD5Sign(waitSign, _key);
            request.Add("md5sign", sign);

            request["name"] = UrlEncode(request["name"]);
            request["branch"] = UrlEncode(request["branch"]);
            string ret = SendPostHttpRequest(request, _url);

            return ret;
        }

        public string ReqDeposit(string domain_id, string account, string val)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("method", "deposit");
            request.Add("domain_id", domain_id);
            request.Add("account", account);
            request.Add("amount", val);

            string waitSign = CreateLinkString(request);
            var sign = MD5Sign(waitSign, _key);
            request.Add("md5sign", sign);
            string ret = SendPostHttpRequest(request, _url);
            return ret;
        }

        public string ReqQueryUser(string domain_id, string account)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("method", "qry_account");
            request.Add("domain_id", domain_id);
            request.Add("account", account);

            string waitSign = CreateLinkString(request);
            var sign = MD5Sign(waitSign, _key);
            request.Add("md5sign", sign);
            string ret = SendPostHttpRequest(request, _url);
            return ret;
        }

        public string ReqQueryPass(string domain_id, string account)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("method", "qry_pass");
            request.Add("domain_id", domain_id);
            request.Add("account", account);

            string waitSign = CreateLinkString(request);
            var sign = MD5Sign(waitSign, _key);
            request.Add("md5sign", sign);
            string ret = SendPostHttpRequest(request, _url);
            return ret;
        }

        public string ReqUpdatePass(string domain_id, string account,string pass)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("method", "update_pass");
            request.Add("domain_id", domain_id);
            request.Add("account", account);
            request.Add("pass", pass);

            string waitSign = CreateLinkString(request);
            var sign = MD5Sign(waitSign, _key);
            request.Add("md5sign", sign);
            string ret = SendPostHttpRequest(request, _url);
            return ret;
        }


        public string ReqWithdraw(string domain_id, string account, string val)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("method", "withdraw");
            request.Add("domain_id", domain_id);
            request.Add("account", account);
            request.Add("amount", val);

            string waitSign = CreateLinkString(request);
            var sign = MD5Sign(waitSign, _key);
            request.Add("md5sign", sign);
            string ret = SendPostHttpRequest(request, _url);
            return ret;
        }


        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }

    }
}
