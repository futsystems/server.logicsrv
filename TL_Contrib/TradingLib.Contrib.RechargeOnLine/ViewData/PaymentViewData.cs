using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;
using DotLiquid;
using System.Security;
using System.Security.Cryptography;


namespace TradingLib.Contrib.RechargeOnLine
{
    public class PaymentViewData:Drop
    {

        JsonWrapperCashOperation _cashop = null;
        PayGWInfo _gwinfo = null;
        public PaymentViewData(JsonWrapperCashOperation cashop,PayGWInfo gwinfo)
        {
            _cashop = cashop;
            _gwinfo = gwinfo;
            
        }

        //交易帐号
        public string Account { get { return _cashop.Account; } }
        //原始资金数额
        public string Amount { get { return _cashop.Amount.ToString(); } }
        //出入金操作
        public string CashOperation { get { return Util.GetEnumDescription(_cashop.Operation); } }

        //商户号
        public string MemberID { get { return _gwinfo.MemberID; } }
        //终端
        public string TerminalID { get { return _gwinfo.TerminalID; } }
        //版本 当前为4.0请勿修改 
        public string InterfaceVersion { get { return _gwinfo.InterfaceVersion; } }
        //加密方式默认1 MD5
        public string KeyType { get { return _gwinfo.KeyType; } }
        //招商银行是1001 空字符串跳转到宝付选择页面
        public string PayID { get { return ""; } }
        //订单日期
        public string TradeDate { get { return _cashop.DateTime.ToString(); } }
        //商户订单号（交易流水号）(建议使用商户订单号加上贵方的唯一标识号)
        public string TransID { get { return _cashop.Ref; } }
        //订单金额，需要和卡面额一致(此处以分为单位)
        public string OrderMoney { get { return Util.FormatDecimal(_cashop.Amount * 100, "{0:F0}"); } }
        //商品名称
        //public string ProductName { get; set; }
        //通知方式 0 不跳转 1 会跳转
        public string NoticeType { get { return "1"; } }
        //客户端跳转地址
        public string PageUrl { get { return GWGlobals.PageUrl; } }
        //服务器端返回地址
        public string ReturnUrl { get { return GWGlobals.NotifyUrl; } }
        //加密参数
        public string Md5Key { get { return _gwinfo.Md5Key; } }
        /// <summary>
        /// 获得加密数据
        /// </summary>
        public string Signature { get { return GetMd5Sign(this.MemberID, this.PayID, this.TradeDate, this.TransID, this.OrderMoney, this.PageUrl, this.ReturnUrl, this.NoticeType, this.Md5Key); } }
        //md5签名
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
            return PayGwHelper.Md5Encrypt(str);

        }


       
    }
}
