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
using NHttp;
namespace TradingLib.Contrib.Payment.GYPay
{
    public class GYPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("GYPayGateWay");

        public GYPayGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.GYPay;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.MerID = data["MerID"].ToString();
            this.Key = data["Key"].ToString();
        }
        string PayUrl = "http://113.106.95.37:7777/gyprovider/netpay/applyPay.do";
        string MerID = "gypay0170113";
        string Key = "723c8942281340859cc9210ddb472225";

        public override Drop CreatePaymentDrop(CashOperation operatioin, HttpRequest request)
        {
            DropGYPayment data = new DropGYPayment();
            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);
            data.Account = operatioin.Account;

            data.gymchtId = this.MerID;
            data.tradeSn = operatioin.Ref;
            data.orderAmount = ((int)(operatioin.Amount * 100)).ToString();
            data.goodsName = "Credits";
            data.bankSegment = ConvBankCode(operatioin.Bank);
            data.cardType = "01";

            data.callbackUrl = APIGlobal.CustNotifyUrl + "/gypay";
            data.notifyUrl = APIGlobal.SrvNotifyUrl + "/gypay";
            data.expirySecond = "3600";
            data.channelType = "1";
            data.nonce = operatioin.Ref;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("gymchtId", data.gymchtId);
            dic.Add("tradeSn", data.tradeSn);
            dic.Add("orderAmount", data.orderAmount);
            dic.Add("goodsName", data.goodsName);
            dic.Add("bankSegment", data.bankSegment);
            dic.Add("cardType", data.cardType);
            dic.Add("callbackUrl", data.callbackUrl);
            dic.Add("notifyUrl", data.notifyUrl);
            dic.Add("expirySecond", data.expirySecond);
            dic.Add("channelType", data.channelType);
            dic.Add("nonce", data.nonce);

            var rawStr = CreateLinkString(dic);

            data.sign = MD5Sign(rawStr,this.Key);
            dic.Add("sign", data.sign);
            var jstr = dic.SerializeObject();

            var resp = SendPostHttpRequest2(this.PayUrl, dic);
            var respdata = resp.DeserializeObject();
            if (respdata["resultCode"].ToString() == "00000")
            {
                data.link = respdata["payUrl"].ToString();
            }
            return data;
        }

        string ConvBankCode(string stdCode)
        {
            switch (stdCode)
            {
                //工商银行
                case "01020000": return "1001";
                //农业银行
                case "01030000": return "1002";
                //建设银行
                case "01050000": return "1004";
                //中国银行
                case "01040000": return "1003";
                //招商
                case "03080000": return "1012";
                //交通
                case "03010000": return "1005";
                //邮政
                case "01000000": return "1006";
                //中信银行
                case "03020000": return "1007";
                //光大
                case "03030000": return "1008";
                //民生
                case "03050000": return "1010";
                default:
                    return "1001";

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
                transid = d["tradeSn"].ToString();
            }
            else
            {
                transid = request.QueryString["tradeSn"];
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
            string statusCode = string.Empty;
            if (request.RequestType.ToUpper() == "POST")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                var recvStr = Encoding.UTF8.GetString(data);
                var d = recvStr.DeserializeObject();
                statusCode = d["tradeState"].ToString();
            }
            else
            {
                statusCode = request.QueryString["tradeState"];
            }

            return statusCode == "SUCCESS";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            string statusCode = string.Empty;
            if (request.RequestType.ToUpper() == "POST")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                var recvStr = Encoding.UTF8.GetString(data);
                var d = recvStr.DeserializeObject();
                statusCode = d["tradeState"].ToString();
            }
            else
            {
                statusCode = request.QueryString["tradeState"];
            }

            return statusCode == "SUCCESS" ? "支付成功" : "支付失败";
        }

        public string SendPostHttpRequest2(string url, Dictionary<string, string> requestData)
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

        public string SendPostHttpRequest(string url, string jsonStr)
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


        public static string MD5Sign(string strToBeEncrypt, string md5Key)
        {
            string strSrc = strToBeEncrypt +"&key=" +md5Key;

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
