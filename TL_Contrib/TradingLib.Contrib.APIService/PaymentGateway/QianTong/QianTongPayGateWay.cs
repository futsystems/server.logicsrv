using System;
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

namespace TradingLib.Contrib.Payment.QianTong
{
    public class QianTongGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("QianTongGateWay");

        public QianTongGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.QianTong;
            var data = config.Config.DeserializeObject();
            this.CerPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "config", "cust", config.Domain_ID.ToString(), "server_cert.cer" });
            this.PfxPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "config", "cust", config.Domain_ID.ToString(), "merchant_cert.pfx" });

            //this.PFXPass =   "Aa996633";
            //this.PayUrl ="http://www.qtongpay.com/pay/pay.htm"; // "https://123.56.119.177:8443/pay/pay.htm";
            //this.MerchantId = "1010199";

            this.PayUrl = data["PayUrl"].ToString();
            this.MerchantId = data["MerID"].ToString();
            this.PFXPass = data["Pass"].ToString();
        }

        string PayUrl { get; set; }
        string MerchantId { get; set; }
        string PFXPass { get; set; }

        string PfxPath { get; set; }
        string CerPath { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operatioin, HttpRequest request)
        {
            DropQianTongPayment data = new DropQianTongPayment();



            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            var custnotify = APIGlobal.CustNotifyUrl + "/qiantong";
            var notifyurl  = APIGlobal.SrvNotifyUrl + "/qiantong";

            string xml = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?><message accountType=\"0\" application=\"SubmitOrder\" bankId=\"\" bizType=\"\" "
                    + "credentialNo=\"\" credentialType=\"\" guaranteeAmt=\"0\" merchantFrontEndUrl=\"{0}\" "
                    + "merchantId=\"{1}\" merchantOrderAmt=\"{2}\" merchantOrderDesc=\"\" merchantOrderId=\"{3}\" "
                    + "merchantPayNotifyUrl=\"{4}\" msgExt=\"\" orderTime=\"{5}\" payMode=\"0\" "
                    + "payerId=\"\" rptType=\"1\" salerId=\"\" userMobileNo=\"\" userName=\"\" userType=\"1\" version=\"1.0.1\"/>",custnotify,MerchantId,(operatioin.Amount * 100).ToFormatStr("{0:F0}"),operatioin.Ref,notifyurl,operatioin.DateTime.ToString());

            X509Certificate2 pfx = new X509Certificate2(PfxPath, this.PFXPass, X509KeyStorageFlags.Exportable);//取得pfx证书
            string privateKey = pfx.PrivateKey.ToXmlString(true);  //商户私钥
            data.ReqData =  Convert.ToBase64String(Encoding.UTF8.GetBytes(xml)) + "|" + RSAUtil.sign(privateKey, xml);

            return data;

        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            throw new NotImplementedException();
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            X509Certificate2 cer = new X509Certificate2(this.CerPath);
            string publicKey = cer.PublicKey.Key.ToXmlString(false);//服务端公钥
            string[] es = request.RawContent.Split(new char[] { '|' });
            //验签
            if (es.Length == 2 && RSAUtil.veryfy(publicKey, es[0], es[1]))
            {
                return true;
            }
            return false;
        }


        public string ParseTransID(NHttp.HttpRequest request)
        {
            X509Certificate2 cer = new X509Certificate2(this.CerPath);
            string publicKey = cer.PublicKey.Key.ToXmlString(false);//服务端公钥
            string[] es = request.RawContent.Split(new char[] { '|' });
            //验签
            if (es.Length == 2 && RSAUtil.veryfy(publicKey, es[0], es[1]))
            {
                string ret = Encoding.UTF8.GetString(Convert.FromBase64String(es[0]));
                //ret = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>\r\n<message application=\"NotifyOrder\" merchantId=\"1008038\" merchantOrderId=\"636383156318160977\" version=\"1.0.1\">\r\n<deductList>\r\n<item payAmt=\"20\" payDesc=\"付款成功\" payOrderId=\"J5ZUH3U53JLXPWQXE\" payStatus=\"01\" payTime=\"20170814160107\"/>\r\n</deductList>\r\n<refundList/>\r\n</message>\r\n";
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(ret);
                return doc["message"].GetAttribute("merchantOrderId");

            }
            return string.Empty;
        }
        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            X509Certificate2 cer = new X509Certificate2(this.CerPath);
            string publicKey = cer.PublicKey.Key.ToXmlString(false);//服务端公钥
            string[] es = request.RawContent.Split(new char[] { '|' });
            if (es.Length == 2 && RSAUtil.veryfy(publicKey, es[0], es[1]))
            {
                string ret = Encoding.UTF8.GetString(Convert.FromBase64String(es[0]));
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(ret);
                return doc["message"]["deductList"]["item"].GetAttribute("payStatus") == "01";
            }
            return false;
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            X509Certificate2 cer = new X509Certificate2(this.CerPath);
            string publicKey = cer.PublicKey.Key.ToXmlString(false);//服务端公钥
            string[] es = request.RawContent.Split(new char[] { '|' });
            if (es.Length == 2 && RSAUtil.veryfy(publicKey, es[0], es[1]))
            {
                string ret = Encoding.UTF8.GetString(Convert.FromBase64String(es[0]));
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(ret);
                return doc["message"]["deductList"]["item"].GetAttribute("payDesc");
            }
            return string.Empty;
        }

    }
}
