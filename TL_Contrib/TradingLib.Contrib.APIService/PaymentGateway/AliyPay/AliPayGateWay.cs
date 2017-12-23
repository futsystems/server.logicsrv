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
using NHttp;

namespace TradingLib.Contrib.APIService
{
    public class AliPayGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("AliPayGateWay");

        public AliPayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.AliPay;

            var data = config.Config.DeserializeObject();
            try
            {
                this.Partner = data["Partner"].ToString();// "2088811810175275";
                this.Key = data["Key"].ToString(); //"ecztwi2fawwwgupwi9ycj17fhuhay2oh";
                this.PayUrl = data["PayUrl"].ToString();// "https://mapi.alipay.com/gateway.do";
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("GateWay:{0} creaet error", config.ID));
            }

            //以下系统默认
            this.InputCharset = "utf-8";
            this.SignType = "MD5";
            this.PaymentType = "1";
            this.Service = "create_direct_pay_by_user";
            
            this.VerifyUrl = "https://mapi.alipay.com/gateway.do?service=notify_verify&";
        }

        public string VerifyUrl { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public string Partner { get; set; }
        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string InputCharset { get; set; }
        /// <summary>
        /// Sing类型
        /// </summary>
        public string SignType { get; set; }
        /// <summary>
        /// 服务
        /// </summary>
        public string Service { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PaymentType { get; set; }
        /// <summary>
        /// 支付URL
        /// </summary>
        public string PayUrl { get; set; }


        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["out_trade_no"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override Drop CreatePaymentDrop(CashOperation operation, HttpRequest request)
        {
            DropAliPayPayment data = new DropAliPayPayment();
            SortedDictionary<string, string> args = new SortedDictionary<string, string>();
            data.PayUrl = string.Format("{0}?_input_charset={1}", this.PayUrl, this.InputCharset);
            data.Partner = this.Partner;
            data.InputCharset = this.InputCharset;
            data.PaymentType = this.PaymentType;
            data.Service = this.Service;
            data.SellerID = this.Partner;

            data.Subject ="软件SAAS服务充值";
            data.OutTradeNo = operation.Ref;
            data.TotalFee = operation.Amount.ToFormatStr();

            data.ReturnUrl = APIGlobal.CustNotifyUrl + "/alipay";
            data.NotifyUrl = APIGlobal.SrvNotifyUrl + "/alipay";
            
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
            args.Add("out_trade_no", data.OutTradeNo);
            args.Add("subject", data.Subject);
            args.Add("total_fee", data.TotalFee);
            args.Add("seller_id", data.SellerID);

            //将空值以及sign signtype 过滤掉
            var tmp = AliPay.Core.FilterPara(args);

            data.SignType = this.SignType;
            //获得sign
            data.Sign = GetSign(tmp, this.SignType, this.Key, this.InputCharset);

            return data;
        }


        //public string BuildRequest(SortedDictionary<string, string> sParaTemp, string strMethod, string strButtonValue)
        //{

        //    StringBuilder sbHtml = new StringBuilder();

        //    sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + this.PayUrl + "_input_charset=" + this.InputCharset + "' method='" + strMethod.ToLower().Trim() + "'>");

        //    foreach (KeyValuePair<string, string> temp in sParaTemp)
        //    {
        //        sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
        //    }

        //    //submit按钮控件请不要含有name属性
        //    sbHtml.Append("<input type='submit' value='" + strButtonValue + "' style='display:none;'></form>");

        //    sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

        //    return sbHtml.ToString();
        //}



        private static string GetSign(Dictionary<string, string> sPara, string signType, string key, string charset)
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

        /// <summary>
        /// 检查访问请求是否有效
        /// 包含服务端通知与客户端页面跳转
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            //支付宝客户端页面跳转使用get 服务端通知使用post 因此获取参数的方式不一样
            SortedDictionary<string, string> args = new SortedDictionary<string, string>();
            string notify_id = string.Empty;
            string sign = string.Empty;
            if (request.RequestType.ToUpper() == "POST")
            {
                foreach (var key in request.Form.AllKeys)
                {
                    args.Add(key, request.Form[key]);
                }
                notify_id = request.Form["notify_id"];
                sign = request.Form["sign"];
            }
            else
            {
                foreach (var key in request.QueryString.AllKeys)
                {
                    args.Add(key, request.QueryString[key]);
                }

                notify_id = request.QueryString["notify_id"];
                sign = request.QueryString["sign"];
            }

            return Verify(args, notify_id, sign);

        }

        /// <summary>
        /// 支付宝服务端通知与客户端页面跳转传递的参数不一样因此需要根据访问方法进行分别处理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            if (request.RequestType.ToUpper() == "POST")
            {
                string status = request.Form["trade_status"];
                if (status == "TRADE_SUCCESS" || status == "TRADE_FINISHED")
                { 
                    //金额判定
                    if (operation.Amount == decimal.Parse(request.Form["total_fee"]))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                string ret = request.QueryString["is_success"];
                return ret == "T";
            }
            

        }

        /// <summary>
        ///  验证消息是否是支付宝发出的合法消息
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="notify_id">通知验证ID</param>
        /// <param name="sign">支付宝生成的签名结果</param>
        /// <returns>验证结果</returns>
        public bool Verify(SortedDictionary<string, string> inputPara, string notify_id, string sign)
        {
            //获取返回时的签名验证结果
            bool isSign = GetSignVeryfy(inputPara, sign);
            //获取是否是支付宝服务器发来的请求的验证结果
            string responseTxt = "false";
            if (notify_id != null && notify_id != "") { responseTxt = GetResponseTxt(notify_id); }

            //写日志记录（若要调试，请取消下面两行注释）
            //string sWord = "responseTxt=" + responseTxt + "\n isSign=" + isSign.ToString() + "\n 返回回来的参数：" + GetPreSignStr(inputPara) + "\n ";
            //Core.LogResult(sWord);

            //判断responsetTxt是否为true，isSign是否为true
            //responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
            //isSign不是true，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关
            if (responseTxt == "true" && isSign)//验证成功
            {
                return true;
            }
            else//验证失败
            {
                return false;
            }
        }

        /// <summary>
        /// 获取是否是支付宝服务器发来的请求的验证结果
        /// </summary>
        /// <param name="notify_id">通知验证ID</param>
        /// <returns>验证结果</returns>
        private string GetResponseTxt(string notify_id)
        {
            string veryfy_url = this.VerifyUrl + "partner=" + this.Partner + "&notify_id=" + notify_id;
            //获取远程服务器ATN结果，验证是否是支付宝服务器发来的请求
            return Get_Http(veryfy_url, 120000);
        }

        private string Get_Http(string strUrl, int timeout)
        {
            string strResult;
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                myReq.Timeout = timeout;
                HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }

                strResult = strBuilder.ToString();
            }
            catch (Exception exp)
            {
                strResult = "错误：" + exp.Message;
            }

            return strResult;
        }

        /// <summary>
        /// 获取返回时的签名验证结果
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="sign">对比的签名结果</param>
        /// <returns>签名验证结果</returns>
        private bool GetSignVeryfy(SortedDictionary<string, string> inputPara, string sign)
        {
            Dictionary<string, string> sPara = new Dictionary<string, string>();

            //过滤空值、sign与sign_type参数
            sPara = AliPay.Core.FilterPara(inputPara);

            //获取待签名字符串
            string preSignStr = AliPay.Core.CreateLinkString(sPara);

            //获得签名验证结果
            bool isSgin = false;
            if (sign != null && sign != "")
            {
                switch (this.SignType)
                {
                    case "MD5":
                        isSgin = AliPay.AlipayMD5.Verify(preSignStr, sign, this.Key, this.InputCharset);
                        break;
                    default:
                        break;
                }
            }

            return isSgin;
        }

    }
}
