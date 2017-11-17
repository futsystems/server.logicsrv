﻿using Common.Logging;
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

namespace TradingLib.Contrib.Payment.JuHe
{
    public class JuHeGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("JuHeGateWay");

        public JuHeGateWay(GateWayConfig config)
            : base(config)
        {
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;

            this.GateWayType = QSEnumGateWayType.JuHe;
            var data = config.Config.DeserializeObject();
            
            this.PayUrl = data["PayUrl"].ToString();
            this.APPID = data["APPID"].ToString();
            this.Key = data["Key"].ToString();


            try
            {

            }
            catch (Exception ex)
            {
                PubKey = data["PubKey"].ToString();
            }
            /*
            ServicePointManager.ServerCertificateValidationCallback +=
             delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
             System.Security.Cryptography.X509Certificates.X509Chain chain,
             System.Net.Security.SslPolicyErrors sslPolicyErrors)
             {
                 return true; // **** Always accept
             };
            **/
            //string var = "fxxRrNNHTckJykEqRWn6VMD/nPgeh8uIQPPxU5wQycBsESvf9bNUxpKGfJg9yzqyahfWJ5sXKk7llBSPltH6YgUEpchD8+NwK0UuUoueMBhQWQM6XdqWvYOLKgh8jB57/cNqsmp7puKYF3i+i5syNO13vfX3Oyx4sGOGWHVS19xj86PVLQBNBPU7YddQptA6A6cMdu1trEfDTQDgE3zkt9hmdiDneYfwLKB+1cdoqStrTpNqJOFaeiIDjVCUSmWWa8laemyduOTaDyi4S6m3SRmz5ICWgeF53A4tQdgy57kQkdlio7WMAnqV1RO2ZspJOWrOaJPabgk35vNlPQxZjQ/2YhUVW+MHP6IWVedFlmIGEJ8x7oLimQ+yq4VVgUjIZ01169FLylVtMIdUc8dlU5NYt2N397iiE/HjmsAPVv5TpXrqGzmkq9lecC5GuKVENhbrvDTWmcNpgDXNeMQoKjXGeAnL76Az90cT4yjPEjW/YSYO00rboSTc4mr7pfUgB5zNtOXYQ0Kw5VPgOE1Kwz4EXuvVb945wsmrcP9xqSil08LMQlM8px753BUm5FWVThGguOsjuXpnpFUblc8iX6e/+L/DNH4zgvSOprObAlNPvXFkuaWYT9RHk5odCvII84hkA8SPOOD76hQC3RaE2K/s5m/3OcthNvypJrvFbgEM7ysW0OvLAfR6EfmG9yv41Tg3XaUwZNtGSa4FvSXra09AAa6smrVldtVcw6dpwd9GIe/ADKXzYvkwKblPW81VgOuZUQKnYTm89UOI9ZDYzPxjS3bl74eXgzMc6uZBlJOpTCEE1GSBIriafg2ohO6xk3UQkQRrhclKzTLyjYmVIA==";
            //string rawData = JuHeRSAUtil.DecryptByPublicKey(var, PubKey);
            //var respdata = rawData.DeserializeObject();
            //string ordid = respdata["data"]["order_no"].ToString();
            //logger.Info("resp" + rawData);

        }

