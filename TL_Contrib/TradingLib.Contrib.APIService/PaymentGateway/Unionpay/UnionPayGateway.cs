using Common.Logging;
using DotLiquid;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;
using com.unionpay.acp.sdk;

namespace TradingLib.Contrib.Payment.UnionPay
{
    public class UnionPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("UnionPayGateWay");

        public UnionPayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.UnionPay;
            var data = config.Config.DeserializeObject();



            this.PayUrl = data["PayUrl"].ToString();
            this.MerID = data["MerID"].ToString();
            this.Pass = data["Pass"].ToString();

            SDKConfig.IfValidateCNName = "false";
            SDKConfig.ValidateCertDir = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "config", "cust", config.Domain_ID.ToString()});
            SDKConfig.RootCertPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "config", "cust", config.Domain_ID.ToString() });
            SDKConfig.EncryptCert = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "config", "cust", config.Domain_ID.ToString(), "acp_prod_enc.cer" });
            SDKConfig.MiddleCertPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "config", "cust", config.Domain_ID.ToString(), "acp_prod_middle.cer" });
            SDKConfig.RootCertPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "config", "cust", config.Domain_ID.ToString(), "acp_prod_root.cer" });
            SDKConfig.SignCertPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "config", "cust", config.Domain_ID.ToString(), "acp_prod_sign.pfx" });
            SDKConfig.SignCertPwd = this.Pass;// "000000";
          
        }

        string PayUrl = "https://gateway.95516.com/gateway/api/frontTransReq.do";
        string MerID = "827320550940043";
        string Pass = "149876";//string.Empty;

 


        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropUnionPayment data = new DropUnionPayment();

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);


            data.version = "5.1.0";
            data.encoding = "UTF-8";
            data.txnType = "01";
            data.txnSubType = "01";
            data.bizType = "000201";
            data.signMethod = "01";
            data.channelType = "08";
            data.accessType = "0";

            data.frontUrl = APIGlobal.CustNotifyUrl + "/unionpay";
            data.backUrl = APIGlobal.SrvNotifyUrl + "/unionpay";
            data.currencyCode = "156";
            data.payTimeout = (Util.ToDateTime(operatioin.DateTime) + TimeSpan.FromHours(1)).ToTLDateTime().ToString();

            data.merId = this.MerID;
            data.orderId = operatioin.Ref;
            data.txnTime = operatioin.DateTime.ToString();
            data.txnAmt = (operatioin.Amount * 100).ToFormatStr("{0:F0}");
            data.payTimeout = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");

            Dictionary<string, string> param = new Dictionary<string, string>();
            param["version"] = data.version;//版本号
            param["encoding"] = data.encoding;//编码方式
            param["txnType"] = data.txnType;//交易类型
            param["txnSubType"] = data.txnSubType;//交易子类
            param["bizType"] = data.bizType;//业务类型
            param["signMethod"] = data.signMethod;//签名方法
            param["channelType"] = data.channelType;//渠道类型
            param["accessType"] = data.accessType;//接入类型
            param["frontUrl"] = data.frontUrl;  //前台通知地址      
            param["backUrl"] = data.backUrl;  //后台通知地址
            param["currencyCode"] = data.currencyCode;//交易币种

            // 订单超时时间。
            // 超过此时间后，除网银交易外，其他交易银联系统会拒绝受理，提示超时。 跳转银行网银交易如果超时后交易成功，会自动退款，大约5个工作日金额返还到持卡人账户。
            // 此时间建议取支付时的北京时间加15分钟。
            // 超过超时时间调查询接口应答origRespCode不是A6或者00的就可以判断为失败。
            param["payTimeout"] = data.payTimeout;


            //TODO 以下信息需要填写
            param["merId"] = data.merId;//商户号，请改自己的测试商户号，此处默认取demo演示页面传递的参数
            param["orderId"] = data.orderId;//商户订单号，8-32位数字字母，不能含“-”或“_”，此处默认取demo演示页面传递的参数，可以自行定制规则
            param["txnTime"] = data.txnTime;//订单发送时间，格式为YYYYMMDDhhmmss，取北京时间，此处默认取demo演示页面传递的参数，参考取法： DateTime.Now.ToString("yyyyMMddHHmmss")
            param["txnAmt"] = data.txnAmt;//交易金额，单位分，此处默认取demo演示页面传递的参数

            AcpService.Sign(param, Encoding.UTF8);
            //var dd = AcpService.CreateAutoFormHtml(SDKConfig.FrontTransUrl, param, System.Text.Encoding.UTF8);// 将SDKUtil产生的Html文档写入页面，从而引导用户浏览器重定向   

            data.certId = param["certId"];
            data.signature = param["signature"];
            return data;
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["orderId"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {

            if (request.RequestType.ToUpper() == "POST")
            {
                Dictionary<string, string> resData = new Dictionary<string, string>();
                var coll = request.Form;

                string[] requestItem = coll.AllKeys;

                for (int i = 0; i < requestItem.Length; i++)
                {
                    resData.Add(requestItem[i], request.Form[requestItem[i]]);
                }
                StringBuilder builder = new StringBuilder();
                logger.Info("receive back notify: " + SDKUtil.CreateLinkString(resData, false, true, System.Text.Encoding.UTF8));

                if (AcpService.Validate(resData, System.Text.Encoding.UTF8))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["respCode"] == "00" || queryString["respCode"] == "A6";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            return (queryString["respCode"] == "00" || queryString["respCode"] == "A6") ? "支付成功" : "支付失败";
        }

    }
}
