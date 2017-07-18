using Common.Logging;
using DotLiquid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;

namespace TradingLib.Contrib.Payment.Fjelt
{
    public class FjeltGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("FjeltGateWay");

        public FjeltGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.Fjelt;

        }


        public string PayUrl = "http://bank.fjelt.com/pay/rest";

        public string APPID = "0H7E0MP64T0005QN";

        public string SESSION = "1e890efd01684016a3295098f9b4ba65";

        public string SECRETKEY = "VCPKGGC2ZwfHCoTk";

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropFjeltPayment data = new DropFjeltPayment();

            data.appid = this.APPID;
            data.method = "masget.pay.compay.router.font.pay";
            data.format = "json";
            object info = new
            {
                amount=operatioin.Amount*100,
                payordernumber = operatioin.Ref,
                fronturl = APIGlobal.CustNotifyUrl + "/fjelt",
                backurl = APIGlobal.SrvNotifyUrl + "/fjelt",
                Body="充值",
                ExtraParams="",
                PayType="0",
            };
            
            data.data = AES.Encrypt(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(info), this.SECRETKEY, this.SECRETKEY);
            data.timestamp = Util.ToDateTime(operatioin.DateTime).ToString("yyyy-MM-dd HH:mm:ss");
            data.session = this.SESSION;
            data.v = "2.0";

            data.sign = AES.MakeMd5(this.SECRETKEY + data.appid + data.data + data.format + data.method + data.session + data.timestamp + data.v + this.SECRETKEY).ToLower();
            
            

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", data.appid);
            dic.Add("method", data.method);
            dic.Add("format", data.format);
            dic.Add("data", data.data);
            dic.Add("timestamp", data.timestamp);
            dic.Add("session", data.session);
            dic.Add("sign", data.sign);
            dic.Add("v", data.v);

            var resp = SendPostHttpRequest("http://bank.fjelt.com/pay/rest", dic);
            var respdata = resp.DeserializeObject();
            logger.Info("response:" + resp);
            data.url = respdata["data"].ToString();

            return data;
        }

        public static string SendPostHttpRequest(string url, Dictionary<string, string> requestData)
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


        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["merOrderNum"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var queryString = request.Params;

            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["respCode"] == "0000";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["respCode"] == "000" ? "成功" : "失败";
        }
    }
}
