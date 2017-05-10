using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using DotLiquid;
using System.Net;
using System.IO;
using System.Security;
using System.Security.Cryptography;

namespace TradingLib.Contrib.APIService
{
    public class DinpayGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("UnspayGateWay");

        public DinpayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.DinPay;

            try
            {
                var data = config.Config.DeserializeObject();
                this.PayUrl = data["PayUrl"].ToString(); //"https://pay.dinpay.com/gateway?input_charset=UTF-8";
                this.SuccessReponse = "SUCCESS";
                this.MerchantCode = data["MerCode"].ToString();
                this.PrivateKey = data["MerPrivateKey"].ToString();
                this.DinPayKey = data["DinPublickKey"].ToString();
                
                var val = data["Domain"];
                this.Domain = val == null ? string.Empty : val.ToString();
                this.PayDirectUrl = this.PayDirectUrl.Replace(APIGlobal.LocalIPAddress, this.Domain);

            }
            catch (Exception ex)
            {
                logger.Error("Create Gateway Error:" + ex.ToString());
            }
        }

        public string MerchantCode ="1111110166";
        public string PrivateKey ="MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALf/+xHa1fDTCsLYPJLHy80aWq3djuV1T34sEsjp7UpLmV9zmOVMYXsoFNKQIcEzei4QdaqnVknzmIl7n1oXmAgHaSUF3qHjCttscDZcTWyrbXKSNr8arHv8hGJrfNB/Ea/+oSTIY7H5cAtWg6VmoPCHvqjafW8/UP60PdqYewrtAgMBAAECgYEAofXhsyK0RKoPg9jA4NabLuuuu/IU8ScklMQIuO8oHsiStXFUOSnVeImcYofaHmzIdDmqyU9IZgnUz9eQOcYg3BotUdUPcGgoqAqDVtmftqjmldP6F6urFpXBazqBrrfJVIgLyNw4PGK6/EmdQxBEtqqgXppRv/ZVZzZPkwObEuECQQDenAam9eAuJYveHtAthkusutsVG5E3gJiXhRhoAqiSQC9mXLTgaWV7zJyA5zYPMvh6IviX/7H+Bqp14lT9wctFAkEA05ljSYShWTCFThtJxJ2d8zq6xCjBgETAdhiH85O/VrdKpwITV/6psByUKp42IdqMJwOaBgnnct8iDK/TAJLniQJABdo+RodyVGRCUB2pRXkhZjInbl+iKr5jxKAIKzveqLGtTViknL3IoD+Z4b2yayXg6H0g4gYj7NTKCH1h1KYSrQJBALbgbcg/YbeU0NF1kibk1ns9+ebJFpvGT9SBVRZ2TjsjBNkcWR2HEp8LxB6lSEGwActCOJ8Zdjh4kpQGbcWkMYkCQAXBTFiyyImO+sfCccVuDSsWS+9jrc5KadHGIvhfoRjIj2VuUKzJ+mXbmXuXnOYmsAefjnMCI6gGtaqkzl527tw=";
        public string DinPayKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCWOq5aHSTvdxGPDKZWSl6wrPpnMHW+8lOgVU71jB2vFGuA6dwa/RpJKnz9zmoGryZlgUmfHANnN0uztkgwb+5mpgmegBbNLuGqqHBpQHo2EsiAhgvgO3VRmWC8DARpzNxknsJTBhkUvZdy4GyrjnUrvsARg4VrFzKDWL0Yu3gunQIDAQAB";
        public string PayUrl { get; set; }

        public string Domain { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropDinpayment data = new DropDinpayment();

            data.InputChartset = "UTF-8";
            data.InterfaceVersion = "V3.0";
            data.MarchantCode = this.MerchantCode;

            data.ReturnUrl = APIGlobal.CustNotifyUrl + "/dinpay";
            data.NotifyUrl = APIGlobal.SrvNotifyUrl + "/dinpay";

            data.OrderAmount =operatioin.Amount.ToFormatStr("{0:F2}");
            data.OrderNo = operatioin.Ref;
            data.OrderTime = Util.ToDateTime(operatioin.DateTime).ToString("yyyy-MM-dd HH:mm:ss");
            data.PayType = "b2c";
            data.ProductName = "充值卡";
            data.ServiceType = "direct_pay";
            data.PayUrl = this.PayUrl;

            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);


            string signSrc = "";

            //组织订单信息
            signSrc = signSrc + "input_charset=" + data.InputChartset + "&";
            signSrc = signSrc + "interface_version=" + data.InterfaceVersion + "&";
            signSrc = signSrc + "merchant_code=" + data.MarchantCode + "&";
            signSrc = signSrc + "notify_url=" + data.NotifyUrl + "&";
            signSrc = signSrc + "order_amount=" + data.OrderAmount + "&";
            signSrc = signSrc + "order_no=" + data.OrderNo + "&";
            signSrc = signSrc + "order_time=" + data.OrderTime + "&";
            signSrc = signSrc + "pay_type=" + data.PayType + "&";
            signSrc = signSrc + "product_name=" + data.ProductName + "&";
            signSrc = signSrc + "return_url=" + data.ReturnUrl + "&";
            signSrc = signSrc + "service_type=" + data.ServiceType;

            string key = DinpayHelper.RSAPrivateKeyJava2DotNet(this.PrivateKey);
            string signdata = DinpayHelper.RSASign(signSrc, key);

            data.Sign = signdata;
            data.SignType = "RSA-S";



            return data;
        }


        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["order_no"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            //获取智付反馈信息
            var queryString = request.Params;
            string merchant_code = queryString["merchant_code"].ToString().Trim();
            string notify_type = queryString["notify_type"].ToString().Trim();
            string notify_id = queryString["notify_id"].ToString().Trim();
            string interface_version = queryString["interface_version"].ToString().Trim();
            string sign_type = queryString["sign_type"].ToString().Trim();
            string dinpaysign = queryString["sign"].ToString().Trim();
            string order_no = queryString["order_no"].ToString().Trim();
            string order_time = queryString["order_time"].ToString().Trim();
            string order_amount = queryString["order_amount"].ToString().Trim();
            string extra_return_param = queryString["extra_return_param"];
            string trade_no = queryString["trade_no"].ToString().Trim();
            string trade_time = queryString["trade_time"].ToString().Trim();
            string trade_status = queryString["trade_status"].ToString().Trim();
            string bank_seq_no = queryString["bank_seq_no"];
            /**
             *签名顺序按照参数名a到z的顺序排序，若遇到相同首字母，则看第二个字母，以此类推，
            *参数名1=参数值1&参数名2=参数值2&……&参数名n=参数值n
            **/
            //组织订单信息
            string signStr = "";

            if (null != bank_seq_no && bank_seq_no != "")
            {
                signStr = signStr + "bank_seq_no=" + bank_seq_no.ToString().Trim() + "&";
            }

            if (null != extra_return_param && extra_return_param != "")
            {
                signStr = signStr + "extra_return_param=" + extra_return_param + "&";
            }
            signStr = signStr + "interface_version=V3.0" + "&";
            signStr = signStr + "merchant_code=" + merchant_code + "&";


            if (null != notify_id && notify_id != "")
            {
                signStr = signStr + "notify_id=" + notify_id + "&notify_type=" + notify_type + "&";
            }

            signStr = signStr + "order_amount=" + order_amount + "&";
            signStr = signStr + "order_no=" + order_no + "&";
            signStr = signStr + "order_time=" + order_time + "&";
            signStr = signStr + "trade_no=" + trade_no + "&";
            signStr = signStr + "trade_status=" + trade_status + "&";

            if (null != trade_time && trade_time != "")
            {
                signStr = signStr + "trade_time=" + trade_time;
            }
            //将智付公钥转换成C#专用格式
            var key = DinpayHelper.RSAPublicKeyJava2DotNet(this.DinPayKey);
            //验签
            bool result = DinpayHelper.ValidateRsaSign(signStr, key, dinpaysign);
            return result;
        }


        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["trade_status"] == "SUCCESS";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            string ResultDesc = queryString["trade_status"];

            return ResultDesc;
        }

    }
}
