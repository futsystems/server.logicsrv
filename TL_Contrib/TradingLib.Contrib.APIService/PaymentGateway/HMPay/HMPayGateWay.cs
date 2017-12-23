using Common.Logging;
using DotLiquid;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;
using NHttp;
namespace TradingLib.Contrib.Payment.HMPay
{
    public class HMPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("HMPayGateWay");

        public HMPayGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.HMPay;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.MerID = data["MerID"].ToString();
            this.MerKey = data["MD5Key"].ToString(); 
        }
        string PayUrl = "http://api.zhongnanpay.com:3022/hmpay/online/createWxOrder.do";// "https://www.sumapay.com/sumapay/unitivepay_bankPayForNoLoginUser";
        string MerID = "168666999001265";
        string MerKey = "lhll07f7qkm9cned";


        public override Drop CreatePaymentDrop(CashOperation operatioin, HttpRequest request)
        {
            DropHMPayment data = new DropHMPayment();
            data.PayUrl = this.PayUrl;
            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            data.merchant_no = this.MerID;
            data.total_fee = ((int)(operatioin.Amount * 100)).ToString();
            data.return_url = APIGlobal.CustNotifyUrl + "/hmpay";
            data.notifyurl = APIGlobal.SrvNotifyUrl + "/hmpay";
            data.pay_num = operatioin.Ref;

            data.pay_type = "gwpay";
            data.device_info = "";
            data.mch_app_name = "";

            string rawStr = string.Format("{0}{1}{2}", data.merchant_no, data.total_fee, DateTime.Now.ToTLDate());
            data.sign = MD5Helper.MD5Sign(rawStr, this.MerKey).ToUpper();
            //MD5(merchant_no+total_fee+today+key)；
            return data;
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["pay_num"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var Request = request.Params;
            var rereturn_code = Request["return_code"];
            var out_trade_no = Request["out_trade_no"];
            var trade_result = Request["trade_result "];
            var message = Request["message"];
            var pay_num = Request["pay_num"];
            var total_fee = Request["total_fee"];
            var sign = Request["sign"];
            var rSign = MD5Helper.MD5Sign(this.MerID + out_trade_no + pay_num + total_fee, this.MerKey);

            return sign.ToUpper() == rSign.ToUpper();
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["return_code"] == "10000";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["return_code"] == "10000" ? "支付成功" : "支付失败";
        }
    }
}
