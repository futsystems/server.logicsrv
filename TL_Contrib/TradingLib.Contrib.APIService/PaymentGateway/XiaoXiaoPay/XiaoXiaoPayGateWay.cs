using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;

using Common.Logging;
using DotLiquid;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;
using NHttp;

namespace TradingLib.Contrib.Payment.XiaoXiao
{
    public class XiaoXiaoPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("XiaoXiaoPayGateWay");

        public XiaoXiaoPayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.XiaoXiaoPay;

            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.MerchantId = int.Parse(data["MerID"].ToString());
            this.PrivateKey = data["PrivateKey"].ToString();
            this.PublicKey = data["PublickKey"].ToString();

            this.SuccessReponse = "SUCCESS";
        }

        string PayUrl = "http://api2.xiaoxiaopay.com:7500/order";
        int MerchantId = 10001015;
        string PrivateKey = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBANLhOMJ/bcs7zQbrKADGP6jjoyc/vxmySHNCTs76OenQtpzIrqzB8Wo7H8Lk78t2ZNaTDg9EGgsaF1UC5IgI+KGFX1N5yP6DXlE26UTROh0Cj+Wd0ghcduw+JvMOIse4aH4Okr/PbByF+v0T8Pb7aXV9aeTCbe/Wwi0Kv9raE2dtAgMBAAECgYBketIrwsIS83SW1leiPtQ4afbkjbhFyzBAUxjwAES2By/r9qdcQ7D/OBrDJ4imvsv/feRWG9H49j6l0BIKS5QDrOU86iNNxmmxTSSJ0ld9fp0PHLsOSxIWBHBWf4fRBiajp9aSEMLRSqIng44HRHayfTwwG2PTEkGhyJSkbvYfNQJBAO58MgMYGuY7D2ttgBPegXaqU7IEgZoDRO4gXETKgDdS1NPPTLXv0yBhuxB20m+90VRlGUIm1IOL2m/Ds1Fww7MCQQDiXgM56LGf4VbGZNB2vuEB74qDBbtfnxHTSiyS6EXyZo15rF94goZlDc7f5KlIu9t7KmmztnkmoCJmM9oMlhhfAkA2pVysWtSnFrdkzVWAuAaAU20UKHYt/TvJOL8LwD767k74LdMNCWQeUmaT0jZMBnm11eigwNcHQX+PBo/LRISDAkBbt+/itM/TkgAQ3qRAmNKecCpVMCtdcRxN2g35cDd3IepM1HTSZUCDDXc27HVSzRr+6o5tjI/GALgvZO8CjoZNAkEApxvCzIkOYgmeQim+5CLO7DKyhtboF96QeY5fI99od9dtuqqRWMV5Smxyaz3Eo6EgZE9WFDapwiRFiveeEX/44Q==";
        string PublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDNFcsoF8H5dHA9aNKMSjfDwYXhR+Lomlc/mkhbUPQxVGF+fNgqgY4QXhBIeQSL/YvkyICoSLlGTJg2vwgUL5zTuQC07nfZ5Tk+vHNwF4umsuseE0aYb2OW6ZyBj4xoVa6EjGmvNH7nZr3+kRHgvTvbXzTVf9azuSxruC9OpOMncwIDAQAB";


        public override Drop CreatePaymentDrop(CashOperation operatioin, HttpRequest request)
        {
            XiaoXiaoPayment data = new XiaoXiaoPayment();

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);


            var order = new
            {
                merchantID = this.MerchantId,
                waresname = "充值",
                cporderid = operatioin.Ref,
                price = double.Parse(operatioin.Amount.ToFormatStr()),
                returnurl = APIGlobal.CustNotifyUrl + "/xiaoxiaopay",
                notifyurl = APIGlobal.SrvNotifyUrl + "/xiaoxiaopay",
                ip = "0.0.0.0",
                paytype = 10014,
            };

            Hashtable req = new Hashtable();
            req.Add("merchantID", order.merchantID);
            req.Add("waresname", order.waresname);
            req.Add("cporderid", order.cporderid);
            req.Add("price", order.price);
            req.Add("returnurl", order.returnurl);
            req.Add("notifyurl", order.notifyurl);
            req.Add("ip", order.ip);
            req.Add("paytype", order.paytype);

            Hashtable tmp = new Hashtable();
            //添加一个Key和value
            tmp.Add("transdata", req.SerializeObject());
            tmp.Add("sign", RSAUtil2.Sign(XiaoXiaoHepler.createParams(req), this.PrivateKey, "UTF-8", "md5"));
            tmp.Add("signtype", "RSA");

            var response = XiaoXiaoHepler.sendPost(this.PayUrl, tmp);
            var respdata = response.DeserializeObject<OrderReturn>();
            logger.Info("response:" + response);

            if (respdata.ResultCode != "20000")
            {
                data.Error = "第三方网关异常:" + respdata.ResultCode;
            }
            else
            {
                data.PayLink = respdata.Info.PayUrl;
            }

            return data;
        }

        public static CashOperation GetCashOperation(NHttp.HttpRequest request)
        {

            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                string req = Encoding.UTF8.GetString(data);
                OrderNotify notify = req.DeserializeObject<OrderNotify>();

                return ORM.MCashOperation.SelectCashOperation(notify.Info.Pay_Order);
            }
            else
            {
                OrderNotify notify = request.RawContent.DeserializeObject<OrderNotify>();

                return ORM.MCashOperation.SelectCashOperation(notify.Info.Pay_Order);
            }

        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            string req = string.Empty;
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                req = Encoding.UTF8.GetString(data);
            }
            else
            {
                req = request.RawContent;
            }

            OrderNotify notify = req.DeserializeObject<OrderNotify>();

            Hashtable tmp2 = new Hashtable();
            tmp2.Add("pay_status", notify.Info.Pay_Status);
            tmp2.Add("pay_order", notify.Info.Pay_Order);
            tmp2.Add("transid", notify.Info.TransID);
            tmp2.Add("pay_fee", notify.Info.Pay_Fee);

            string sign = notify.Sign.Replace(" ", "+");
            string signSrc = XiaoXiaoHepler.createParams(tmp2);
            //验证签名
            if (RSAUtil2.Verify(signSrc, this.PublicKey, sign, "UTF-8", "md5"))
            {
                if (notify.ResultCode == "20000")
                {
                    return true;
                }
            }

            return false;
        }


        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            string req = string.Empty;
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                req = Encoding.UTF8.GetString(data);
            }
            else
            {
                req = request.RawContent;
            }

            OrderNotify notify = req.DeserializeObject<OrderNotify>();

            Hashtable tmp2 = new Hashtable();
            tmp2.Add("pay_status", notify.Info.Pay_Status);
            tmp2.Add("pay_order", notify.Info.Pay_Order);
            tmp2.Add("transid", notify.Info.TransID);
            tmp2.Add("pay_fee", notify.Info.Pay_Fee);

            string sign = notify.Sign.Replace(" ", "+");
            string signSrc = XiaoXiaoHepler.createParams(tmp2);
            //验证签名
            if (RSAUtil2.Verify(signSrc, this.PublicKey, sign, "UTF-8", "md5"))
            {
                if (notify.ResultCode == "20000")
                {
                    return true;
                }
            }

            return false;
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            string req = string.Empty;
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                req = Encoding.UTF8.GetString(data);
            }
            else
            {
                req = request.RawContent;
            }

            OrderNotify notify = req.DeserializeObject<OrderNotify>();
            return notify.Info.Pay_Status;
        }
    }

    class Info
    {
        public string PayUrl;
        public string NonceStr;
    }
    class OrderReturn
    { 
        public Info Info;

        public string ResultCode;

        public string Sign;


        public string SignType;

    }

    class NotifyInfo
    { 
        public string Pay_Status;

        public string Pay_Order;

        public string TransID;

        public string Pay_Fee;


    }
    class OrderNotify
    {
        public NotifyInfo Info;

        public string ResultCode;

        public string Sign;

        public string SignType;
    }
}
