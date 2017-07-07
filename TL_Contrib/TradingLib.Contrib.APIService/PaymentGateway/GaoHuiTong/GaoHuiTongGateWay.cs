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

namespace TradingLib.Contrib.Payment.GaoHuiTong
{
    public class GaoHuiTongGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("GaoHuiTongGateWay");

        public GaoHuiTongGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.GaoHuiTong;


            GaoHuiTongHelper.PublicKey = this.PublicKey;
            GaoHuiTongHelper.PrivateKey = this.PrivateKey;
            GaoHuiTongHelper.MD5Key = this.MD55Key;

            
        }
        //"http://106.120.193.133/gateway/ebankPayApi/pay";
        public string PayUrl = "http://106.120.193.133/gateway/ebankPayApi/pay"; //"http://gateway.gaohuitong.com/gateway/ebankPayApi/pay";

        public string MerchantID = "000110048160020";

        public string PrivateKey = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCftDNUhCN/444xUiIkV+dxH6lb7of4nkmcvpygKImyepNwusRcvTdtZp/z+K/IHOcEYGS8iSSioM1ZWX1uStXPx4Zo9nYBBcLd3f4TuAF2dzwh1CiVVUV5Ve0xYJD6CHp3Z0Dw0xoKYooPpYUpwvtsDmkus8TfA1bABWizCsCc6Tsqswd15sxqc6+XuCxc97HAwPME8zZLtK7RUGn84P5Kf6zvLqjqUhG0gpjZfFAsNHNvUSvza8Y9obNrKaVGvRWYxaicoaPJH0ZxXmegNnV9VZNZXB982EtUEwoPk0ufz58RQiDnWA5QawIknRXDLUyMoy0F8QoIsvKjzI6GpmTXAgMBAAECggEAWDYpkAo4rYAcX0O1lgtzy/koC55SPlH36Psj+hbKD+pCnCadJXhiMCxaN2Dqfwbv12wC2FyL/sQBCNQ0QwJU3SKhLELN5TywaOogV/Xv4OZ1MV5FWE60RBPhIr/q9CBQvLkslpiTrp7FEWVkiy+mvgWrtV8YY/ItLX0PWq2avE/3MXlhBPorM63rMOwO9DwfhCHnYHabe1eWCw8APfJqeETM4IAah5G7ySAaX3uqziSYDO1uSIEBVxtlEcIXlDrA9Sj4TR0fXzkc3ozoutgpk+DTUz23FcEyK+YrkVRi4BXkQDsAH7YNCUw2FSZulkUyKHryakPNX8+V9MmrbL7AsQKBgQD3WGfcnrtC1xSPLDCOkd2C1Juys13zIv/4DxYH1dLq+0bdIKIbH/7i41IAAMcetGICRSFlAOT0oisqOMZbpK6QGqH8V0esEoDp4loBjKpeNzFWHm+sDULuC/SO3uw897VMKGN6yxr8IemRgEZFbn1KDx74Qcgnt6RpIG3wRG4wWQKBgQClSr9M8HOCEPZ0YVK0VG7qN0oxHkoxeOUiHxyUbYtRY86BDsqYspxP5qsYVGPZTXNki2xpQCsU52A+YYAf+8/7djDfhW9g1iKZv4aV5iq2mzVAxdT6AYimQAiIj0iYSCiM5jrmncEX30arOVfI1ing+KHoykEWSNixRPgAb8MYrwKBgQDJh/ioI5UEcuZHeYPexi6r6LsrsUW9UykoXnJe0/PUjgRBK9OpMjqldv5bDkcvV13754O8HixuvqtY7YWBKf8pXunZBuxY4YK0Dj+zv38Y4POL7aSjlPKRrqAGwM/PJS1M7iOP62kDQkZizRd0fwAKlaNwN3j0E4ccONYazEbTkQKBgASmmcNccKOUPpr/sggI6CYG8Dt5krTZpfjTz0YN3wGnQUQ4WlL5k5Rb9Sx2E2kl2L1XfvFnMM8hw3991tEPkMaOiMmBQ6UB4W9aCDtngoQo0dLEvj1albG304WkInLGdP2h7L5Yafp1+dMPhfzMqdj+pe+a4UHzQsWaHzBD9MVZAoGAEzN1Qsx0U/jdSGQUrb2XBHQq/c0PAcZ8VdXYRO2LoG9bk2DSXx+OcITXs8bUS+DV3HJGZ37ZT/vn21K+egxZzh9NHYvrUn9B19OjSjcEdSoGm3bLT8xvOMePyA/EYKjCx7umM0ozDmrmEo76xu4gLcWfdQVyT4YJnwLsCYCiL7A=";

        public string PublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCWY9ClHmqFbsTCr1N/uKjX5GWJgLqZG0Uod8KiAeQAtdlThI2BUNLwv5JwR6mqfOww+RagpXipWfhbyemkSwlxlRgW1zAlxEnKF7WciM65OTxVReG+FnSTetHBgxETgDN4hRDa5eXYcLYyLEAjBOmkEqaPxG/THGwqh8lXkphpIwIDAQAB";

        public string MD55Key = "HnahbpRDDzFGr8213GC3uA==";

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropGaoHuiTongPayment data = new DropGaoHuiTongPayment();

            data.serviceName = "ebankPayApi";
            data.version = "V1";
            data.platform = string.Empty;
            data.merchantId = this.MerchantID;

            data.signType = "RSA";
            //data.signType = "MD5";

            data.payType = "01";
            data.charset = "UTF-8";

            data.merOrderId = operatioin.Ref;
            data.currency = "CNY";
            data.returnUrl = APIGlobal.CustNotifyUrl + "/gaohuitong";
            data.notifyUrl = APIGlobal.SrvNotifyUrl + "/gaohuitong";

            data.productName = "充值卡";
            data.productDesc = "会员购物卡";
            data.tranAmt = operatioin.Amount.ToFormatStr();
            data.tranTime = operatioin.DateTime.ToString();

            data.bankCardType = "01";
            data.bankCode = "CMB";
            data.clientIp ="127.0.0.1";
            data.payerId = string.Empty;

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);



            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("serviceName", data.serviceName);
            param.Add("version", data.version);
            param.Add("platform", data.platform);
            param.Add("merchantId", data.merchantId);

            param.Add("signType", data.signType);
            param.Add("payType", data.payType);
            param.Add("charset", data.charset);

            param.Add("merOrderId", data.merOrderId);
            param.Add("currency", data.currency);
            param.Add("notifyUrl", data.notifyUrl);
            param.Add("returnUrl", data.returnUrl);

            param.Add("productName", data.productName);
            param.Add("productDesc", data.productDesc);
            param.Add("tranAmt", data.tranAmt);
            param.Add("tranTime", data.tranTime);


            // param.Add("sign",this.sign.Text.Trim());
            param.Add("clientIp", data.clientIp);
            param.Add("payerId",data.payerId);

            //待签名请求参数数组
            Dictionary<string, string> sPara = new Dictionary<string, string>();
            //过滤签名参数数组
            sPara = GaoHuiTongHelper.FilterPara(param);
            //data.sign = GaoHuiTongHelper.BuildMysignRSA(sPara, "UTF-8");
            data.sign = GaoHuiTongHelper.BuildMysignMD5(sPara, "UTF-8");

            return data;

        }


        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["merOrderId"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            //var queryString = request.Params;
            //int orderstatus = Convert.ToInt32(queryString["orderstatus"]);
            //string ordernumber = queryString["ordernumber"];
            //string paymoney = queryString["paymoney"];
            //string sign = queryString["sign"];
            //string attach = queryString["attach"];
            //string signSource = string.Format("partner={0}&ordernumber={1}&orderstatus={2}&paymoney={3}{4}", this.PartnerID, ordernumber, orderstatus, paymoney, this.MD5Key);
            //bool ret = sign.ToUpper() == FZPayHelper.MD5(signSource, false).ToUpper();
            //return ret;
            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["retCode"] == "0000";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["retMsg"].ToString();
        }


    }
}
