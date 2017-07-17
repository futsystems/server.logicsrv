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
                PayType="",
            };
            
            data.data = AES.Encrypt(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(info), this.SECRETKEY, this.SECRETKEY);
            data.timestamp = Util.ToDateTime(operatioin.DateTime).ToString("yyyy-MM-dd HH:mm:ss");
            data.session = this.SESSION;
            data.sign = AES.MakeMd5(this.SECRETKEY + data.appid + data.data + data.format + data.method + data.session + data.timestamp + data.v + this.SECRETKEY).ToLower();
            
            data.v = "2.0";

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);



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
