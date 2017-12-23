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
namespace TradingLib.Contrib.Payment.OpenEPay
{
    public class OpenEPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("OpenEPayGateWay");

        public OpenEPayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.OpenEPay;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.MerID = data["MerID"].ToString();
            this.MD5Key = data["MD5Key"].ToString();
            this.Domain = data["Domain"].ToString();
            this.frontURL = this.frontURL.Replace(APIGlobal.LocalIPAddress, this.Domain);
            this.backURL = this.backURL.Replace(APIGlobal.LocalIPAddress, this.Domain);
            this.PayDirectUrl = this.PayDirectUrl.Replace(APIGlobal.LocalIPAddress, this.Domain);
           
        }

        string PayUrl = "http://opsweb.koolyun.cn/gateway/index.do";
        string MerID = "101000171115001";
        string MD5Key = "1qaz2wSx88";
        string Domain = "";
        string frontURL = APIGlobal.CustNotifyUrl + "/openepay";
        string backURL = APIGlobal.SrvNotifyUrl + "/openepay";
        public override Drop CreatePaymentDrop(CashOperation operatioin, HttpRequest request)
        {
            DropOpenEPayPayment data = new DropOpenEPayPayment();

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            data.inputCharset = "1";
            data.pickupUrl = this.frontURL;
            data.receiveUrl = this.backURL;
            data.version = "v1.0";
            data.language = "1";
            data.signType = "0";
            data.merchantId = this.MerID;
            data.orderNo = operatioin.Ref;
            data.orderAmount = (operatioin.Amount * 100).ToFormatStr("{0:F0}");
            data.orderCurrency = "156";
            data.orderDatetime = operatioin.DateTime.ToString();
            data.productName = "充值";
            data.payType = "0";

            StringBuilder bufSignSrc = new StringBuilder();
            appendSignPara(bufSignSrc, "inputCharset", data.inputCharset);
            appendSignPara(bufSignSrc, "pickupUrl", data.pickupUrl);
            appendSignPara(bufSignSrc, "receiveUrl", data.receiveUrl);
            appendSignPara(bufSignSrc, "version", data.version);
            appendSignPara(bufSignSrc, "language", data.language);
            appendSignPara(bufSignSrc, "signType", data.signType);
            appendSignPara(bufSignSrc, "merchantId", data.merchantId);
            appendSignPara(bufSignSrc, "payerName", "");
            appendSignPara(bufSignSrc, "payerEmail", "");
            appendSignPara(bufSignSrc, "payerTelephone", "");
            appendSignPara(bufSignSrc, "orderNo", data.orderNo);
            appendSignPara(bufSignSrc, "orderAmount", data.orderAmount);
            appendSignPara(bufSignSrc, "orderCurrency", data.orderCurrency);
            appendSignPara(bufSignSrc, "orderDatetime", data.orderDatetime);
            appendSignPara(bufSignSrc, "orderExpireDatetime", "");
            appendSignPara(bufSignSrc, "productName", data.productName);
            appendSignPara(bufSignSrc, "productPrice", "");
            appendSignPara(bufSignSrc, "productNum", "");
            appendSignPara(bufSignSrc, "productId", "");
            appendSignPara(bufSignSrc, "productDesc", "");
            appendSignPara(bufSignSrc, "ext1", "");
            appendSignPara(bufSignSrc, "ext2", "");
            appendSignPara(bufSignSrc, "extTL", "");
            appendSignPara(bufSignSrc, "payType", data.payType);
            appendSignPara(bufSignSrc, "issuerId", "");
            appendLastSignPara(bufSignSrc, "key", MD5Key);

            var srcMsg = bufSignSrc.ToString();
            data.signMsg = MD5(srcMsg).ToUpper();

            return data;

        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["orderNo"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var Request = request.Params;

            var merchantId = Request["merchantId"];
            var version = Request["version"];
            var language = Request["language"];
            var signType = Request["signType"];
            var payType = Request["payType"];
            var issuerId = Request["issuerId"];
            var mchtOrderId = Request["mchtOrderId"];
            var orderNo = Request["orderNo"];
            var orderDatetime = Request["orderDatetime"];
            var orderAmount = Request["orderAmount"];
            var payDatetime = Request["payDatetime"];
            var payAmount = Request["payAmount"];
            var ext1 = Request["ext1"];
            var ext2 = Request["ext2"];
            var payResult = Request["payResult"];

            var signMsg = Request["signMsg"];

            StringBuilder bufSignSrc = new StringBuilder();

            appendSignPara(bufSignSrc, "merchantId", merchantId);
            appendSignPara(bufSignSrc, "version", version);
            appendSignPara(bufSignSrc, "language", language);
            appendSignPara(bufSignSrc, "signType", signType);
            appendSignPara(bufSignSrc, "payType", payType);
            appendSignPara(bufSignSrc, "issuerId", issuerId);
            appendSignPara(bufSignSrc, "mchtOrderId", mchtOrderId);
            appendSignPara(bufSignSrc, "orderNo", orderNo);
            appendSignPara(bufSignSrc, "orderDatetime", orderDatetime);
            appendSignPara(bufSignSrc, "orderAmount", orderAmount);
            appendSignPara(bufSignSrc, "payDatetime", payDatetime);
            appendSignPara(bufSignSrc, "payAmount", payAmount);
            appendSignPara(bufSignSrc, "ext1", ext1);
            appendSignPara(bufSignSrc, "ext2", ext2);
            appendSignPara(bufSignSrc, "payResult", payResult);
            appendLastSignPara(bufSignSrc, "key", MD5Key);

            String srcMsg = bufSignSrc.ToString();

            bool ret = MD5(srcMsg).ToUpper() == signMsg;

            return ret;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["payResult"] == "1";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["payResult"] == "1" ? "支付成功" : "支付失败";
        }



        private bool isEmpty(String src)
        {
            if (null == src || "".Equals(src) || "-1".Equals(src))
            {
                return true;
            }
            return false;
        }

        private void appendSignPara(System.Text.StringBuilder buf, String key, String value)
        {
            if (!isEmpty(value))
            {
                buf.Append(key).Append('=').Append(value).Append('&');
            }
        }

        private void appendLastSignPara(System.Text.StringBuilder buf, String key,
            String value)
        {
            if (!isEmpty(value))
            {
                buf.Append(key).Append('=').Append(value);
            }
        }

        private string MD5(string rawStr)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] FromData = System.Text.Encoding.GetEncoding("utf-8").GetBytes(rawStr);
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
