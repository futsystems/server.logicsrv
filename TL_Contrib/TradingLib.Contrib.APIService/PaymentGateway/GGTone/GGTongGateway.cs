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

namespace TradingLib.Contrib.Payment.GGTong
{
    public class GGTongGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("GGTongGateWay");

        public GGTongGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.GGTong;
            var data = config.Config.DeserializeObject();

            this.PayUrl = data["PayUrl"].ToString();
            this.Partner = data["Partner"].ToString();
            this.UserSeller = data["UserSeller"].ToString();
            this.MD5Key = data["MD5Key"].ToString();
            this.Domain = data["Domain"].ToString();

            this.PayDirectUrl = this.PayDirectUrl.Replace(APIGlobal.LocalIPAddress, this.Domain);
        }

        string PayUrl = "http://www.gigitong.com/PayOrder/payorder";
        string Partner = "455263594803353";
        string UserSeller = "392058";
        string MD5Key = "uqG5HmQFT6TNgigSz7PPQpiv9CQNUzXx";
        string Domain = "huichenyibai.com";

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropGGTongPayment data = new DropGGTongPayment();

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            data.partner = this.Partner;
            data.user_seller = this.UserSeller;
            data.out_order_no = operatioin.Ref;
            data.subject = "充值";
            data.total_fee = operatioin.Amount.ToFormatStr();
            data.body = "账户充值";
            data.return_url = APIGlobal.CustNotifyUrl + "/ggtong";
            data.notify_url = APIGlobal.SrvNotifyUrl + "/ggtong";
            data.return_url = data.return_url.Replace(APIGlobal.LocalIPAddress, this.Domain);
            data.notify_url = data.notify_url.Replace(APIGlobal.LocalIPAddress, this.Domain);

            data.pay_type = "3";
            data.banktype = "";

            string rawStr = string.Format("body={0}&notify_url={1}&out_order_no={2}&partner={3}&return_url={4}&subject={5}&total_fee={6}&user_seller={7}",
                data.body,data.notify_url,data.out_order_no,data.partner,data.return_url,data.subject,data.total_fee,data.user_seller);

            data.sign = Sign(rawStr+MD5Key);

            return data;

        }

        public static string Sign(string rawStr)
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

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["out_order_no"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var Request = request.Params;

            string out_order_no = Request["out_order_no"];
            string total_fee = Request["total_fee"];
            string trade_status = Request["trade_status"];
            string sign = Request["sign"];

            string rawStr = out_order_no + total_fee + trade_status + this.Partner + this.MD5Key;


            string nsign = Sign(rawStr);
            return (sign == nsign);
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["trade_status"] == "TRADE_SUCCESS";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["trade_status"] == "TRADE_SUCCESS" ? "支付成功" : "支付失败";
        }

            
    }
}
