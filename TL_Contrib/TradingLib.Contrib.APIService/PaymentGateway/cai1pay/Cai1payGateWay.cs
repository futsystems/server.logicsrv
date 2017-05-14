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
    public class Cai1payGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("Cai1payGateWay");

        public Cai1payGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.Cai1Pay;
            var data = config.Config.DeserializeObject();

            this.PayUrl = data["PayUrl"].ToString(); //"http://pay.chinagpay.com/bas/FrontTrans";
            this.MerCode = data["MerID"].ToString(); //"929030000081335";
            this.MerKey = data["MD5Key"].ToString();// "hZdATerAjnT5HV25zBunvFdaUKdPTsvd";

            //this.PayUrl = "http://testpay.cai1pay.com/gateway.aspx";
           // this.MerCode = "T0000601";
            //this.MerKey = "N3vuSHZ3j4WWemOmJinzmTG1t9KQTi2oeu7fxej7EWpFqkDRiFusIa0UNUbsXi9bcHSKFJPFZGKqDakiWSInOk5eTCr3cQA6woD8sTmzQEscpkqNJSwe0wnRmOQc2vCi";
            this.SuccessReponse = "success";
        }

        public string PayUrl { get; set; }
        public string MerCode { get; set; }
        public string MerKey { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropCai1Payment data = new DropCai1Payment();


            data.MerCode = this.MerCode;
            data.MerOrderNo = operatioin.Ref;
            data.OrderAmount = operatioin.Amount.ToFormatStr();
            data.OrderDate = Util.ToDateTime(operatioin.DateTime).ToTLDate().ToString();
            data.Currency = "RMB";
            data.GatewayType = "01";
            data.Language = "GB";
            data.ReturnUrl = APIGlobal.CustNotifyUrl + "/cai1pay";
            data.ServerUrl = APIGlobal.SrvNotifyUrl + "/cai1pay";
            data.Attach = "";
            data.OrderEncodeType = "2";
            data.RetEncodeType = "12";
            data.RetType = "1";
            data.PayUrl = this.PayUrl;
            string rawstr = data.MerOrderNo + data.OrderAmount + data.OrderDate + data.Currency;
            data.Sign = Sign(rawstr, this.MerKey).ToLower();

            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);



            return data;
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["MerOrderNo"];
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

            string prestr = args["MerOrderNo"] + args["Amount"] + args["OrderDate"] + args["Succ"] + args["SysOrderNo"] + args["Currency"];
            string signature = args["Signature"];
            return signature == Sign(prestr, this.MerKey).ToLower();
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["succ"] == "Y";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            string ResultDesc = queryString["Msg"];

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
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }


    }
}
