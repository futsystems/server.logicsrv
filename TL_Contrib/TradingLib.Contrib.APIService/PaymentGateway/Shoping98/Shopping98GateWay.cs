using Common.Logging;
using DotLiquid;
using System.Text;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;

namespace TradingLib.Contrib.Payment.Shopping98
{
    public class Shopping98GateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("Shopping98GateWay");

        public Shopping98GateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.Shopping98;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.MerID = data["MerID"].ToString();
            this.Key = data["Key"].ToString(); 
        }

        string PayUrl = "http://paytest.shopping98.com/scanpay/scan/pay/gateway";
        string MerID = "685675100944666624";
        string Key = "1177cb187e4c4847ac60ed90802ed05c";
        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropShopping98Payment data = new DropShopping98Payment();

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);


            data.service = "v1_scan_pay";
            data.version = "1.0";
            data.mch_no = this.MerID;
            data.charset = "UTF-8";
            data.req_time = operatioin.DateTime.ToString();
            data.sign_type = "MD5";
            data.nonce_str = operatioin.Ref;
            data.out_trade_no = operatioin.Ref;
            data.order_subject = "充值";
            data.order_desc = "充值";
            data.acquirer_type = ConvBankCode(operatioin.Bank);
            data.total_fee = ((int)(operatioin.Amount * 100)).ToString();
            data.return_url = APIGlobal.CustNotifyUrl + "/shopping98";
            data.notify_url = APIGlobal.SrvNotifyUrl + "/shopping98";
            data.currency = "CNY";
            data.client_ip = "127.0.0.1";
            data.order_time = operatioin.DateTime.ToString();
            data.time_expire = "30";
            data.device_info = "";
            data.attach = "";
            data.extend = "";

            SortedDictionary<string, string> args = new SortedDictionary<string, string>();
            args.Add("service", data.service);
            args.Add("version", data.version);
            args.Add("mch_no", data.mch_no);
            args.Add("charset", data.charset);
            args.Add("req_time", data.req_time);
            args.Add("sign_type", data.sign_type);
            args.Add("nonce_str", data.nonce_str);
            args.Add("out_trade_no", data.out_trade_no);
            args.Add("order_subject", data.order_subject);
            args.Add("order_desc", data.order_desc);
            args.Add("acquirer_type", data.acquirer_type);
            args.Add("total_fee", data.total_fee);
            args.Add("return_url", data.return_url);
            args.Add("notify_url", data.notify_url);
            args.Add("currency", data.currency);
            args.Add("client_ip", data.client_ip);
            args.Add("order_time", data.order_time);
            args.Add("time_expire", data.time_expire);
            args.Add("device_info", data.device_info);
            args.Add("attach", data.attach);
            args.Add("extend", data.extend);

            var tmp = FilterPara(args);
            var srcStr = CreateLinkString(tmp);

            data.sign = MD5Helper.MD5Sign(srcStr, this.Key);
            tmp["sign_type"] = data.sign_type;
            tmp["sign"] = data.sign;

            var resp = SendPostHttpRequest(this.PayUrl, tmp);
            var respdata = resp.DeserializeObject();
            data.url = respdata["code_url"].ToString();
            return data;
        }


        string ConvBankCode(string stdCode)
        {
            switch (stdCode)
            {
                //支付宝
                case "AliPay": return "alipay";
                case "WeiXin": return "wechat";
                default:
                    return "alipay";

            }
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["out_trade_no"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var Request = request.Params;

            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["resp_code"] == "0000";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["resp_code"] == "0000" ? "支付成功" : "支付失败";
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
        public static string CreateLinkString(Dictionary<string, string> dicArray)
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

    }
}
