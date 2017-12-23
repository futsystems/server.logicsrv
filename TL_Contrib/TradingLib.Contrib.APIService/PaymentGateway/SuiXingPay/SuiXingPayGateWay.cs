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
using NHttp;
namespace TradingLib.Contrib.APIService
{
    public class SuiXingPayGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("SuiXingPayGateWay");

        public SuiXingPayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.SuiXingPay;

            var data = config.Config.DeserializeObject();
            this.MercNo = data["MerNo"].ToString(); //"600000000000707";
            this.PublickKey = data["PublickKey"].ToString(); //"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAu9vD3I9m++zgdVzNwfUTzJd+PoOnSvFv/Pc2jKCgxfIUHxNnY7QctymJxk/ZTcMVP1eURmoM6T5VpZzr/TOQjACJVLqoDqXDCSOBKIwPeVjl1nldsm6Gpbgi8A0qf72Nqy2f7MjciesLdYqpQUN4RY01lA4vnJEnZiFpEYEUxJCzaJZHhHY2z7PQEwaTXG/JxgJR1pkqHZUoqCg3YhYcBhQNXk4cslJvwIqgcdu38uXdBUft54OevLnWXRJMFyDxAaiLzcJ3zLc2LBzUhf/jxzfl6hkAXM4+h1fEssOrxA4z5vdhwV+rat4CvHZTo8j57LxRyCn8R1Q96DBQu+IEuwIDAQAB";

            this.PrivateKey = data["PrivateKey"].ToString();// "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBAMSSi3AI3UsBVXaIr3AxhrWRmv6DPE6x/7XPMOTTSIylwqRuVncGsB81iFlOI5p8avUbhJ0EbiFHx+hb9T3SK7/mPHvj7IKg9TMDlz9AFBeErvveHQ63PMsYmex5AeOo4ZCEpEGLr3YdOMMBkMYafL4jjfELV8b4+SiYh6bh7N/ZAgMBAAECgYBzPAFX/SpVNQP3x+so1y/EMwZsm1x4OarQ1xQvoJiQMwAmyrQPdLE8qnIzADao8rT02VvlfFpfUigj0/yxcArKnleMUTTD3+rj2B5DhRQ4FVim31+8rGS4q/MoUsBbTupLQC11kdW46Sk7K4tt0QtgWPC3Rmco2cctD7Tyr/rQXQJBAOBfnKoAa49aMeNoAYzI25ArbCNKTC1OoRNPL1I5wkXTZkw4uVl8AKCwYvsWUDh1MNcuZJSnlh4T98pxnpc19b8CQQDgR79/NsydSNwmj4d0dcWmX9S8R0uXEV959kdiOlH3vpHizelkiL6+i7fIi4Xg5vBVeZgoNEU9HPiFM7p4iQBnAkEAomcSMkqTj/Ms4PjLBmfr5HJJl1GTycd790n9anq8D2ZwSQNVxVtn6OdC/ZKtBfBtJZNC4gZbNfImDLYooDJCaQJBAIIIdzmdnwJuFN3yh3l5MvAkTaYfu+7Jbs66gGauI4n9Hn4eHnxgYB7/yL1oT2W2347fId3leGNXYatw82MhI28CQQC4uRHySQq1+p3zyTRxo/nyMi6dze45MhXNJaxG2NfSwOTkVpT296irgJtzPGRLk1ZvD7VwCFEhC/nDZUKvANfD";
            this.PayUrl = data["PayUrl"].ToString();// "https://api.suixingpay.com/onlinepay/pay";

            this.SuccessReponse = "SUCCESS";
        }

        public string MercNo { get; set; }
        public string PublickKey { get; set; }
        public string PrivateKey { get; set; }
        public string PayUrl { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operatioin, HttpRequest request)
        {
            DropSuiXingPayment data = new DropSuiXingPayment();
            data.mercNo = this.MercNo;
            data.tranCd = "1001";
            data.version = "1.0";
            data.ip = string.Empty;
            data.encodeType = "RSA#RSA";
            data.type = "1";
            data.PayUrl = this.PayUrl;

            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);
            data.PayUrl = this.PayUrl;

            var tmp = new
            {
                orderNo = operatioin.Ref,
                tranAmt = operatioin.Amount.ToFormatStr(),
                ccy = "CNY",
                pname = "充值",
                pnum = "1",
                pdesc = "测试商品",
                retUrl =  APIGlobal.CustNotifyUrl + "/suixingpay",
                notifyUrl = APIGlobal.SrvNotifyUrl + "/suixingpay",
            };

            string publicKey = SuiXingHelper.RSAPublicKeyJava2DotNet(this.PublickKey);
            string rawData = tmp.SerializeObject();
            string encData = Convert.ToBase64String(SuiXingHelper.RSAEncryptByPublicKey(Encoding.GetEncoding("UTF-8").GetBytes(rawData), publicKey));
            data.reqData = encData;

            var tmp2 = new
            {
                mercNo = data.mercNo,
                tranCd = data.tranCd,
                version = data.version,
                reqData = data.reqData,
                ip = data.ip
            };


            string privateKey = SuiXingHelper.RSAPrivateKeyJava2DotNet(this.PrivateKey);
            string rawData2 = tmp2.SerializeObject();
            string sign = SuiXingHelper.Sign(Encoding.GetEncoding("UTF-8").GetBytes(rawData2), privateKey);
            data.sign = sign;
            return data;

        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            string t = queryString["_t"];
            var data = t.DeserializeObject();

            string transid = data["orderNo"].ToString();
            return ORM.MCashOperation.SelectCashOperation(transid);
        }


        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            string t = request.Params["_t"];
            var data = t.DeserializeObject();
            var tmp = new
            {
                mercNo = data["mercNo"].ToString(),
                orderNo = data["orderNo"].ToString(),
                tranCd = data["tranCd"].ToString(),
                version =  "1.0",//data["version"].ToString(),
                resCode = data["resCode"].ToString(),
                resMsg = data["resMsg"].ToString(),
                resData = data["resData"].ToString(),
            };

            string publicKey = SuiXingHelper.RSAPublicKeyJava2DotNet(this.PublickKey);
            string rawData = tmp.SerializeObject();
            string sign = data["sign"].ToString();

            //return SuiXingHelper.Verify(Encoding.GetEncoding("UTF-8").GetBytes(rawData), Convert.FromBase64String(sign), publicKey);
            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            string t = request.Params["_t"];
            var data = t.DeserializeObject();
            return data["resCode"].ToString() == "000000";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            string t = request.Params["_t"];
            var data = t.DeserializeObject();
            string ResultDesc = data["resMsg"].ToString();

            return ResultDesc;
        }


    }
}
