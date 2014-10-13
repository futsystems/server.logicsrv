using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.RechargeOnLine
{
    public class Payment
    {
        //MD5Key
        public string MD5Key;
        //商户号
        public string MerNo;
        //该笔订单总金额,必须为正数，保留两位有效小数。
        public string Amount;
        //商户网站产生的订单号[唯一]
        public string BillNo;
        //返回地址
        public string ReturnURL;
        //通知地址
        public string NotifyURL;
        //MD5加密信息 传递加密参数
        public string MD5info;
        //支付方式
        public string PayType;
        //支付结果返回码
        public string Succeed;
        //支付状态说明
        public string Result;
        
        //商户备注信息
        public string MerRemark;
    }
}
