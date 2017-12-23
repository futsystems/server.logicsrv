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
namespace TradingLib.Contrib.Payment.Ecpss
{
    public class EcpssGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("EcpssGateWay");

        public EcpssGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.Ecpss;
            var data = config.Config.DeserializeObject();

            this.SuccessReponse = "ok";

            try
            {
                this.PayUrl = data["PayUrl"].ToString();
                this.MerNo = data["MerNo"].ToString();
                this.MD5Key = data["MD5Key"].ToString();
                this.Domain = data["Domain"].ToString();
            }
            catch (Exception ex)
            { 
                
            }
            
            this.ReturnURL = APIGlobal.CustNotifyUrl + "/ecpss";
            this.AdviceURL = APIGlobal.SrvNotifyUrl + "/ecpss";

            this.ReturnURL = this.ReturnURL.Replace(APIGlobal.LocalIPAddress, this.Domain);
            this.AdviceURL = this.AdviceURL.Replace(APIGlobal.LocalIPAddress, this.Domain);
            //this.PayDirectUrl = this.PayDirectUrl.Replace(APIGlobal.LocalIPAddress, this.Domain);
            this.PayDirectUrl = string.Format("http://{0}/cash/depositdirect?ref=", this.Domain);
        }

        public string MerNo = "34352";
        public string PayUrl = "https://gwapi.yemadai.com/pay/sslpayment";
        public string MD5Key = "j_ezUE*Pc";

        public string Domain = "shop.zjzsb.top";

        public string ReturnURL = "";
        public string AdviceURL = "";


        public override Drop CreatePaymentDrop(CashOperation operatioin, HttpRequest request)
        {
            DropEcpssPayment data = new DropEcpssPayment();

            data.MerNo = this.MerNo;
            data.BillNo = operatioin.Ref;
            data.VAmount = operatioin.Amount.ToFormatStr();

            data.ReturnURL = this.ReturnURL;// APIGlobal.CustNotifyUrl + "/ecpss";
            data.AdviceURL = this.AdviceURL;// APIGlobal.SrvNotifyUrl + "/ecpss";

            data.SignInfo = "";
            data.OrderTime = operatioin.DateTime.ToString();
            data.payType = "B2CDebit";

            data.defaultBankNumber = string.Empty;
            data.Remark = string.Empty;
            data.products = string.Empty;

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);


            string md5src = string.Format("MerNo={0}&BillNo={1}&Amount={2}&OrderTime={3}&ReturnURL={4}&AdviceURL={5}&{6}", data.MerNo, data.BillNo, data.VAmount, data.OrderTime, data.ReturnURL, data.AdviceURL, this.MD5Key);
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
            string OrderNo = queryString["OrderNo"];
            string Amount = queryString["Amount"];
            string Succeed = queryString["Succeed"];
            string signInfo = queryString["SignInfo"];

            string md5src = string.Format("MerNo={0}&BillNo={1}&OrderNo={2}&Amount={3}&Succeed={4}&{5}", this.MerNo, BillNo, OrderNo, Amount, Succeed, this.MD5Key);
            logger.Info("RawStr:" + md5src);
            logger.Info("L-SignInfo:" + EcpssHelper.MD5Sign(md5src));
            logger.Info("M-SignIfno:" + signInfo);
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
