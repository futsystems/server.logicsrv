using Common.Logging;
using DotLiquid;
using System;
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


        }

        public string MerchantID;

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropGaoHuiTongPayment data = new DropGaoHuiTongPayment();

            data.serviceName = "ebankPayApi";
            data.version = "V1";
            data.platform = string.Empty;
            data.merchantId = this.MerchantID;

            data.signType = "RSA";
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
            data.bankCode = "";
            data.clientIp = string.Empty;
            data.payerId = string.Empty;

            return data;

        }


    }
}
