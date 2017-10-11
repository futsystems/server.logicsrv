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

namespace TradingLib.Contrib.Payment.Pay848
{
    public class Pay848GateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("Pay848GateWay");

        public Pay848GateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.Pay848;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.MerID = data["MerID"].ToString();
            this.MD5Key = data["Key"].ToString(); 
        }

        string MerID = "1308";
        string PayUrl = "http://pay.848pay.com/chargebank.aspx";
        string MD5Key = "7feb790d332f42769d3f34147587be0b";

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropPay848Payment data = new DropPay848Payment();

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            data.parter = this.MerID;
            data.type = ConvBankCode(operatioin.Bank);
            data.value = operatioin.Amount.ToFormatStr();
            data.orderid = operatioin.Ref;

            data.callbackurl = APIGlobal.CustNotifyUrl + "/pay848";
            data.hrefbackurl = APIGlobal.SrvNotifyUrl + "/pay848";
            data.payerIp = string.Empty;
            data.attach = string.Empty;

            string rawStr = string.Format("parter={0}&type={1}&value={2}&orderid={3}&callbackurl={4}", data.parter, data.type, data.value, data.orderid, data.callbackurl);

            data.sign = Sign(rawStr, this.MD5Key);

            return data;
        }


        string ConvBankCode(string stdCode)
        {
            switch (stdCode)
            {
                //工商银行
                case "01020000": return "967";
                //农业银行
                case "01030000": return "964";
                //建设银行
                case "01050000": return "965";
                //中国银行
                case "01040000": return "963";
                //招商
                case "03080000": return "970";
                //交通
                case "03010000": return "981";
                //邮政
                case "01000000": return "971";
                //支付宝
                case "AliPay": return "101";
                case "WeiXin": return "1004";
                default:
                    return "967";

            }
        }



        /// <summary>
        /// 签名字符串
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>签名结果</returns>
        public static string Sign(string prestr, string key)
        {
            StringBuilder sb = new StringBuilder(32);

            prestr = prestr + key;

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding("GB2312").GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["orderid"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            var Request = request.Params;

            string orderid = Request["orderid"];
            string opstate = Request["opstate"];
            string ovalue = Request["ovalue"];
            string sign = Request["sign"];
            string sysorderid = Request["sysorderid"];
            string completiontime = Request["completiontime"];
            string attach = Request["attach"];
            string msg = Request["msg"];




            string rawStr = string.Format("orderid={0}&opstate={1}&ovalue={2}", orderid, opstate, ovalue);


            return Sign(rawStr,this.MD5Key) == sign;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["opstate"] == "0";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return queryString["opstate"] == "0" ? "支付成功" : "支付失败";
        }


    }
}
