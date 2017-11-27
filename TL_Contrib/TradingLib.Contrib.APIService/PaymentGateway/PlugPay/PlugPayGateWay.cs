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

namespace TradingLib.Contrib.Payment.PlugPay
{
    public class PlugPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("PlugPayGateWay");

        public PlugPayGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.PlugPay;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.AppID = data["AppID"].ToString();
            this.AppSecret = data["AppSecret"].ToString(); 
        }

        string PayUrl = "http://yunrenjie.com/kepay/order/pay";
        string AppID = "100001";
        string AppSecret = "2b8f91abb97c4174a326602304544ce8";
        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropPlugPayPayment data = new DropPlugPayPayment();

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);
            data.Account = operatioin.Account;

            data.p1_app_id = AppID;
            data.p2_channel = "YEEPAY-PC";
            data.p3_bank_code = operatioin.Bank;
            data.p4_bill_no = operatioin.Ref;
            data.p5_total_fee = ((int)(100*operatioin.Amount)).ToString();
            data.p6_goods_title = "Credit";
            data.p7_goods_desc = "";
            data.p8_goods_extended = "";
            data.p9_timestamp = operatioin.DateTime.ToString();
            data.p10_return_url = APIGlobal.CustNotifyUrl + "/plugpay";
            data.p11_notify_url = APIGlobal.SrvNotifyUrl + "/plugpay";

            string rawStr = data.p1_app_id + data.p2_channel + data.p3_bank_code + data.p4_bill_no + data.p5_total_fee + data.p6_goods_title + data.p7_goods_desc + data.p8_goods_extended + data.p9_timestamp + data.p10_return_url + data.p11_notify_url;

            data.p0_sign = Sign(rawStr, this.AppSecret);

            return data;
        }

        public  string Sign(string strToBeEncrypt, string md5Key)
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
            return Byte2String.ToUpper();
        }


        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            string transid = queryString["r3_bill_no"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var Request = request.Params;

            string r1_app_id = Request["r1_app_id"];
            string r2_status = Request["r2_status"];
            string r3_bill_no = Request["r3_bill_no"];
            string r4_total_fee = Request["r4_total_fee"];
            string r5_date_time = Request["r5_date_time"];

            string hmac = Request["sign"];

            string rawStr = this.AppID + r2_status + r3_bill_no + r4_total_fee + r5_date_time;

            string nhmac = Sign(rawStr, this.AppSecret);
            return (nhmac == hmac);
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["r2_status"] == "1";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["r2_status"] == "1" ? "支付成功" : "支付失败";
        }
    }
}
