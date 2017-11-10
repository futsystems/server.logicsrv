using Common.Logging;
using DotLiquid;
using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;

namespace TradingLib.Contrib.Payment.HaiFu
{
    public class HaiFuGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("HaiFuGateWay");

        public HaiFuGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.HaiFu;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.AppKey = data["AppKey"].ToString();
            this.AppSecret = data["AppSecret"].ToString();
        }



        string PayUrl = "http://haifu.neargh.com:9091/paying/lovepay/getQr";
        string AppKey = "ccb8acfdfaabd9682ae6e0ee36920171";
        string AppSecret = "f47cb702f021526c3ec0bd5bd2c25b5213a30956";

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropHaiFuPayment data = new DropHaiFuPayment();
            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);
            data.Account = operatioin.Account;

            data.body = "充值";
            data.total_fee = ((int)(operatioin.Amount * 100)).ToString();
            data.product_id = operatioin.Ref;
            data.goods_tag = "1";
            data.op_user_id = AppKey;
            data.nonce_str = operatioin.Ref;
            data.spbill_create_ip = "127.0.0.1";

            data.front_notify_url = APIGlobal.CustNotifyUrl + "/haifu";
            data.notify_url = APIGlobal.SrvNotifyUrl + "/haifu";
            data.wx_app_id = string.Empty;
            data.sub_openid = string.Empty;
            data.pay_type = "5";
            data.bank_id = ConvBankCode(operatioin.Bank);


            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            dic.Add("body", data.body);
            dic.Add("total_fee", data.total_fee);
            dic.Add("product_id", data.product_id);
            dic.Add("goods_tag", data.goods_tag);
            dic.Add("op_user_id", data.op_user_id);
            dic.Add("nonce_str", data.nonce_str);
            dic.Add("spbill_create_ip", data.spbill_create_ip);
            dic.Add("front_notify_url", data.front_notify_url);
            dic.Add("notify_url", data.notify_url);
            //dic.Add("wx_app_id", data.wx_app_id);
            //dic.Add("sub_openid", data.sub_openid);
            dic.Add("pay_type", data.pay_type);
            dic.Add("bank_id", data.bank_id);

            var rawStr = CreateLinkString(dic);

            //data.sign = MD5Sign(rawStr, this.AppSecret);
            data.sign = SHA1(rawStr + this.AppSecret);

            dic.Add("sign", data.sign);
            var jstr = dic.SerializeObject();
            var resp = SendPostHttpRequest2(this.PayUrl, jstr);
            var respdata = resp.DeserializeObject();
            try
            {
                data.link = respdata["code_url"].ToString();
            }
            catch (Exception ex)
            {

            }
            return data;
        }

        string ConvBankCode(string stdCode)
        {
            switch (stdCode)
            {
                //工商银行
                case "01020000": return "10001";
                //农业银行
                case "01030000": return "10003";
                //建设银行
                case "01050000": return "10004";
                //中国银行
                case "01040000": return "10008";
                //招商
                case "03080000": return "10002";
                //交通
                case "03010000": return "10005";
                //邮政
                case "01000000": return "10012";
                //中信银行
                case "03020000": return "10010";
                //光大
                case "03030000": return "10013";
                //民生
                case "03050000": return "10007";
                default:
                    return "10001";

            }
        }


        public static CashOperation GetCashOperation(NHttp.HttpRequest request)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = string.Empty;
            if (request.RequestType.ToUpper() == "POST")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                var recvStr = Encoding.UTF8.GetString(data);
                var d = recvStr.DeserializeObject();
                transid = d["productId"].ToString();
            }
            else
            {
                transid = request.QueryString["productId"];
            }
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var Request = request.Params;

            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            string transid = string.Empty;
            if (request.RequestType.ToUpper() == "POST")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                var recvStr = Encoding.UTF8.GetString(data);
                var d = recvStr.DeserializeObject();
                transid = d["errcode"].ToString();
            }
            else
            {
                transid = request.QueryString["errcode"];
            }
            return transid == "200";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            string transid = string.Empty;
            if (request.RequestType.ToUpper() == "POST")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                var recvStr = Encoding.UTF8.GetString(data);
                var d = recvStr.DeserializeObject();
                transid = d["errcode"].ToString();
            }
            else
            {
                transid = request.QueryString["errcode"];
            }
            return transid == "200" ? "支付成功" : "支付失败";
        }

        public static string SHA1(string content)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = Encoding.UTF8.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }  

        public static string MD5Sign(string strToBeEncrypt, string appSecret)
        {
            string strSrc = strToBeEncrypt + appSecret;

            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] FromData = System.Text.Encoding.GetEncoding("utf-8").GetBytes(strSrc);
            Byte[] TargetData = md5.ComputeHash(FromData);
            string Byte2String = "";
            for (int i = 0; i < TargetData.Length; i++)
            {
                Byte2String += TargetData[i].ToString("x2");
            }
            return Byte2String.ToUpper();
        }


        /// <summary>
        /// 除去数组中的空值和签名参数并以字母a到z的顺序排序
        /// </summary>
        /// <param name="dicArrayPre">过滤前的参数组</param>
        /// <returns>过滤后的参数组</returns>
        public static Dictionary<string, string> FilterPara(SortedDictionary<string, string> dicArrayPre)
        {
            Dictionary<string, string> dicArray = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                if (temp.Key.ToLower() != "sign" && temp.Key.ToLower() != "sign_type" && temp.Value != "" && temp.Value != null)
                {
                    dicArray.Add(temp.Key, temp.Value);
                }
            }

            return dicArray;
        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string CreateLinkString(IDictionary<string, string> dicArray)
        {
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


        public string SendPostHttpRequest(string url, Dictionary<string, string> requestData)
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
            logger.Info("Request:" + sb.ToString());

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
            Console.WriteLine(result);
            return result;
        }

        public string SendPostHttpRequest2(string url, string jsonStr)
        {
            logger.Info(string.Format("send request:{0} url:{1}", jsonStr, url));
            string result = string.Empty;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonStr);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            Console.WriteLine(result);
            return result;
        }
    }
}
