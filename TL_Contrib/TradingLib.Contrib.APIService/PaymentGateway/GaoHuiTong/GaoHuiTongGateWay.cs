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
        public string PayUrl = "http://gateway.gaohuitong.com/gateway/ebankPayApi/pay"; //"http://gateway.gaohuitong.com/gateway/ebankPayApi/pay";

        public string MerchantID = "000440048160482";

        public string PrivateKey = "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBAMSSi3AI3UsBVXaIr3AxhrWRmv6DPE6x/7XPMOTTSIylwqRuVncGsB81iFlOI5p8avUbhJ0EbiFHx+hb9T3SK7/mPHvj7IKg9TMDlz9AFBeErvveHQ63PMsYmex5AeOo4ZCEpEGLr3YdOMMBkMYafL4jjfELV8b4+SiYh6bh7N/ZAgMBAAECgYBzPAFX/SpVNQP3x+so1y/EMwZsm1x4OarQ1xQvoJiQMwAmyrQPdLE8qnIzADao8rT02VvlfFpfUigj0/yxcArKnleMUTTD3+rj2B5DhRQ4FVim31+8rGS4q/MoUsBbTupLQC11kdW46Sk7K4tt0QtgWPC3Rmco2cctD7Tyr/rQXQJBAOBfnKoAa49aMeNoAYzI25ArbCNKTC1OoRNPL1I5wkXTZkw4uVl8AKCwYvsWUDh1MNcuZJSnlh4T98pxnpc19b8CQQDgR79/NsydSNwmj4d0dcWmX9S8R0uXEV959kdiOlH3vpHizelkiL6+i7fIi4Xg5vBVeZgoNEU9HPiFM7p4iQBnAkEAomcSMkqTj/Ms4PjLBmfr5HJJl1GTycd790n9anq8D2ZwSQNVxVtn6OdC/ZKtBfBtJZNC4gZbNfImDLYooDJCaQJBAIIIdzmdnwJuFN3yh3l5MvAkTaYfu+7Jbs66gGauI4n9Hn4eHnxgYB7/yL1oT2W2347fId3leGNXYatw82MhI28CQQC4uRHySQq1+p3zyTRxo/nyMi6dze45MhXNJaxG2NfSwOTkVpT296irgJtzPGRLk1ZvD7VwCFEhC/nDZUKvANfD";
        
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

            //string signSrc = GaoHuiTongHelper.CreateLinkString(sPara);

            //string key = DinpayHelper.RSAPrivateKeyJava2DotNet(this.PrivateKey);
            //string signdata = DinpayHelper.RSASign(signSrc, key);

            data.sign = GaoHuiTongHelper.BuildMysignRSA(sPara, "UTF-8");
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
