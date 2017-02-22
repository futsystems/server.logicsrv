using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using DotLiquid;


namespace TradingLib.Contrib.APIService
{
    public class AliPayGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("AliPayGateWay");

        public AliPayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.BaoFu;

            this.Partner = "2088811810175275";
            this.Key = "ecztwi2fawwwgupwi9ycj17fhuhay2oh";
            this.InputCharset = "utf-8";
            this.SignType = "MD5";
            this.PaymentType = "1";
            this.SellerEmail = "cashier@deskhosted.com";

            this.Service = "create_direct_pay_by_user";
            this.PayUrl = "https://mapi.alipay.com/gateway.do?";

        }


        public string Partner { get; set; }

        public string SellerEmail { get; set; }
        public string Key { get; set; }

        public string InputCharset { get; set; }

        public string SignType { get; set; }

        public string Service { get; set; }
        
        
        public string PaymentType { get; set; }

        public string PayUrl { get; set; }


        public override Drop CreatePaymentDrop(CashOperation operation)
        {
            DropAliPayPayment data = new DropAliPayPayment();
            SortedDictionary<string, string> args = new SortedDictionary<string, string>();
            data.Partner = this.Partner;
            data.InputCharset = this.InputCharset;
            
            data.PaymentType = this.PaymentType;
            data.Service = this.Service;

            data.SellerEmail = this.SellerEmail;
            data.Subject ="标题";
            data.Body = "服务";
            data.ShowUrl = "www.163.com";

            data.OutTradeNo = operation.Ref;
            data.TotalFee = operation.Amount.ToFormatStr();

            data.ReturnUrl = APIGlobal.CustNotifyUrl + "/alipay";
            data.NotifyUrl = APIGlobal.SrvNotifyUrl + "/alipay";
            data.PayUrl = this.PayUrl;



            data.Account = operation.Account;
            data.Amount = string.Format("{0}[{1}]", operation.Amount.ToFormatStr(), operation.Amount.ToChineseStr());
            data.Ref = operation.Ref;
            data.Operation = Util.GetEnumDescription(operation.OperationType);


            args.Add("partner", data.Partner);
            args.Add("_input_charset", data.InputCharset);
            args.Add("service", data.Service);
            args.Add("payment_type", data.PaymentType);
            args.Add("notify_url", data.NotifyUrl);
            args.Add("return_url", data.ReturnUrl);
            args.Add("seller_email", data.SellerEmail);
            args.Add("out_trade_no", data.OutTradeNo);
            args.Add("subject", data.Subject);
            args.Add("total_fee", data.TotalFee);
            args.Add("body", data.Body);
            args.Add("show_url", data.ShowUrl);
            args.Add("paymethod", "directPay");

            var tmp = AliPay.Core.FilterPara(args);

            //args.Add("show_url", data.Partner);
            //args.Add("show_url", data.Partner);

            data.SignType = this.SignType;
            data.Sign = BuildRequestMysign(tmp, this.SignType, this.Key,this.InputCharset);
            

            return data;
        }

        public override string DemoResponse(CashOperation operation)
        {
            DropAliPayPayment data = new DropAliPayPayment();
            SortedDictionary<string, string> args = new SortedDictionary<string, string>();
            data.Partner = this.Partner;
            data.InputCharset = this.InputCharset;

            data.PaymentType = this.PaymentType;
            data.Service = this.Service;

            data.SellerEmail = this.SellerEmail;
            data.Subject = "标题";
            data.Body = "服务";
            data.ShowUrl = "www.163.com";

            data.OutTradeNo = operation.Ref;
            data.TotalFee = operation.Amount.ToFormatStr();

            data.ReturnUrl = APIGlobal.CustNotifyUrl + "/alipay";
            data.NotifyUrl = APIGlobal.SrvNotifyUrl + "/alipay";
            data.PayUrl = this.PayUrl;



            data.Account = operation.Account;
            data.Amount = string.Format("{0}[{1}]", operation.Amount.ToFormatStr(), operation.Amount.ToChineseStr());
            data.Ref = operation.Ref;
            data.Operation = Util.GetEnumDescription(operation.OperationType);


            args.Add("partner", data.Partner);
            args.Add("_input_charset", data.InputCharset);
            args.Add("service", data.Service);
            args.Add("payment_type", data.PaymentType);
            args.Add("notify_url", data.NotifyUrl);
            args.Add("return_url", data.ReturnUrl);
            args.Add("seller_email", data.SellerEmail);
            args.Add("out_trade_no", data.OutTradeNo);
            args.Add("subject", data.Subject);
            args.Add("total_fee", data.TotalFee);
            args.Add("body", data.Body);
            args.Add("show_url", data.ShowUrl);

            var tmp = AliPay.Core.FilterPara(args);

            //args.Add("show_url", data.Partner);
            //args.Add("show_url", data.Partner);

            data.SignType = this.SignType;
            data.Sign = BuildRequestMysign(tmp, this.SignType, this.Key, this.InputCharset);
            args.Add("sign", data.Sign);
            args.Add("sign_type", data.SignType);

            return BuildRequest(args, "get", "demo");
        }

        public string BuildRequest(SortedDictionary<string, string> sParaTemp, string strMethod, string strButtonValue)
        {
            //待请求参数数组
            Dictionary<string, string> dicPara = new Dictionary<string, string>();
            

            StringBuilder sbHtml = new StringBuilder();

            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + this.PayUrl + "_input_charset=" + this.InputCharset + "' method='" + strMethod.ToLower().Trim() + "'>");

            foreach (KeyValuePair<string, string> temp in sParaTemp)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='" + strButtonValue + "' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

            return sbHtml.ToString();
        }



        private static string BuildRequestMysign(Dictionary<string, string> sPara,string signType,string key,string charset)
        {
            //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            string prestr = AliPay.Core.CreateLinkString(sPara);

            //把最终的字符串签名，获得签名结果
            string mysign = "";
            switch (signType)
            {
                case "MD5":
                    mysign = AliPay.AlipayMD5.Sign(prestr, key, charset);
                    break;
                default:
                    mysign = "";
                    break;
            }

            return mysign;
        }

    }
}
