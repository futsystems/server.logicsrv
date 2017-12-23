using Common.Logging;
using DotLiquid;
using System;
using System.Net;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;
using NHttp;
namespace TradingLib.Contrib.Payment.FZPay
{
    public class FZPayGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("FZPayGateWay");

        public FZPayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.FZPay;

            var data = config.Config.DeserializeObject();

            this.PartnerID = data["MerID"].ToString();// "16985";
            this.MD5Key = data["MerKey"].ToString();// "de096cd227a594b4ab79af72a60ec035";
            this.PayUrl = data["PayUrl"].ToString();// "http://api.yunshi44.top/PayBank.aspx";


            this.SuccessReponse = "ok";

        }

        public string PayUrl { get; set; }
        public string PartnerID { get; set; }
        public string MD5Key { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operatioin, HttpRequest request)
        {
            DropFZPayment data = new DropFZPayment();

            data.partner = this.PartnerID;
            data.banktype = operatioin.Bank;
            data.paymoney = operatioin.Amount.ToFormatStr();
            data.ordernumber = operatioin.Ref;
            data.attach = string.Empty;
            data.hrefbackurl = APIGlobal.CustNotifyUrl + "/fzpay";
            data.callbackurl = APIGlobal.SrvNotifyUrl + "/fzpay";

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);


            string signSource = string.Format("partner={0}&banktype={1}&paymoney={2}&ordernumber={3}&callbackurl={4}{5}",data.partner, data.banktype, data.paymoney, data.ordernumber, data.callbackurl, this.MD5Key);
            data.sign = FZPayHelper.MD5(signSource, false).ToLower();
            return data;
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["ordernumber"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            int orderstatus = Convert.ToInt32(queryString["orderstatus"]);
            string ordernumber = queryString["ordernumber"];
            string paymoney = queryString["paymoney"];
            string sign = queryString["sign"];
            string attach = queryString["attach"];
            string signSource = string.Format("partner={0}&ordernumber={1}&orderstatus={2}&paymoney={3}{4}", this.PartnerID, ordernumber, orderstatus, paymoney, this.MD5Key);
            bool ret= sign.ToUpper() == FZPayHelper.MD5(signSource, false).ToUpper();
            return ret;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["orderstatus"] == "1";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["orderstatus"] == "1" ? "成功" : "失败";
        }




    }
}
