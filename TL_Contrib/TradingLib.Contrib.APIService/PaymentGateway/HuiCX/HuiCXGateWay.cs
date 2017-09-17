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

namespace TradingLib.Contrib.Payment.HuiCX
{
    public class HuiCXGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("HuiCXGateWay");

         public HuiCXGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.HuiCX;

            var data = config.Config.DeserializeObject();

            this.PlatformID = data["MerNo"].ToString(); //"2110011495172706";
            this.MerNo = data["MerNo"].ToString(); //"2110011495172706";
            this.PayUrl = data["PayUrl"].ToString();// "http://saascashier.mobaopay.com/cgi-bin/netpayment/pay_gate.cgi";// "https://trade.mobaopay.com/cgi-bin/netpayment/pay_gate.cgi";

            this.MD5Key = data["MD5Key"].ToString(); //"5feb16286fa46e3af84863d9722af75e";
            this.SuccessReponse = "SUCCESS";

            try
            {
                this.Domain = data["Domain"].ToString();
            }
            catch (Exception ex)
            { 
            
            }

            this.merchUrl = APIGlobal.SrvNotifyUrl + "/huicx";
            this.merchUrl = this.merchUrl.Replace(APIGlobal.LocalIPAddress, this.Domain);
            this.PayDirectUrl = this.PayDirectUrl.Replace(APIGlobal.LocalIPAddress, this.Domain);
        }

        string Domain { get; set; }

        public string PlatformID { get; set; }

        public string MerNo { get; set; }

        public string MD5Key { get; set; }


        public string PayUrl { get; set; }

        string merchUrl { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropMoBoPayment data = new DropMoBoPayment();

            data.apiName = "WEB_PAY_B2C";
            data.apiVersion = "1.0.0.0";
            data.platformID = this.PlatformID;
            data.merchNo = this.MerNo;
            data.orderNo = operatioin.Ref;
            data.tradeDate = Util.ToTLDate().ToString();
            data.amt = operatioin.Amount.ToFormatStr();
            data.merchUrl = this.merchUrl;//APIGlobal.SrvNotifyUrl + "/mobopay";
            data.tradeSummary = "充值";

            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);
            data.PayUrl = this.PayUrl;


            // 组织数据和签名
            Dictionary<string, string> payData = new Dictionary<string, string>();
            payData.Add("apiName", data.apiName);
            payData.Add("apiVersion", data.apiVersion);
            payData.Add("platformID", data.platformID);
            payData.Add("merchNo", data.merchNo);
            payData.Add("orderNo", data.orderNo);
            payData.Add("tradeDate", data.tradeDate);
            payData.Add("amt", data.amt);
            payData.Add("merchUrl", data.merchUrl);
            payData.Add("merchParam", string.Empty);
            payData.Add("tradeSummary", data.tradeSummary);
            string requestStr = MobaopayMerchant.Instance.generatePayRequest(payData);  // 组织签名源数据

            data.signMsg = MobaopaySignUtil.Instance.sign(requestStr, this.MD5Key);

            return data;
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            string transid = queryString["orderNo"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            //获取智付反馈信息
            var dict = request.Params;
            // 验证签名，先获取到签名源字符串和签名字符串后，做签名验证。
            string srcString = string.Format("apiName={0}&notifyTime={1}&tradeAmt={2}&merchNo={3}&merchParam={4}&orderNo={5}&tradeDate={6}&accNo={7}&accDate={8}&orderStatus={9}",
                    dict["apiName"],
                    dict["notifyTime"],
                    dict["tradeAmt"],
                    dict["merchNo"],
                    dict["merchParam"],
                    dict["orderNo"],
                    dict["tradeDate"],
                    dict["accNo"],
                    dict["accDate"],
                    dict["orderStatus"]);
            string sigString = dict["signMsg"];
            string notifyType = dict["notifyType"];

            sigString = sigString.Replace("\r", "").Replace("\n", "");
            bool verifyResult = MobaopaySignUtil.Instance.verifyData(sigString, srcString,this.MD5Key);
            return verifyResult;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["orderStatus"] == "1";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            string ResultDesc = queryString["orderStatus"];

            return ResultDesc;
        }

    }
}
