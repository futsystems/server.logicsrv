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

namespace TradingLib.Contrib.Payment.RMTech
{
    public class RMTechGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("RMTechGateWay");

        public RMTechGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.RMTech;
            var data = config.Config.DeserializeObject();
            //this.PayUrl = data["PayUrl"].ToString();
           // this.MerID = data["MerID"].ToString();
            //this.Key = data["Key"].ToString();
        }

        string PayUrl = "https://www.rm-tech.com.cn/agent-platform/cloudplatform/api/trade.html";
        string MerID = "100060000000198";
        string Key = "068534056bac473582b1c789b62e3827";

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropRMTechPayment data = new DropRMTechPayment();

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            data.tradeType = "cs.pay.submit";
            data.version = "2.0";
            data.channel = "";
            data.mchNo = this.MerID;
            data.sign = "";
            data.body = "充值";
            data.mchOrderNo = operatioin.Ref;
            data.oamount = (double)operatioin.Amount;
            data.decAmount = 0;
            data.description = "";
            data.currency = "CNY";
            data.timePaid = operatioin.DateTime.ToString();
            data.timeExpire = "";
            data.timeSettle = "";
            data.subject = "充值";
            data.extra = "";
            data.innerOrderNo = operatioin.Ref;
            data.virAccNo = "0";
            data.oamount2 = (double)operatioin.Amount;
            data.freezeDays = 0;

            var detail = new
            {
                innerOrderNo = operatioin.Ref,
                virAccNo = "0",
                amount = (double)operatioin.Amount,
            };

            
            var extra = new 
            {
                bankPayType = "01",
                cardType = "01",
                bankCode="",
                notifyUrl=APIGlobal.SrvNotifyUrl + "/rmtech",
                callbackUrl = APIGlobal.CustNotifyUrl + "/rmtech",
            };
            
            /*
            var extra = new
            {
                openId = "oVpgIwd9ARpCzoioXEKsKwdXnsi8",
                notifyUrl = APIGlobal.SrvNotifyUrl + "/rmtech",
            };
             * **/

            var obj = new {
                amount = (double)operatioin.Amount,
                body = "充值",
                channel = "wx_pub",
                amountSettle = string.Empty,
                timeExpire = string.Empty,
                timeSettle = string.Empty,
                subject = string.Empty,
                currency ="CNY",
                description = "充值",
                mchNo = this.MerID,
                mchOrderNo = operatioin.Ref,
                timePaid = "",
                tradeType = "cs.pay.submit",
                version = "2.0",
                details = new []{detail},
                extra = extra,
                sign="",

            };

            SortedDictionary<string, string> args = new SortedDictionary<string, string>();
            args.Add("amount",((decimal)obj.amount).ToFormatStr());
            args.Add("body", obj.body);
            args.Add("channel", obj.channel);
            args.Add("amountSettle", obj.amountSettle);
            args.Add("timeExpire", obj.timeExpire);
            args.Add("timeSettle", obj.timeSettle);
            args.Add("subject", obj.subject);
            args.Add("currency", obj.currency);
            args.Add("description", obj.description);
            args.Add("mchNo", obj.mchNo);
            args.Add("mchOrderNo", obj.mchOrderNo);
            args.Add("timePaid", obj.timePaid);
            args.Add("tradeType", obj.tradeType);
            args.Add("version", obj.version);
            args.Add("bankPayType", extra.bankPayType);
            args.Add("cardType", extra.cardType);
            args.Add("bankCode", extra.bankCode);
            //args.Add("openId", extra.openId);
            args.Add("notifyUrl", extra.notifyUrl);
            args.Add("callbackUrl", extra.callbackUrl);
            args.Add("details", "[{" + string.Format("amount={0}&innerOrderNo={1}&virAccNo={2}", ((decimal)detail.amount).ToFormatStr(), detail.innerOrderNo, detail.virAccNo) + "}]");




            string rawStr = CreateLinkString(FilterPara(args));
            logger.Info("rawStr:" + rawStr);
            var sign = MD5Sign(rawStr, this.Key);

            var request = new
            {
                amount = obj.amount,
                body = obj.body,
                channel = obj.channel,
                amountSettle = obj.amountSettle,
                timeExpire = obj.timeExpire,
                timeSettle = obj.timeSettle,
                subject = obj.subject,
                currency = obj.currency,
                description = obj.description,
                mchNo = obj.mchNo,
                mchOrderNo = obj.mchOrderNo,
                timePaid = obj.timePaid,
                tradeType = obj.tradeType,
                version = obj.version,
                details = new[] { detail },
                extra = extra,
                sign = sign,

            };

            string result = SendPostHttpRequest(this.PayUrl, request.SerializeObject());

            return data;
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



        public  string MD5Sign(string strToBeEncrypt, string md5Key)
        {
            string strSrc = strToBeEncrypt +"&key="+ md5Key;
            logger.Info("signStr:" + strSrc);
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
                if (temp.Key.ToLower() != "sign" && temp.Value != "" && temp.Value != null)
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

    }

    
}
