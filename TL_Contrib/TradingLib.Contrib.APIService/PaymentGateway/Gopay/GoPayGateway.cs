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
    public class GoPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("GoPayGateWay");

        public GoPayGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.GoPay;
            var data = config.Config.DeserializeObject();

            //this.PayUrl = "https://gatewaymer.gopay.com.cn/Trans/WebClientAction.do";

            //this.VerficationCode = "11111aaaaa";
            //this.VirCardNo = "0000000002000000257";
            //this.MerID = "0000001502";

            this.PayUrl = data["PayUrl"].ToString(); //"http://pay.chinagpay.com/bas/FrontTrans";
            this.MerID = data["MerID"].ToString(); //"929030000081335";
            this.VerficationCode = data["VerficationCode"].ToString();// "hZdATerAjnT5HV25zBunvFdaUKdPTsvd";
            this.VirCardNo = data["AccNo"].ToString();

            this.SuccessReponse = "RespCode=0000|JumpURL=";
        }

        public string MerID { get; set; }
        public string VirCardNo { get; set; }
        public string VerficationCode { get; set; }
        public string PayUrl { get; set; }
        public override Drop CreatePaymentDrop(CashOperation operation)
        {
            DropGoPayPayment data = new DropGoPayPayment();
            data.Version = "2.2";
            data.Charset = "2";
            data.Language = "1";
            data.SignType = "1";
            data.TranCode = "8888";
            data.MerchantID = this.MerID;
            data.MerOrderNum = operation.Ref;
            data.TranAmt = operation.Amount.ToFormatStr();
            data.CurrencyType = "156";

            data.FrontMerUrl = APIGlobal.CustNotifyUrl + "/gopay";
            data.BackgroundMerUrl = APIGlobal.SrvNotifyUrl + "/gopay";

            data.TranDateTime = operation.DateTime.ToString();
            data.VirCardNoIn = this.VirCardNo;
            data.TranIP = "192.168.0.1";
            data.PayUrl = this.PayUrl;

            data.Account = operation.Account;
            data.Amount = string.Format("{0}[{1}]", operation.Amount.ToFormatStr(), operation.Amount.ToChineseStr());
            data.Ref = operation.Ref;
            data.Operation = Util.GetEnumDescription(operation.OperationType);

            string rawstr = string.Format("version=[{0}]tranCode=[{1}]merchantID=[{2}]merOrderNum=[{3}]tranAmt=[{4}]feeAmt=[]tranDateTime=[{5}]frontMerUrl=[{6}]backgroundMerUrl=[{7}]orderId=[]gopayOutOrderId=[]tranIP=[{8}]respCode=[]gopayServerTime=[]VerficationCode=[{9}]",
                data.Version,
                data.TranCode,
                data.MerchantID,
                data.MerOrderNum,
                data.TranAmt,
                data.TranDateTime,
                data.FrontMerUrl,
                data.BackgroundMerUrl,
                data.TranIP,
                this.VerficationCode);

            data.SignValue = md5(rawstr);


            return data;
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["merOrderNum"];
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

            string prestr = string.Format("version=[{0}]tranCode=[{1}]merchantID=[{2}]merOrderNum=[{3}]tranAmt=[{4}]feeAmt=[{5}]tranDateTime=[{6}]frontMerUrl=[{7}]backgroundMerUrl=[{8}]orderId=[{9}]gopayOutOrderId=[{10}]tranIP=[{11}]respCode=[{12}]gopayServerTime=[]VerficationCode=[{13}]",
                args["version"],
                args["tranCode"],
                args["merchantID"],
                args["merOrderNum"],
                args["tranAmt"],
                args["feeAmt"],
                args["tranDateTime"],
                args["frontMerUrl"],
                args["backgroundMerUrl"],
                args["orderId"],
                args["gopayOutOrderId"],
                args["tranIP"],
                args["respCode"],
                this.VerficationCode
                );

            bool ret = args["signValue"] == md5(prestr);
            return ret;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["respCode"] == "0000";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            string ResultDesc = queryString["msgExt"];
            return ResultDesc;
        }

        static string md5(string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

    }
}
