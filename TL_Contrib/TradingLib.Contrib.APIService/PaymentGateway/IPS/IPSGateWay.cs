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
    public class IPSGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("IPSGateWay");

        public IPSGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.IPS;

            this.MerCode = "193499";
            this.Account = "193499";
            this.MD5Key = "VOeuPDR5lplJvT0qoXxTHSlgr5xS4nAgo4hEAy35yhrCcmDPwamThz2zYQ0ULPjSRjvJ72BRJEjsaBV6Kj3eTBA9KdW462N9uogM0kngOqdhAjt2Yqflbo4npkJ6yqCw";
            this.PayUrl = "https://newpay.ips.com.cn/psfp-entry/gateway/payment.do";

            this.ReturnUrl = APIGlobal.CustNotifyUrl + "/ips";
            this.NotifyUrl = APIGlobal.SrvNotifyUrl + "/ips";

            this.Domain = "dlsmaa.pw";
            this.ReturnUrl = this.ReturnUrl.Replace("127.0.0.1", this.Domain);
            this.NotifyUrl = this.NotifyUrl.Replace("127.0.0.1", this.Domain);
        }

        string MerCode { get; set; }

        string Account { get; set; }

        string MD5Key { get; set; }

        string PayUrl { get; set; }


        string ReturnUrl { get; set; }

        string NotifyUrl { get; set; }

        string Domain { get; set; }

        public override Drop CreatePaymentDrop(CashOperation operation)
        {
            DropIPSPayment data = new DropIPSPayment();

            data.Account = operation.Account;
            data.Amount = string.Format("{0}[{1}]", operation.Amount.ToFormatStr(), operation.Amount.ToChineseStr());
            data.Ref = operation.Ref;
            data.Operation = Util.GetEnumDescription(operation.OperationType);
            data.PayUrl = this.PayUrl;

            
            string body = string.Format("<body><MerBillNo>{0}</MerBillNo><Amount>{1}</Amount><Date>{2}</Date><CurrencyType>156</CurrencyType><GatewayType>01</GatewayType><Lang>GB</Lang><Merchanturl><![CDATA[{3}]]></Merchanturl><FailUrl><![CDATA[]]></FailUrl><Attach><![CDATA[]]></Attach><OrderEncodeType>5</OrderEncodeType><RetEncodeType>17</RetEncodeType><RetType>1</RetType><ServerUrl><![CDATA[{4}]]></ServerUrl><BillEXP></BillEXP><GoodsName>账户充值</GoodsName><IsCredit> </IsCredit><BankCode>1104</BankCode><ProductType> </ProductType></body>",
                operation.Ref,
                operation.Amount,
                Util.ToTLDate(),
                this.ReturnUrl,
                this.NotifyUrl);
            string strtosign = body + this.MerCode + this.MD5Key;
            string sign = Md5Encrypt(strtosign);

            data.GateWayReq = string.Format("<Ips><GateWayReq><head><Version>v1.0.0</Version><MerCode>{0}</MerCode><MerName></MerName><Account>{1}</Account><MsgId></MsgId><ReqDate>{2}</ReqDate><Signature>{3}</Signature></head>{4}</GateWayReq></Ips>",
                this.MerCode,
                this.Account,
                Util.ToTLDateTime(),
                sign,
                body                
                );
            return data;
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
