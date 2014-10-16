using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using HttpServer;
using System.Security;
using System.Security.Cryptography;


namespace TradingLib.Contrib.RechargeOnLine
{
    public class PayGwHelper
    {

        /// <summary>
        /// 检查宝付的pageurl回调
        /// http://58.37.90.221:8050/custnotify?MemberID=100000178&TerminalID=10000001&TransID=635488241621875001&Result=1&ResultDesc=01&FactMoney=1&AdditionalInfo=&SuccTime=20141013191045&Md5Sign=49c6ef9b8c795edc0b597ca5b282fe96&BankID=3002
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="md5key"></param>
        public static bool  CheckPaggeURL_Baofu(IParameterCollection queryString,string md5key)
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
            string Md5Key = md5key;//密钥 双方约定
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
