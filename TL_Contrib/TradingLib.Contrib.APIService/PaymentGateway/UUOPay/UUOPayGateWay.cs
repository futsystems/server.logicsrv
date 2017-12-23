using Common.Logging;
using DotLiquid;
using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;
using NHttp;
namespace TradingLib.Contrib.Payment.UUOPay
{
    public class UUOPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("UUOPayGateWay");

        public UUOPayGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.UUOPay;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.MerID = data["MerID"].ToString();
            this.Key = data["Key"].ToString();
            this.MerAcc = data["MerACC"].ToString();
        }

        string MerID = "100461";
        string MerAcc = "1041726176@qq.com";
        string Key = "48B6DB3B9431892506532C9FD9B58961";
        string PayUrl = "http://pay.uuopay.com/pay.php";

        public override Drop CreatePaymentDrop(CashOperation operatioin, HttpRequest request)
        {
            DropUUOPayPayment data = new DropUUOPayPayment();

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);
            data.Account = operatioin.Account;

            data.apiName = "WWW_PAY";
            data.apiVersion = "V.1.0.0";
            data.platformID = MerID;
            data.merchNo = MerAcc;
            data.orderNo = operatioin.Ref;//operatioin.Ref.Substring(operatioin.Ref.Length - 6 - 1, 6);
            data.tradeDate = Util.ToDateTime(operatioin.DateTime).ToTLDate().ToString();
            data.amt = operatioin.Amount.ToFormatStr();
            data.returnUrl = APIGlobal.CustNotifyUrl + "/uuopay";
            data.notifyUrl = APIGlobal.SrvNotifyUrl + "/uuopay";

            data.merchParam = string.Empty;
            data.tradeSummary = string.Empty;
            data.signMsg = "";
            data.bankCode = ConvBankCode(operatioin.Bank);
            data.choosePayType = "YOUBAO";

            string rawStr = string.Format("apiName={0}&apiVersion={1}&platformID={2}&merchNo={3}&orderNo={4}&tradeDate={5}&amt={6}&notifyUrl={7}&returnUrl={8}&merchParam={9}",
                data.apiName,
                data.apiVersion,
                data.platformID,
                data.merchNo,
                data.orderNo,
                data.tradeDate,
                data.amt,
                data.notifyUrl,
                data.returnUrl,
                data.merchParam);

            data.signMsg = MD5Sign(rawStr, this.Key);

            return data;

        }
        string ConvBankCode(string stdCode)
        {
            switch (stdCode)
            {
                //工商银行
                case "01020000": return "ICBC";
                //农业银行
                case "01030000": return "ABC";
                //建设银行
                case "01050000": return "CCB";
                //中国银行
                case "01040000": return "BOC";
                //招商
                case "03080000": return "CMB";
                //交通
                case "03010000": return "BOCOM";
                //邮政
                case "01000000": return "PSBC";
                //中信银行
                case "03020000": return "CITIC";
                //光大
                case "03030000": return "CEB";
                //民生
                case "03050000": return "CMBC";
                default:
                    return "ICBC";

            }
        }
        public static CashOperation GetCashOperation(NHttp.HttpRequest request)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = string.Empty;
            if (request.RequestType.ToUpper() == "POST")
            {
                transid = request.Params["orderNo"].ToString();
            }
            else
            {
                transid = request.QueryString["orderNo"];
            }
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var Request = request.Params;


            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            string statusCode = string.Empty;
            if (request.RequestType.ToUpper() == "POST")
            {
                statusCode = request.Params["orderStatus"].ToString();
            }
            else
            {
                statusCode = request.QueryString["orderStatus"];
            }

            return statusCode == "1";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            string statusCode = string.Empty;
            if (request.RequestType.ToUpper() == "POST")
            {
                statusCode = request.Params["orderStatus"].ToString();
            }
            else
            {
                statusCode = request.QueryString["orderStatus"];
            }
            return statusCode == "1" ? "支付成功" : "支付失败";
        }

        public string MD5Sign(string strToBeEncrypt, string md5Key)
        {
            string strSrc = strToBeEncrypt + md5Key;
            logger.Info("signStr:" + strSrc);
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] FromData = System.Text.Encoding.GetEncoding("utf-8").GetBytes(strSrc);
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
