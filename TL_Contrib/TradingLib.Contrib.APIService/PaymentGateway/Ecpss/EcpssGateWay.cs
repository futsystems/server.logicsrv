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

namespace TradingLib.Contrib.Payment.Ecpss
{
    public class EcpssGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("EcpssGateWay");

        public EcpssGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.Ecpss;

            this.SuccessReponse = "ok";
        }

        public string MerNo = "34352";
        public string PayUrl = "https://pay.ecpss.com/sslpayment";
        public string MD5Key = "j_ezUiUU";


        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropEcpssPayment data = new DropEcpssPayment();

            data.MerNo = this.MerNo;
            data.BillNo = operatioin.Ref;
            data.VAmount = operatioin.Amount.ToFormatStr();

            data.ReturnURL = APIGlobal.CustNotifyUrl + "/ecpss";
            data.AdviceURL = APIGlobal.SrvNotifyUrl + "/ecpss";

            data.SignInfo = "";
            data.orderTime = operatioin.DateTime.ToString();
            data.defaultBankNumber = string.Empty;
            data.Remark = string.Empty;
            data.products = string.Empty;

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);


            string md5src = data.MerNo + "&" + data.BillNo + "&" + data.VAmount + "&" + data.ReturnURL + "&" + this.MD5Key;
            data.SignInfo = EcpssHelper.MD5Sign(md5src);

            return data;
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["BillNo"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }


        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var queryString = request.Params;

            string BillNo = queryString["BillNo"];
            string Amount = queryString["Amount"];
            string Succeed = queryString["Succeed"];
            string signInfo = queryString["SignMD5info"];

            string md5src = BillNo + "&" + Amount + "&" + Succeed + "&" + this.MD5Key;				//对数据进行加密验证



            bool ret = signInfo == (EcpssHelper.MD5Sign(md5src));
            return ret;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["Succeed"] == "88";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["Result"];
        }



    }
}
