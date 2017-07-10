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
using TradingLib.Contrib.APIService;

namespace TradingLib.Contrib.Payment.DDBill
{
    public class DDBilPayGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("DDBilPayGateWay");

        public DDBilPayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.DDBillPay;

            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString(); 
            this.SuccessReponse = "SUCCESS";
            this.MerchantCode = data["MerCode"].ToString();
            this.PrivateKey = data["MerPrivateKey"].ToString();
            this.ZhiHPayKey = data["PublickKey"].ToString();

            var val = data["Domain"];
            this.Domain = val == null ? string.Empty : val.ToString();
            this.PayDirectUrl = this.PayDirectUrl.Replace(APIGlobal.LocalIPAddress, this.Domain);


        }

        public string MerchantCode = "1111110166";
        public string PrivateKey ="MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALf/+xHa1fDTCsLYPJLHy80aWq3djuV1T34sEsjp7UpLmV9zmOVMYXsoFNKQIcEzei4QdaqnVknzmIl7n1oXmAgHaSUF3qHjCttscDZcTWyrbXKSNr8arHv8hGJrfNB/Ea/+oSTIY7H5cAtWg6VmoPCHvqjafW8/UP60PdqYewrtAgMBAAECgYEAofXhsyK0RKoPg9jA4NabLuuuu/IU8ScklMQIuO8oHsiStXFUOSnVeImcYofaHmzIdDmqyU9IZgnUz9eQOcYg3BotUdUPcGgoqAqDVtmftqjmldP6F6urFpXBazqBrrfJVIgLyNw4PGK6/EmdQxBEtqqgXppRv/ZVZzZPkwObEuECQQDenAam9eAuJYveHtAthkusutsVG5E3gJiXhRhoAqiSQC9mXLTgaWV7zJyA5zYPMvh6IviX/7H+Bqp14lT9wctFAkEA05ljSYShWTCFThtJxJ2d8zq6xCjBgETAdhiH85O/VrdKpwITV/6psByUKp42IdqMJwOaBgnnct8iDK/TAJLniQJABdo+RodyVGRCUB2pRXkhZjInbl+iKr5jxKAIKzveqLGtTViknL3IoD+Z4b2yayXg6H0g4gYj7NTKCH1h1KYSrQJBALbgbcg/YbeU0NF1kibk1ns9+ebJFpvGT9SBVRZ2TjsjBNkcWR2HEp8LxB6lSEGwActCOJ8Zdjh4kpQGbcWkMYkCQAXBTFiyyImO+sfCccVuDSsWS+9jrc5KadHGIvhfoRjIj2VuUKzJ+mXbmXuXnOYmsAefjnMCI6gGtaqkzl527tw=";
        public string ZhiHPayKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCJQIEXUkjG2RoyCnfucMX1at7OPtOCDSiKZhtzHw5HOjXKteBpYBqEBOZc9pNjP/fKbvBNZ3Z7XxUn5ECfQbPCtH9y++c0WxAYPoZiPDEYeQmRJfqPR68c0aAtZN5Kh7H1SI2ZRvoMUdZGvvFy3vuPnTwm3R+aHq17bch/0ZAudwIDAQAB";

        public string PayUrl = "https://pay.ddbill.com/gateway?input_charset=UTF-8";

        public string Domain { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropDDBilPayment data = new DropDDBilPayment();
            data.input_charset = "UTF-8";
            data.interface_version = "V3.3";
            data.merchant_code = this.MerchantCode;
            

            data.return_url = APIGlobal.CustNotifyUrl + "/ddbillpay";
            data.notify_url = APIGlobal.SrvNotifyUrl + "/ddbillpay";

            data.order_amount = operatioin.Amount.ToFormatStr();
            data.order_no = operatioin.Ref;
            data.order_time = Util.ToDateTime(operatioin.DateTime).ToString("yyyy-MM-dd HH:mm:ss");
            data.pay_type = "b2c";
            data.product_name = "充值卡";
            data.service_type = "direct_pay";

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            string signSrc = "";

            //组织订单信息
            signSrc = signSrc + "input_charset=" + data.input_charset + "&";
            signSrc = signSrc + "interface_version=" + data.interface_version + "&";
            signSrc = signSrc + "merchant_code=" + data.merchant_code + "&";
            signSrc = signSrc + "notify_url=" + data.notify_url + "&";
            signSrc = signSrc + "order_amount=" + data.order_amount + "&";
            signSrc = signSrc + "order_no=" + data.order_no + "&";
            signSrc = signSrc + "order_time=" + data.order_time + "&";
            signSrc = signSrc + "pay_type=" + data.pay_type + "&";
            signSrc = signSrc + "product_name=" + data.product_name + "&";
            signSrc = signSrc + "return_url=" + data.return_url + "&";
            signSrc = signSrc + "service_type=" + data.service_type;



            string key = DDBillPayHelper.RSAPrivateKeyJava2DotNet(this.PrivateKey);
            string signdata = DDBillPayHelper.RSASign(signSrc, key);

            data.sign = signdata;
            data.sign_type = "RSA-S";



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
            signStr = signStr + "interface_version=V3.3" + "&";
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
            var key = DDBillPayHelper.RSAPublicKeyJava2DotNet(this.ZhiHPayKey);
            //验签
            bool result = DDBillPayHelper.ValidateRsaSign(signStr, key, dinpaysign);
            return true;
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
