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

namespace TradingLib.Contrib.Payment.C9Pay
{
    public class C9PayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("C9PayGateWay");

        public C9PayGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.C9Pay;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.MerID = data["MerID"].ToString();
            this.MerKey = data["MD5Key"].ToString();
        }
        string PayUrl = "http://cli.c9pay.com/deal_interGetWay.action";
        string MerID = "170928182529173w";
        string MerKey = "7C2C21F8EE1334AEE4913C1B550932029ADB0DB7775709CA";


        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropC9Payment data = new DropC9Payment();
            data.PayUrl = this.PayUrl;
            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            data.MerchantNo = this.MerID;
            data.OrderNo = operatioin.Ref;
            data.AmountI = operatioin.Amount.ToFormatStr();
            data.GoodsInfo = "Credits";
            data.ReturnUrl = APIGlobal.CustNotifyUrl + "/c9pay";
            data.NotifyUrl = APIGlobal.SrvNotifyUrl + "/c9pay";

            data.BankCode = ConvBankCode(operatioin.Bank);

            string rawStr = data.MerchantNo + data.OrderNo + data.AmountI + data.GoodsInfo + data.ReturnUrl + data.NotifyUrl + data.BankCode;

            data.signMd5 = MD5Helper.MD5Sign(rawStr, this.MerKey);

            return data;
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
                case "03080000": return "CMBCHINA";
                //交通
                case "03010000": return "BOCO";
                //邮政
                case "01000000": return "POST";
                //中信银行
                case "03020000": return "ECITIC";
                //光大
                case "03030000": return "CEB";
                //民生
                case "03050000": return "CMBC";
                default:
                    return "ICBC";

            }
        }


        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["OrderNo"];
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

            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["Status"] == "10000";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["Status"] == "10000" ? "支付成功" : "支付失败";
        }
    }
}
