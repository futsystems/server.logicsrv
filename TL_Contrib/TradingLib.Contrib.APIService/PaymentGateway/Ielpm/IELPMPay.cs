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
using ielpm_merchant_code_demo.com.ielpm.merchant.code.sdk;

namespace TradingLib.Contrib.APIService
{
    public class IELPMPayGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("IELPMPayGateWay");

        public IELPMPayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.IELPMPay;
            var data = config.Config.DeserializeObject();

            this.PublicKeyName = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "config", "cust", config.Domain_ID.ToString(), "SS.cer" });
            this.PrivateKeyName = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "config", "cust", config.Domain_ID.ToString(), "CS.pfx" });
            this.MerNo = data["MerNo"].ToString(); //"S20170526002962";
            this.CertPassword = data["CerPass"].ToString(); //"1546362";
            //this.PayUrl = "https://cashier.ielpm.com/paygate/v1/web/cashier";
            this.PayUrl = data["PayUrl"].ToString(); //data["CerPass"].ToString();

            CertUtil.CertPassword = this.CertPassword;
            CertUtil.PublicCertPath = this.PublicKeyName;
            CertUtil.SignCertPath = this.PrivateKeyName;

            this.SuccessReponse = "YYYYYY";

        }

        public string PayUrl { get; set; }
        public string MerNo { get; set; }
        public string CertPassword { get; set; }
        public string PublicKeyName { get; set; }

        public string PrivateKeyName { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropIelpmPayment data = new DropIelpmPayment();

            data.merchantNo = this.MerNo;
            data.version = "v1.1";
            data.channelNo = "03";
            data.tranSerialNum = operatioin.Ref;
            data.tranTime = operatioin.DateTime.ToString();
            data.currency = "CNY";
            data.amt = (operatioin.Amount * 100).ToFormatStr("{0:F0}");
            data.bizType = "01";
            data.goodsName = "充值卡";
            data.returnUrl =  APIGlobal.CustNotifyUrl + "/ielpmpay";
            data.notifyUrl = APIGlobal.SrvNotifyUrl + "/ielpmpay";
            data.buyerName = IELPMUtil.EncryptData(operatioin.Account, Encoding.UTF8);
            data.buyerId = operatioin.Account;
            data.goodsInfo = "充值卡";
            data.valid = "60";
            data.remark = "";
            data.contact = IELPMUtil.EncryptData(operatioin.Account, Encoding.UTF8);
            data.goodsNum = "1";
            data.YUL1 = string.Empty;
            data.referer = string.Empty;

            data.ip = "127.0.0.1";

            SortedDictionary<string, string> resData = new SortedDictionary<string, string>(new IELPMComparer());

            resData.Add("merchantNo", data.merchantNo);
            resData.Add("version", data.version);
            resData.Add("channelNo", data.channelNo);
            resData.Add("tranSerialNum", data.tranSerialNum);
            resData.Add("tranTime", data.tranTime);
            resData.Add("currency", data.currency);
            resData.Add("amount", data.amt);
            resData.Add("bizType", data.bizType);
            resData.Add("goodsName", data.goodsName);
            resData.Add("notifyUrl", data.notifyUrl);
            resData.Add("returnUrl", data.returnUrl);
            resData.Add("buyerName", data.buyerName);
            resData.Add("buyerId", data.buyerId);
            resData.Add("goodsInfo", data.goodsInfo);
            resData.Add("valid", data.valid);
            resData.Add("remark", data.remark);
            resData.Add("contact",data.contact);
            resData.Add("goodsNum", data.goodsNum);
            resData.Add("ip", data.ip);
            resData.Add("YUL1", data.YUL1);
            resData.Add("referer", data.referer);
            //resData.Add("", data.);
            //resData.Add("", data.);
            //resData.Add("", data.);
            //resData.Add("", data.);
            //resData.Add("", data.);

            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);
            data.PayUrl = this.PayUrl;

            string sign = IELPMUtil.Sign(resData, Encoding.UTF8);

            data.sign = sign;


            return data;
        }


        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            string transid = queryString["tranSerialNum"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {

            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["rtnCode"] == "0000";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            string ResultDesc = queryString["rtnMsg"];

            return ResultDesc;
        }

    }
}
