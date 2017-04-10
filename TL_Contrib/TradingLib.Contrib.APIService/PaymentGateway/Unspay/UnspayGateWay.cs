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
    public class UnspayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("UnspayGateWay");

        public UnspayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.UnsPay;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString(); //"www.unspay.com/unspay/page/linkbank/payRequest.do";
            this.MerchantID = data["MerID"].ToString();
            this.MerchantKey = data["MerKey"].ToString();
            this.SuccessReponse = "true";
        }


        string MerchantID { get; set; }
        string MerchantKey { get; set; }
        public string PayUrl { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operation)
        {
            DropUnspayPayment data = new DropUnspayPayment();

            data.MerchantID = this.MerchantID;
            data.MerchantUrl = APIGlobal.SrvNotifyUrl + "/unspay";
            data.ResponseMode = "3";
            data.OrderID = operation.Ref;
            data.CurrencyType = "CNY";
            data.OrderAmount = operation.Amount.ToFormatStr();
            data.AssuredPay = "false";
            data.Time = Util.ToTLDateTime().ToString();
            data.Remark = "";
            
            data.MerchantKey = this.MerchantKey;

            string tmp = string.Format("merchantId={0}&merchantUrl={1}&responseMode={2}&orderId={3}&currencyType={4}&amount={5}&assuredPay={6}&time={7}&remark={8}&merchantKey={9}",
                data.MerchantID,
                data.MerchantUrl,
                data.ResponseMode,
                data.OrderID,
                data.CurrencyType,
                data.OrderAmount,
                data.AssuredPay,
                data.Time,
                data.Remark,
                data.MerchantKey);

            data.MAC = Md5Encrypt(tmp);

            data.Account = operation.Account;
            data.Amount = string.Format("{0}[{1}]", operation.Amount.ToFormatStr(), operation.Amount.ToChineseStr());
            data.Ref = operation.Ref;
            data.Operation = Util.GetEnumDescription(operation.OperationType);
            data.PayUrl = this.PayUrl;

            return data;
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            string orderid = queryString["orderId"];
            return ORM.MCashOperation.SelectCashOperation(orderid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            var merchandID = queryString["merchantId"];
            var responseMode = queryString["responseMode"];
            var orderId = queryString["orderId"];
            var currencyType = queryString["currencyType"];
            var amount = queryString["amount"];
            var returnCode = queryString["returnCode"];
            var returnMessage = queryString["returnMessage"];
            var Md5Sign = queryString["mac"];

            string tmp = string.Format("merchantId={0}&responseMode={1}&orderId={2}&currencyType={3}&amount={4}&returnCode={5}&returnMessage={6}&merchantKey={7}",
                merchandID,
                responseMode,
                orderId,
                currencyType,
                amount,
                returnCode,
                returnMessage,
                this.MerchantKey);
            if (Md5Sign.ToLower() == Md5Encrypt(tmp).ToLower())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            string result = queryString["returnCode"];
            return result == "0000";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            string returnMessage = queryString["returnMessage"];

            return returnMessage;
        }


        //将字符串经过md5加密，返回加密后的字符串的小写表示
        public static string Md5Encrypt(string strToBeEncrypt)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] FromData = System.Text.Encoding.GetEncoding("utf-8").GetBytes(strToBeEncrypt);
            Byte[] TargetData = md5.ComputeHash(FromData);
            string Byte2String = "";
            for (int i = 0; i < TargetData.Length; i++)
            {
                Byte2String += TargetData[i].ToString("x2");
            }
            return Byte2String.ToLower();
        }


    
    }
}
