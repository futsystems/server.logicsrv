using Common.Logging;
using DotLiquid;
using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;

namespace TradingLib.Contrib.Payment.SumPay
{
    public class SumPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("SumPayGateWay");

        public SumPayGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.SumPay;
            var data = config.Config.DeserializeObject();

        }

        string PayUrl ="https://www.sumapay.com/sumapay/pay_bankPayForNoLoginUser";// "https://www.sumapay.com/sumapay/unitivepay_bankPayForNoLoginUser";
        string MerID = "3610000019";
        string MerKey = "gS1z2EdeGFCtPcTfOwuAOCTnEahyktTx";
        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DroSumPayment data = new DroSumPayment();

            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            data.requestId = operatioin.Ref;
            data.tradeProcess = this.MerID;
            data.totalBizType = "BIZ01106";
            data.totalPrice = operatioin.Amount.ToFormatStr();
            data.bankcode = "cmb";

            data.returnurl = APIGlobal.CustNotifyUrl + "/sumpay";
            data.backurl = APIGlobal.CustNotifyUrl + "/sumpay";
            data.noticeurl = APIGlobal.SrvNotifyUrl + "/sumpay";
            data.description = "Credits";


            data.productId = "100";
            data.productName = "Product";
            data.productNumber = "1";
            data.fund = operatioin.Amount.ToFormatStr();
            data.bizType = "BIZ01106";
            data.merAcct = this.MerID;
            data.PayUrl = this.PayUrl;


            //调用签名函数生成签名串
            var sbOld = new StringBuilder();
            sbOld.Append(data.requestId);
            sbOld.Append(data.tradeProcess);
            sbOld.Append(data.totalBizType);
            sbOld.Append(data.totalPrice);
            sbOld.Append(data.backurl);
            sbOld.Append(data.returnurl);
            sbOld.Append(data.noticeurl);
            sbOld.Append(data.description);

            //生成签名字符串
            var signatrue = SumPayHelper.GenSign(sbOld.ToString(), MerKey);

            data.mersignature = signatrue;
            return data;
        }


        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["r6_Order"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var Request = request.Params;

            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["r1_Code"] == "1";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["r1_Code"] == "1" ? "支付成功" : "支付失败";
        }

    }
}
