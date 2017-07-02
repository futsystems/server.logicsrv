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
using TradingLib.Contrib.APIService;

namespace TradingLib.Contrib.Payment.ETone
{
    public class ETonePayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("ETonePayGateWay");

        public ETonePayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.ETonePay;

            var data = config.Config.DeserializeObject();

            this.PayUrl = data["PayUrl"].ToString(); //"http://58.56.23.89:7002/NetPay/BankSelect.action";
            this.MerchantID = data["MerID"].ToString(); //"888888888888888";
            this.BussID = data["BussID"].ToString(); //"100000";
            this.DataKey = data["MerKeyBussID"].ToString(); //"8EF53C251102A4E6";
        }


        public string MerchantID { get; set; }

        public string BussID { get; set; }

        public string DataKey { get; set; }

        public string PayUrl { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropETonePayment data = new DropETonePayment();

            data.version = "1.0.0";
            data.transCode = "8888";
            data.merchantId = this.MerchantID;
            data.merOrderNum = operatioin.Ref;
            data.bussId = this.BussID;
            data.tranAmt = (operatioin.Amount * 100).ToFormatStr("{0:F0}");
            data.sysTraceNum = operatioin.Ref;
            data.tranDateTime = operatioin.DateTime.ToString();
            data.currencyType = "156";
            data.merURL = APIGlobal.CustNotifyUrl + "/etonepay";
            data.backURL = APIGlobal.SrvNotifyUrl + "/etonepay";
            data.orderInfo = string.Empty;
            //data.userId = string.Empty;
            //data.userIp = string.Empty;
            //data.bankId = string.Empty;
            //data.stlmId = string.Empty;
            data.entryType = "1";
            //data.reserver1 = string.Empty;
            //data.reserver2 = string.Empty;
            //data.reserver3 = string.Empty;
            //data.reserver4 = string.Empty;

            data.PayUrl = this.PayUrl;

            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);


            if (data.orderInfo != string.Empty)
                data.orderInfo = ETonePayUtils.StringToHexString(data.orderInfo);
            string txnString = data.version + "|" + data.transCode + "|" + data.merchantId + "|" + data.merOrderNum + "|" + data.bussId + "|" + data.tranAmt + "|" + data.sysTraceNum
                 + "|" + data.tranDateTime + "|" + data.currencyType + "|" + data.merURL + "|" + data.backURL + "|" + data.orderInfo + "|" + "";
            data.signValue = ETonePayUtils.md5(txnString + this.DataKey);

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

            string transCode = queryString["transCode"];
            string merchantId = queryString["merchantId"];
            string respCode = queryString["respCode"];
            string sysTraceNum = queryString["sysTraceNum"];
            string merOrderNum = queryString["merOrderNum"];
            string orderId = queryString["orderId"];
            string bussId = queryString["bussId"];
            string tranAmt = queryString["tranAmt"];
            string orderAmt = queryString["orderAmt"];
            string bankFeeAmt = queryString["bankFeeAmt"];
            string integralAmt = queryString["integralAmt"];
            string vaAmt = queryString["vaAmt"];
            string bankAmt = queryString["bankAmt"];
            string bankId = queryString["bankId"];
            string integralSeq = queryString["integralSeq"];
            string vaSeq = queryString["vaSeq"];
            string bankSeq = queryString["bankSeq"];
            string tranDateTime = queryString["tranDateTime"];
            string payMentTime = queryString["payMentTime"];
            string settleDate = queryString["settleDate"];
            string currencyType = queryString["currencyType"];
            string orderInfo = queryString["orderInfo"];
            string userId = queryString["userId"];
            string userIp = queryString["userIp"];
            string reserver1 = queryString["reserver1"];
            string reserver2 = queryString["reserver2"];
            string reserver3 = queryString["reserver3"];
            string reserver4 = queryString["reserver4"];
            string signValue = queryString["signValue"];

            string txnString = transCode + "|" + merchantId + "|" + respCode + "|" + sysTraceNum + "|" + merOrderNum + "|"
                + orderId + "|" + bussId + "|" + tranAmt + "|" + orderAmt + "|" + bankFeeAmt + "|" + integralAmt + "|"
                + vaAmt + "|" + bankAmt + "|" + bankId + "|" + integralSeq + "|" + vaSeq + "|"
                + bankSeq + "|" + tranDateTime + "|" + payMentTime + "|" + settleDate + "|" + currencyType + "|" + orderInfo + "|" + userId;

            string retSignValue = ETonePayUtils.md5(txnString + this.DataKey);

            return signValue == retSignValue;

        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["respCode"] == "0000";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["respCode"]=="000"?"成功":"失败";
        }



    }
}
