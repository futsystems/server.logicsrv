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

namespace TradingLib.Contrib.Payment.JoinPay
{
    public class JoinPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("JoinPayGateWay");

        public JoinPayGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.JoinPay;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.MerID = data["MerID"].ToString();
            this.Key = data["Key"].ToString(); 
        }

        string PayUrl = "https://www.joinpay.com/gateway/gateway_init.action";
        string MerID = "888100000002340";
        string Key = "08fe2621d8e716b02ec0da35256a998d";
        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropJoinPayPayment data = new DropJoinPayPayment();

            data.p1_MerchantNo = this.MerID;
            data.p2_OrderNo = operatioin.Ref;
            data.p3_Amount = operatioin.Amount.ToFormatStr();
            data.p4_Cur = "1";
            data.p5_ProductName = "充值";
            data.p6_Mp = "";
            data.p7_ReturnUrl = APIGlobal.CustNotifyUrl + "/joinpay";
            data.p8_NotifyUrl = APIGlobal.SrvNotifyUrl + "/joinpay";
            data.p9_FrpCode = "";
            data.pa_OrderPeriod = "0";




            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            string sbOld = "";
            sbOld += data.p1_MerchantNo;
            sbOld += data.p2_OrderNo;
            sbOld += data.p3_Amount;
            sbOld += data.p4_Cur;
            sbOld += data.p5_ProductName;
            sbOld += data.p6_Mp;
            sbOld += data.p7_ReturnUrl;
            sbOld += data.p8_NotifyUrl;
            sbOld += data.p9_FrpCode;
            sbOld += data.pa_OrderPeriod;

            data.hmac = MD5Sign(sbOld, Key);


            return data;
        }


        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["r2_OrderNo"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var Request = request.Params;

            string r1_MerchantNo = Request["r1_MerchantNo"];
            string r2_OrderNo = Request["r2_OrderNo"];
            string r3_Amount = Request["r3_Amount"];
            string r4_Cur = Request["r4_Cur"];
            string r5_Mp = Request["r5_Mp"];
            string r6_Status = Request["r6_Status"];
            string r7_TrxNo = Request["r7_TrxNo"];
            string r8_BankOrderNo = Request["r8_BankOrderNo"];
            string r9_BankTrxNo = Request["r9_BankTrxNo"];
            string ra_PayTime = Request["ra_PayTime"];
            string rb_DealTime = Request["rb_DealTime"];
            string rc_BankCode = Request["rc_BankCode"];
            string hmac = Request["hmac"];
         

            string sbOld = "";
            sbOld += r1_MerchantNo;
            sbOld += r2_OrderNo;
            sbOld += r3_Amount;
            sbOld += r4_Cur;
            sbOld += r5_Mp;
            sbOld += r6_Status;
            sbOld += r7_TrxNo;
            sbOld += r8_BankOrderNo;
            sbOld += r9_BankTrxNo;
            sbOld += ra_PayTime;
            sbOld += rb_DealTime;
            sbOld += rc_BankCode;


            string nhmac = MD5Sign(sbOld, Key);
            //return (nhmac == hmac);
            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["r6_Status"] == "100";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["r6_Status"] == "100" ? "支付成功" : "支付失败";
        }


        public static string MD5Sign(string strToBeEncrypt, string md5Key)
        {
            string strSrc = strToBeEncrypt + md5Key;

            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] FromData = System.Text.Encoding.GetEncoding("utf-8").GetBytes(strSrc);
            Byte[] TargetData = md5.ComputeHash(FromData);
            string Byte2String = "";
            for (int i = 0; i < TargetData.Length; i++)
            {
                Byte2String += TargetData[i].ToString("x2");
            }
            return Byte2String.ToLower();
        }

    }
}
