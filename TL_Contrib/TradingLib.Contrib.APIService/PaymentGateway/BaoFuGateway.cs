using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Security;
using System.Security.Cryptography;
using Common.Logging;
using DotLiquid;

namespace TradingLib.Contrib.APIService
{
    public class BaoFuGateway:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("BaoFuGateway");

        public BaoFuGateway(GateWayConfig config)
            :base(config)
        {
            this.GateWayType = QSEnumGateWayType.BaoFu;

            this.PayUrl = "http://vgw.baofoo.com/payindex";
            this.MemberID = "100000178";
            this.TerminalID = "10000001";
            this.Md5Key = "abcdefg";

            this.InterfaceVersion = "4.0";
            this.KeyType = "1";
        }

        private string PayUrl { get; set; }
        //商户号
        private string MemberID { get; set; }

        //终端
        private string TerminalID { get; set; }

        //版本 当前为4.0请勿修改
        private string InterfaceVersion { get; set; }

        //加密方式默认1 MD5
        private string KeyType { get; set; }

        //加密参数
        private string Md5Key { get; set; }


        /// <summary>
        /// 支付过程
        /// 1.创建充值操作
        /// 2.支付网关对象利用充值操作对象生成对应的viewdata
        /// 3.通过viewdata渲染对应的页面用户执行确认跳转到支付网关页面
        /// </summary>
        /// <param name="operatioin"></param>
        /// <returns></returns>
        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropBaoFuPayment data = new DropBaoFuPayment();

            data.MemberID = this.MemberID;
            data.TerminalID = this.TerminalID;
            data.InterfaceVersion = this.InterfaceVersion;
            data.KeyType = this.KeyType;

            //招商银行是1001 空字符串跳转到宝付选择页面
            data.PayID = "";

            data.TradeDate = operatioin.DateTime.ToString();
            data.TransID = operatioin.Ref;
            data.OrderMoney = (operatioin.Amount * 100).ToFormatStr("{0:F0}");
            data.NoticeType = "1";
            data.PageUrl = APIGlobal.CustNotifyUrl+"/baofu";
            data.ReturnUrl =APIGlobal.SrvNotifyUrl+ "/baofu";
            data.Signature = GetMd5Sign(data.MemberID, data.PayID, data.TradeDate, data.TransID, data.OrderMoney, data.PageUrl, data.ReturnUrl, data.NoticeType, this.Md5Key);

            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);
            
            return data;
        }

        //md5签名
        /// <summary>
        /// 根据宝付协议规则计算签名
        /// md5key是不公开的 服务端可以通过保存的md5key获得具体的参数
        /// </summary>
        /// <param name="_MerchantID"></param>
        /// <param name="_PayID"></param>
        /// <param name="_TradeDate"></param>
        /// <param name="_TransID"></param>
        /// <param name="_OrderMoney"></param>
        /// <param name="_Page_url"></param>
        /// <param name="_Return_url"></param>
        /// <param name="_NoticeType"></param>
        /// <param name="_Md5Key"></param>
        /// <returns></returns>
        private string GetMd5Sign(string _MerchantID, string _PayID, string _TradeDate, string _TransID,
            string _OrderMoney, string _Page_url, string _Return_url, string _NoticeType, string _Md5Key)
        {
            string mark = "|";
            string str = _MerchantID + mark
                        + _PayID + mark
                        + _TradeDate + mark
                        + _TransID + mark
                        + _OrderMoney + mark
                        + _Page_url + mark
                        + _Return_url + mark
                        + _NoticeType + mark
                        + _Md5Key;
            return Md5Encrypt(str);

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


        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["TransID"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        /// <summary>
        /// 检查宝付的pageurl回调
        /// http://58.37.90.221:8050/custnotify?MemberID=100000178&TerminalID=10000001&TransID=635488241621875001&Result=1&ResultDesc=01&FactMoney=1&AdditionalInfo=&SuccTime=20141013191045&Md5Sign=49c6ef9b8c795edc0b597ca5b282fe96&BankID=3002
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="md5key"></param>
        public override bool CheckParameters(System.Collections.Specialized.NameValueCollection queryString)
        {
            string MemberID = queryString["MemberID"];//商户号
            string TerminalID = queryString["TerminalID"];//商户终端号
            string TransID = queryString["TransID"];//商户流水号
            string Result = queryString["Result"];//支付结果(1:成功,0:失败)
            string ResultDesc = queryString["ResultDesc"];//支付结果描述
            string FactMoney = queryString["FactMoney"];//实际成交金额
            string AdditionalInfo = queryString["AdditionalInfo"];//订单附加消息
            string SuccTime = queryString["SuccTime"];//交易成功时间
            string Md5Sign = queryString["Md5Sign"].ToLower();//md5签名
            string Md5Key = this.Md5Key;//密钥 双方约定
            String mark = "~|~";//分隔符

            string _WaitSign = "MemberID=" + MemberID + mark + "TerminalID=" + TerminalID + mark + "TransID=" + TransID + mark + "Result=" + Result + mark + "ResultDesc=" + ResultDesc + mark
                 + "FactMoney=" + FactMoney + mark + "AdditionalInfo=" + AdditionalInfo + mark + "SuccTime=" + SuccTime
                 + mark + "Md5Sign=" + Md5Key;

            if (Md5Sign.ToLower() == Md5Encrypt(_WaitSign).ToLower())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool CheckPayResult(System.Collections.Specialized.NameValueCollection queryString, CashOperation operation)
        {
            int result = int.Parse(queryString["Result"]);
            return result == 1;
        }

        public override string GetResultComment(System.Collections.Specialized.NameValueCollection queryString)
        {
            string ResultDesc = queryString["ResultDesc"];

            return ResultDesc;
        }
    }
}
