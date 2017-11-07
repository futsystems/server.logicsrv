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

namespace TradingLib.Contrib.Payment.RMTech
{
    public class RMTechGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("RMTechGateWay");

        public RMTechGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.RMTech;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.MerID = data["MerID"].ToString();
            this.Key = data["Key"].ToString();
        }

        string PayUrl = "http://api.101ka.com/GateWay/Bank/Default.aspx";
        string MerID = "8886781";
        string Key = "074ab6e0020449b0b07e2de7c884750c";

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropRMTechPayment data = new DropRMTechPayment();

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            data.tradeType = "cs.pay.submit";
            data.version = "2.0";
            data.channel = "";
            data.mchNo = this.MerID;
            data.sign = "";
            data.body = "充值";
            data.mchOrderNo = operatioin.Ref;
            data.oamount = (double)operatioin.Amount;
            data.decAmount = 0;
            data.description = "";
            data.currency = "CNY";
            data.timePaid = operatioin.DateTime.ToString();
            data.timeExpire = "";
            data.timeSettle = "";
            data.subject = "充值";
            data.extra = "";
            data.innerOrderNo = operatioin.Ref;
            data.virAccNo = "0";
            data.oamount2 = (double)operatioin.Amount;
            data.freezeDays = 0;

            var detail = new
            {
                innerOrderNo = operatioin.Ref,
                virAccNo = "0",
                amount = (double)operatioin.Amount,
            };

            var extra = new 
            {
                bankPayType = "01",
                cardType = "01",
                bankCode="",
                notifyUrl=APIGlobal.SrvNotifyUrl + "/rmtech",
                callbackUrl = APIGlobal.CustNotifyUrl + "/rmtech",
            };

            var obj = new {
                amount = (double)operatioin.Amount,
                body = "充值",
                channel = "gateway",
                amountSettle = string.Empty,
                timeExpire = string.Empty,
                timeSettle = string.Empty,
                subject = string.Empty,
                currency ="CNY",
                description = "充值",
                mchNo = this.MerID,
                mchOrderNo = operatioin.Ref,
                timePaid = "",
                tradeType = "cs.pay.submit",
                version = "2.0",
                details = new []{detail},
                extra = extra,
                sign="",

            };

            //string rawStr = string.Format("amount={0}&amountSettle=&bankPayType=01&body={1}&channel={2}&currency=CNY&description={3}&details=[{amount={4}&innerOrderNo={5}&virAccNo=0}]&mchNo={6}&notifyUrl=http://xxxxx/xx&openId=12345678&mchOrderNo=201609090001&subject=&timeExpire=&timePaid=&timeSettle=&tradeType=cs.pay.submit&version=2.0 ")
            return data;
        }
    }
}