        string PayUrl = "http://119.23.246.110:9010/mps/v1/charges";
        string APPID = "app_YjJjMTE0NGFhMjQ5";
        string Key = "sk_live_MjMwNmVhMjhlMmIyZjNhYTAz";
        static string PubKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCAGdVJK1oSyaClQTJ4GwFy41uaz/5zIeHYwWNu+sN1Guf/Fr3Gt+VXqwqOKihKJrz6O7kQy98ggyibb0cvEcF1MEiaqk0w64uNuBmmRF49fGbnqg8HpSQbpEmSQzu3nhG9KEZLGQj5a8UIp7QxGS9prfF9L8I2PKpaSVxE2ZocsQIDAQAB";
                               
        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropJuHePayment data = new DropJuHePayment();
            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);
            data.Account = operatioin.Account;

            data.order_no = operatioin.Ref;
            data.app_id = this.APPID;
            data.channel = "upacp_pc";
            data.amountf = ((int)(operatioin.Amount * 100)).ToString();
            data.client_ip = "127.0.0.1";
            data.subject = "充值";
            data.body = "充值";
            data.result_url = APIGlobal.CustNotifyUrl + "/juhe";
            data.notify_url = APIGlobal.SrvNotifyUrl + "/juhe";


            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("order_no", data.order_no);//必须唯一，不可重复
            parameters.Add("app[id]", data.app_id);
            parameters.Add("channel",data.channel);//请联系商务确认已开通的支付通道 upacp_wap：银联快捷  upacp_pc：银联网关   wx_wap：微信 H5 支付 alipay_wap：支付宝h5
            parameters.Add("amount", data.amountf);
            parameters.Add("client_ip", data.client_ip);
            parameters.Add("subject", data.subject);
            parameters.Add("body", data.body);
            parameters.Add("notify_url", data.notify_url);    //C/S架构请主动查询支付结果。
            parameters.Add("metadata","");
            parameters.Add("extra[result_url]",data.result_url);

            logger.Info(string.Format("send request to:{0}", this.PayUrl));
            String str = HttpHelper.PostHttpResponseJson(this.PayUrl,null,parameters,this.Key);//status=200表示调用成功
            //string str = SendPostHttpRequest(this.PayUrl, parameters);
            var respdata = str.DeserializeObject();
            logger.Info("response:" + respdata);
            try
            {
                data.url = respdata["credential"]["pay_url"].ToString();
            }
            catch (Exception ex)
            {

            }

            return data;
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
            logger.Info("Request:" + result);
            return result;
        }


        public static CashOperation GetCashOperation(NHttp.HttpRequest request)
        {
            string var = "fxxRrNNHTckJykEqRWn6VMD/nPgeh8uIQPPxU5wQycBsESvf9bNUxpKGfJg9yzqyahfWJ5sXKk7llBSPltH6YgUEpchD8+NwK0UuUoueMBhQWQM6XdqWvYOLKgh8jB57/cNqsmp7puKYF3i+i5syNO13vfX3Oyx4sGOGWHVS19xj86PVLQBNBPU7YddQptA6A6cMdu1trEfDTQDgE3zkt9hmdiDneYfwLKB+1cdoqStrTpNqJOFaeiIDjVCUSmWWa8laemyduOTaDyi4S6m3SRmz5ICWgeF53A4tQdgy57kQkdlio7WMAnqV1RO2ZspJOWrOaJPabgk35vNlPQxZjQ/2YhUVW+MHP6IWVedFlmIGEJ8x7oLimQ+yq4VVgUjIZ01169FLylVtMIdUc8dlU5NYt2N397iiE/HjmsAPVv5TpXrqGzmkq9lecC5GuKVENhbrvDTWmcNpgDXNeMQoKjXGeAnL76Az90cT4yjPEjW/YSYO00rboSTc4mr7pfUgB5zNtOXYQ0Kw5VPgOE1Kwz4EXuvVb945wsmrcP9xqSil08LMQlM8px753BUm5FWVThGguOsjuXpnpFUblc8iX6e/+L/DNH4zgvSOprObAlNPvXFkuaWYT9RHk5odCvII84hkA8SPOOD76hQC3RaE2K/s5m/3OcthNvypJrvFbgEM7ysW0OvLAfR6EfmG9yv41Tg3XaUwZNtGSa4FvSXra09AAa6smrVldtVcw6dpwd9GIe/ADKXzYvkwKblPW81VgOuZUQKnYTm89UOI9ZDYzPxjS3bl74eXgzMc6uZBlJOpTCEE1GSBIriafg2ohO6xk3UQkQRrhclKzTLyjYmVIA==";

         
            string recvStr = string.Empty;
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                recvStr = Encoding.UTF8.GetString(data);
            }
            else
            {
                recvStr = request.RawContent;
            }

            string rawData = JuHeRSAUtil.DecryptByPublicKey(recvStr, PubKey);
            var respdata = rawData.DeserializeObject();
            string ordid = respdata["data"]["order_no"].ToString();

            return ORM.MCashOperation.SelectCashOperation(ordid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            string recvStr = string.Empty;
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                recvStr = Encoding.UTF8.GetString(data);
            }
            else
            {
                recvStr = request.RawContent;
            }

            var Request = request.Params;
            string rawData = JuHeRSAUtil.DecryptByPublicKey(recvStr, PubKey);
            var respdata = rawData.DeserializeObject();
            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            string recvStr = string.Empty;
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                recvStr = Encoding.UTF8.GetString(data);
            }
            else
            {
                recvStr = request.RawContent;
            }

            var Request = request.Params;
            string rawData = JuHeRSAUtil.DecryptByPublicKey(recvStr, PubKey);
            var respdata = rawData.DeserializeObject();

            string status = respdata["data"]["paid"].ToString();

            return status.ToUpper() == "TRUE";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            string recvStr = string.Empty;
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                recvStr = Encoding.UTF8.GetString(data);
            }
            else
            {
                recvStr = request.RawContent;
            }

            var Request = request.Params;
            string rawData = JuHeRSAUtil.DecryptByPublicKey(recvStr, PubKey);
            var respdata = rawData.DeserializeObject();


            string status = respdata["data"]["paid"].ToString();
            return status.ToUpper() == "TRUE"  ? "支付成功" : "支付失败";
        }

    }
}