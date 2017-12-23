using Common.Logging;
using DotLiquid;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;
using NHttp;
namespace TradingLib.Contrib.Payment.NewPay
{
    public class NewPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("NewPayGateWay");

        public NewPayGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.NewPay;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.PartnerID = data["MerID"].ToString();
            this.MD5Key = data["Key"].ToString(); 


        }


        string PayUrl = "https://pay.newpaypay.com/center/proxy/partner/v1/pay.jsp";
        string PartnerID = "246584000588";
        string MD5Key = "5457ce87-990b-11e7-9f8e-d995d8294f93";


        public override Drop CreatePaymentDrop(CashOperation operatioin, HttpRequest request)
        {
            DropNewPayPayment data = new DropNewPayPayment();

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            data.version = "v1";
            data.partnerId = this.PartnerID;
            data.orderId = operatioin.Ref;
            data.goods = Convert.ToBase64String(Encoding.UTF8.GetBytes("充值卡"));
            data.iamount = operatioin.Amount.ToFormatStr();

            data.pageUrl = APIGlobal.CustNotifyUrl + "/newpay";
            data.notifyUrl = APIGlobal.SrvNotifyUrl + "/newpay";
            data.reserve = "";
            data.extendInfo = "";
            data.payMode = "01";
            data.bankId = this.ConvBankCode(operatioin.Bank);
            data.creditType = "0";


            string rawStr = string.Format("version={0}&partnerId={1}&orderId={2}&goods={3}&amount={4}&expTime=&notifyUrl={5}&pageUrl={6}&reserve=&extendInfo=&payMode={7}&bankId={8}&creditType={9}&key={10}",
                data.version,
                data.partnerId,
                data.orderId,
                data.goods,
                data.iamount,
                data.notifyUrl,
                data.pageUrl,
                data.payMode,
                data.bankId,
                data.creditType,
                this.MD5Key);

            data.sign = Sign(rawStr);

            return data;
        }

        static string Sign(string rawStr)
        {
            StringBuilder sb = new StringBuilder(32);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.UTF8.GetBytes(rawStr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }


        string ConvBankCode(string stdCode)
        {
            switch (stdCode)
            {
                //工商银行
                case "01020000": return "ICBC";
                //农业银行
                case "01030000": return "ABC";
                //建设银行
                case "01050000": return "CCB";
                //中国银行
                case "01040000": return "BOC";
                //招商
                case "03080000": return "CMB";
                //交通
                case "03010000": return "BCOM";
                //邮政
                case "01000000": return "PSBC";
                default:
                    return "ICBC";

            }
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["orderId"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var Request = request.Params;

            string partnerId = Request["partnerId"];
            string orderId = Request["orderId"];
            string amount = Request["amount"];
            string result = Request["result"];
            string payTime = Request["payTime"];
            string traceId = Request["traceId"];
            string reserve = Request["reserve"];
            string creditType = Request["creditType"];
            string md5 = Request["md5"];



            string rawStr = string.Format("partnerId={0}&orderId={1}&amount={2}&result={3}&payTime={4}&traceId={5}&reserve={6}&creditType={7}&key={8}",
                partnerId,
                orderId,
                amount,
                result,
                payTime,
                traceId,
                reserve,
                creditType,
                this.MD5Key);




            string nSign = Sign(rawStr);
            return nSign == md5;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["result"] == "S";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["result"] == "S" ? "支付成功" : "支付失败";
        }


    }
}
