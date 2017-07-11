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

        public string MerchantID = "000110048160186";

        public string PrivateKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAOsjrXtJEWsNM1kwcJvszNTXuDxYmxTI891PIsHLP6eVpIRE+Cad7kkf4ctBjsnQNXh6KxMqh8uWddPjzLAZ+v3eOksEqVwGXWOOPiX47LkAL3YWOQdI8lKg82N4fKyazCyu5ohPRA8KN1x8NIZ96vrBYBg0NFITHNULesU064Q3AgMBAAECgYB83ZtYZrrbME1eRXznMF6tgEiTszHXXccydL2uT7Jj2fFdVAq65w8MweNzvkKHJQvgCHArY2BtWl8DwGqH8aP4e3Xak7ri4Xryizekmy1okRUMQjI81fSI9ft89kZRvZUSCbKO8iPyHVv7gezWgYQMnOdiGKgH6KNWbKKFm9o2gQJBAP5HSDwMc84d1E/9IiYG2tQPSJqFh1HZO+uoXMpKlgC+9cPx7PRNRRxtZTRoIhTxhEO9FcDZ+S2j9G33mx6RO+ECQQDsuzk0pUYMguj0WvA8g4k8z8wYvveseDzlbUDlk363xzf06TcW20ad0Jv6zxxZXHUlHatP9JTi2+jPWVZDm4MXAkEA/jxbNt7kcrNUcl2P5SF5bgmR+B2F/QoMZUTC8ee9LyW/KJkc5+7SKOggBlQPr40DR48ozteh0kZZwBGPYPKpQQJBAM0eGB4Oilhy4YhAd2HnkvT3E9/pL87ny9P6yz1ghG6WllyF6m7KhjBdvZke9XBUVUhYocRApvS+GMX4oW8pAvcCQFNU2kJOzxAId+1Yj+s+NrAG0P5VHmqxN0xeCjAJ36f0jctMIaHjG3s8uK8PeSOT33vgWIJqNvdxsfh0X7hN0Ys=";

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

            param.Add("meraOrderId", data.merOrderId);
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

            string signSrc = GaoHuiTongHelper.CreateLinkString(sPara);

            signSrc = signSrc.Replace("meraOrderId", "merOrderId");

            signSrc = "charset=UTF-8&clientIp=127.0.0.1&currency=CNY&merOrderId=636353865776837580&merchantId=000110048160186&notifyUrl=http://117.90.54.37:9999/cash/srvnotify/gaohuitong&payType=01&productDesc=会员购物卡&productName=充值卡&returnUrl=http://117.90.54.37:9999/cash/custnotify/gaohuitong&serviceName=ebankPayApi&tranAmt=1.00&tranTime=20170711162147&version=V1";
            //string key = DinpayHelper.RSAPrivateKeyJava2DotNet(this.PrivateKey);
            //string signdata = DinpayHelper.RSASign(signSrc, key);

            data.sign = RSAHelper.sign(signSrc, this.PrivateKey);

            var sign2 = GaoHuiTongHelper.BuildMysignRSA(sPara, "UTF-8");
            bool same = data.sign == GaoHuiTongHelper.BuildMysignRSA(sPara, "UTF-8");
            string pubkey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDrI617SRFrDTNZMHCb7MzU17g8WJsUyPPdTyLByz+nlaSERPgmne5JH+HLQY7J0DV4eisTKofLlnXT48ywGfr93jpLBKlcBl1jjj4l+Oy5AC92FjkHSPJSoPNjeHysmswsruaIT0QPCjdcfDSGfer6wWAYNDRSExzVC3rFNOuENwIDAQAB";
            bool valid = RSAHelper.verify(signSrc, data.sign, pubkey);
            // data.sign = GaoHuiTongHelper.BuildMysignMD5(sPara, "UTF-8");

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
