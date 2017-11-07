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

namespace TradingLib.Contrib.Payment.YeePay
{
    public class YeePayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("P101KAGateWay");

        public YeePayGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.YeePay;
            try
            {
                var data = config.Config.DeserializeObject();
                this.PayUrl = data["PayUrl"].ToString();
                this.MerID = data["MerID"].ToString();
                this.Key = data["Key"].ToString();
                this.Domain = data["Domain"].ToString();
                this.PayDirectUrl = this.PayDirectUrl.Replace(APIGlobal.LocalIPAddress, this.Domain);
            }
            catch (Exception ex)
            { 
            
            }
        }
        public string ReturnURL = "";
        public string AdviceURL = "";
        string PayUrl = "https://www.yeepay.com/app-merchant-proxy/node";
        string MerID = "10015656278";
        string Key = "dcam6m24zkwxtegsqum45dmj2manw24j6ktk8562td16uv5d4ix91ojsql8r";
        string Domain = string.Empty;
        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropYeePayPayment data = new DropYeePayPayment();

            data.p0_Cmd = "Buy";
            data.p1_MerId = MerID;
            data.p2_Order = operatioin.Ref;
            data.p3_Amt = operatioin.Amount.ToFormatStr();
            data.p4_Cur = "CNY";
            data.p5_Pid = "";
            data.p6_Pcat = "";
            data.p7_Pdesc = "";
            data.p8_Url = APIGlobal.CustNotifyUrl + "/yeepay";
            data.p9_SAF = "";
            data.pa_MP = "";
            data.pb_ServerNotifyUrl = APIGlobal.SrvNotifyUrl + "/yeepay";
            data.pd_FrpId = "";
            data.pr_NeedResponse = "1";



            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            //所有请求接口数据字段名
            string[] list = { "p0_Cmd", "p1_MerId", "p2_Order", "p3_Amt", "p4_Cur", "p5_Pid", "p6_Pcat", "p7_Pdesc", "p8_Url", "p9_SAF", "pa_MP", "pb_ServerNotifyUrl", "pd_FrpId", "pn_Unit", "pm_Period", "pr_NeedResponse", "pt_UserName", "pt_PostalCode", "pt_Address", "pt_TeleNo", "pt_Mobile", "pt_Email", "pt_LeaveMessage", "hmac" };

            //需要生成签名的字段
            string[] list_tohmac = { "p0_Cmd", "p1_MerId", "p2_Order", "p3_Amt", "p4_Cur", "p5_Pid", "p6_Pcat", "p7_Pdesc", "p8_Url", "p9_SAF", "pa_MP", "pb_ServerNotifyUrl","pd_FrpId", "pm_Period", "pn_Unit", "pr_NeedResponse", "pt_UserName", "pt_PostalCode", "pt_Address", "pt_TeleNo", "pt_Mobile", "pt_Email", "pt_LeaveMessage" };

            //需要进行转码的字段
            string[] list_changeType = { "p5_Pid" };

            //存储前台数据
            Dictionary<string, string> map_request = new Dictionary<string, string>();
            map_request["p0_Cmd"] = data.p0_Cmd;
            map_request["p1_MerId"] = data.p1_MerId;
            map_request["p2_Order"] = data.p2_Order;
            map_request["p3_Amt"] = data.p3_Amt;
            map_request["p4_Cur"] = data.p4_Cur;
            map_request["p5_Pid"] = data.p5_Pid;
            map_request["p6_Pcat"] = data.p6_Pcat;
            map_request["p7_Pdesc"] = data.p7_Pdesc;
            map_request["p8_Url"] = data.p8_Url;
            map_request["p9_SAF"] = data.p9_SAF;
            map_request["pa_MP"] = data.pa_MP;
            map_request["pb_ServerNotifyUrl"] = data.pb_ServerNotifyUrl;
            map_request["pd_FrpId"] = data.pd_FrpId;
            map_request["pm_Period"] = data.pm_Period;
            map_request["pn_Unit"] = data.pn_Unit;
            map_request["pr_NeedResponse"] = data.pr_NeedResponse;
            //"pt_PostalCode", "pt_Address", "pt_TeleNo", "pt_Mobile", "pt_Email", "pt_LeaveMessage", "
            map_request["pt_UserName"] = "";
            map_request["pt_PostalCode"] = "";
            map_request["pt_Address"] = "";
            map_request["pt_TeleNo"] = "";
            map_request["pt_Mobile"] = "";
            map_request["pt_Email"] = "";
            map_request["pt_LeaveMessage"] = "";
            


            //生成签名字符串
            string data_hmac = toCreateHmacData(map_request, list_tohmac);
            //log.Append("加密的字符串：" + data_hmac + "\n");


            //生成hmac签名
            string hmac = Digest.CreateHmac(data_hmac, Key);
            //log.Append("请求hmac：" + hmac + "\n");
            data.hmac = hmac;


            return data;
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["r6_Order"];
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

            string nhmac = Digest.CreateHmac(sbOld, Key);
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



        /// <summary>
        /// 将Dictionary对象转化成HMAC签名的字符串数据
        /// </summary>
        /// <param name="info">request请求的数据信息</param>
        /// <param name="param">参加签名的数组</param>
        /// <param name="valueSplit">数据间的分隔符</param>
        /// <returns>签名字符串</returns>
        public static string toCreateHmacData(Dictionary<string, string> info, string[] paramRequests, string valueSplit)
        {
            //返回结果
            string result = "";

            foreach (string param in paramRequests)
            {
                result += result.Equals("") ? (info[param]) : (info[param].Equals("") ? (info[param]) : valueSplit + info[param]);
            }
            return result;
        }

        /// <summary>
        /// 将Dictionary对象转化成HMAC签名的字符串数据
        /// </summary>
        /// <param name="info">request请求的数据信息</param>
        /// <param name="param">参加签名的数组</param>
        /// <returns>签名字符串</returns>
        public static string toCreateHmacData(Dictionary<string, string> info, string[] paramRequests)
        {
            //返回结果
            string result = "";

            foreach (string param in paramRequests)
            {
                result = result + info[param];
            }

            return result;
        }


    }
}
