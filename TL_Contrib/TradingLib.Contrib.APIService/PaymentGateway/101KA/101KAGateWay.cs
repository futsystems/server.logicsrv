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

namespace TradingLib.Contrib.Payment.P101KA
{
    public class P101KAGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("P101KAGateWay");

        public P101KAGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.P101KA;
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
            Drop101KAPayment data = new Drop101KAPayment();

            data.p0_Cmd = "Buy";
            data.p1_MerId = MerID;
            data.p2_Order = operatioin.Ref;
            data.p3_Amt = operatioin.Amount.ToFormatStr();
            data.p4_Cur = "CNY";
            data.p5_Pid = "";
            data.p6_Pcat = "";
            data.p7_Pdesc = "";
            data.p8_Url = APIGlobal.SrvNotifyUrl + "/p101ka";
            data.p9_SAF = "";
            data.pa_MP = "";
            data.pd_FrpId = "";
            data.pr_NeedResponse = "1";



            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            string sbOld = "";
            sbOld += data.p0_Cmd;
            sbOld += data.p1_MerId;
            sbOld += data.p2_Order;
            sbOld += data.p3_Amt;
            sbOld += data.p4_Cur;

            sbOld += data.p5_Pid;
            sbOld += data.p6_Pcat;
            sbOld += data.p7_Pdesc;
            sbOld += data.p8_Url;
            sbOld += data.p9_SAF;

            sbOld += data.pa_MP;
            sbOld += data.pd_FrpId;
            sbOld += data.pr_NeedResponse;

            data.hmac = P101KAHelper.HmacSign(sbOld, Key);


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
            var Request = request.Params;

            string p1_MerId = MerID;
            string r0_Cmd = Request["r0_Cmd"];
            string r1_Code = Request["r1_Code"];
            string r2_TrxId = Request["r2_TrxId"];
            string r3_Amt = Request["r3_Amt"];
            string r4_Cur = Request["r4_Cur"];
            string r5_Pid = Request["r5_Pid"];
            string r6_Order = Request["r6_Order"];
            string r7_Uid = Request["r7_Uid"];
            string r8_MP = Request["r8_MP"];
            string r9_BType = Request["r9_BType"];
            string rp_PayDate = Request["rp_PayDate"];
            string hmac = Request["hmac"];

            string sbOld = "";
            sbOld += p1_MerId;
            sbOld += r0_Cmd;
            sbOld += r1_Code;
            sbOld += r2_TrxId;
            sbOld += r3_Amt;
            sbOld += r4_Cur;
            sbOld += r5_Pid;
            sbOld += r6_Order;
            sbOld += r7_Uid;
            sbOld += r8_MP;
            sbOld += r9_BType;

            string nhmac = P101KAHelper.HmacSign(sbOld, Key);
            return (nhmac == hmac);
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["r1_Code"] == "1";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["r1_Code"] == "1" ? "支付成功" : "支付失败";
        }

    }
}
