using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using DotLiquid;
using System.Net;
using System.IO;
using System.Security;
using System.Security.Cryptography;

namespace TradingLib.Contrib.APIService
{
    public class ChinagpayGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("UnspayGateWay");

        public ChinagpayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.ChinagPay;

            //this.PayUrl = "http://180.169.129.78:38280/bas/FrontTrans";
            //this.MerID = "200000000000001";
            //this.MD5Key = "88888888";

            this.SuccessReponse = "SUCCESS";
            var data = config.Config.DeserializeObject();

            this.PayUrl = data["PayUrl"].ToString(); //"http://pay.chinagpay.com/bas/FrontTrans";
            this.MerID = data["MerID"].ToString(); //"929030000081335";
            this.MD5Key = data["MD5Key"].ToString();// "hZdATerAjnT5HV25zBunvFdaUKdPTsvd";
        }

        public string MerID { get; set; }
        public string PayUrl { get; set; }
        public string MD5Key { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropChinagPayment data = new DropChinagPayment();

            data.SignMethod = "MD5";
            data.Signature = string.Empty;
            data.Version = "1.0.0";
            data.TxnType = "01";
            data.TxnSubType = "00";
            data.BizType = "000000";
            data.AccessType = "0";
            data.AccessMode = "01";
            data.MerId = this.MerID;
            data.MerOrderId = operatioin.Ref;
            data.TxnTime = operatioin.DateTime.ToString();
            data.TxnAmt = (operatioin.Amount * 100).ToFormatStr("{0:F0}");
            data.Currency = "CNY";
            data.FrontUrl = APIGlobal.CustNotifyUrl + "/chinagpay";
            data.BackUrl = APIGlobal.SrvNotifyUrl + "/chinagpay";
            data.PayType = "0201";
            data.PayUrl = this.PayUrl;

            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            //签名
            string str = string.Format("accessMode={0}&accessType={1}&backUrl={2}&bizType={3}&currency={4}&frontUrl={5}&merId={6}&merOrderId={7}&payType={8}&txnAmt={9}&txnSubType={10}&txnTime={11}&txnType={12}&version={13}",
                data.AccessMode,
                data.AccessType,
                data.BackUrl,
                data.BizType,
                data.Currency,
                data.FrontUrl,
                data.MerId,
                data.MerOrderId,
                data.PayType,
                data.TxnAmt,
                data.TxnSubType,
                data.TxnTime,
                data.TxnType,
                data.Version);

            data.Signature = Sign(str, this.MD5Key);

            return data;
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["merOrderId"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            SortedDictionary<string, string> args = new SortedDictionary<string, string>();
            if (request.RequestType.ToUpper() == "POST")
            {
                foreach (var key in request.Form.AllKeys)
                {
                    args.Add(key, request.Form[key]);
                }
            }
            else
            {
                foreach (var key in request.QueryString.AllKeys)
                {
                    args.Add(key, request.QueryString[key]);
                }
            }

            if (args.ContainsKey("respMsg"))
            {
                args["respMsg"] = Encoding.UTF8.GetString(Convert.FromBase64String(args["respMsg"]));
            }

            string prestr = CreateLinkString(args);
            string signature = args["signature"];
            return signature == Sign(prestr, this.MD5Key);
        }

        string CreateLinkString(SortedDictionary<string, string> dicArrayPre)
        {
            StringBuilder prestr = new StringBuilder();

            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                bool need = (temp.Key.ToLower() != "signmethod" && temp.Key.ToLower() != "signature");
                if (need)
                {
                    prestr.Append(temp.Key + "=" + temp.Value + "&");
                }
            }
            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);
            return prestr.ToString();
        }


        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["respCode"] == "1001";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            string ResultDesc = queryString["respCode"] == "1001" ? "成功" : "失败";

            return ResultDesc;
        }


        /// <summary>
        /// 签名字符串
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>签名结果</returns>
        static string Sign(string prestr, string key)
        {
            StringBuilder sb = new StringBuilder(32);

            prestr = prestr + key;

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.UTF8.GetBytes(prestr));
            return Convert.ToBase64String(t);
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="sign">签名结果</param>
        /// <param name="key">密钥</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>验证结果</returns>
        static bool Verify(string prestr, string sign, string key, string _input_charset)
        {
            string mysign = Sign(prestr, key);
            if (mysign == sign)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
