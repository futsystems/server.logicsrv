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
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;


namespace TradingLib.Contrib.APIService
{

    public class TFBPayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("TFBPayGateWay");

        public TFBPayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.TFBPay;
            var data = config.Config.DeserializeObject();

            this.PayUrl = data["PayUrl"].ToString();// "http://apitest.tfb8.com/cgi-bin/v2.0/api_cardpay_apply.cgi";
            this.SPID = data["SPID"].ToString(); //"1800071515";
            this.MD5Key = data["MD5Key"].ToString(); //"12345";
            this.SPUserID = data["SPUserID"].ToString(); //"101347613";
            this.PublickKey = data["PublickKey"].ToString(); //"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCjDrkoVbyv4jTxeKtKEiK2mZiezQvfJV3sGhiwOnB+By5sa5Sa6Ls4dt5AGVqKHxyQVKRpu/utwtEt2MijWx45P1y2xGe7oDz2hUXP0j8sSa1NP26TmWHwO7czgJxxrdJ6RNqskSfjwsa5YMsqmcrumxUIxeCg5EOkgU26bnPoZQIDAQAB";
            this.PrivateKey = data["PrivateKey"].ToString(); //"MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAK+LzCZnUWIsRSxKyGZrZI+BU+Y+wnTXPpVbKcm5LT1fg/+o7aQR6B7pheWSEH5xLiFmtUkWSgZ7tYJhjovJkwgIJ91BQBg3rVT3xPCjeVu88mrdvzQOe6sS5WNPu3Wxbht9uACO16zupdDrruhjRUaCX5tkLukccU3bqp9FpkkNAgMBAAECgYBx8mB1nSLqgqnz8ibatGL185CuJ5a5mO36rM4XLqf66oEX9mMq2KS/S/2p4oHqUTUMYUrTQjCSvMI4+3I3soRI4k4J5VsyP9zHyHzafvNUTUyp2ybaVgmh3oxU4sx015fd+3Qc219l+Jdod+rIi68NJqhhMUU+q7yxmesCUCkZAQJBAOWH5bu9FmFIiSjWHVj6XE0904KOWSoHsenymzMZfM0s1kck1hUvwntUcmUhkiuz4BBmiKOy65MtNyJ6ChE3UP0CQQDDyi/gX/xOhCOpWoDMnYyKGyQH7GMJBIwK/X80Yha3Qtl/WrdqrpNV/ZHyQJgcIQFoMNLbNotoUOMAjthkrR1RAkAU5RAmzQnShVXnH8bAKNpqNayhf+/iAZ1SnMFAH5va2bAP/ex3NUfRDljzl+DElbVaCNt7e3gyh7UzMETmWFDJAkAwFtw1jz3ohxo/QYR7PYNEdLAf5hbZIy3GkUcKNcGAl8HWPxDn+iMkLtkHGIiD+DNhRQS1ZStOnvdyrqNF7yNRAkEAxm2MZmPHl+7jbDjHG6c+3SE6e0s7iZyatgh2gosKXdpqUWe3zVXPN04kLarZ7tasl1IBqHr1LpzdHEUReiNRBQ==";
            this.Test = bool.Parse(data["Test"].ToString()); //true;

            this.Charset = "GB2312";
        }


        public string SPID { get; set; }
        public string SPUserID { get; set; }
        public string MD5Key { get; set; }
        public string PublickKey { get; set; }
        public string PrivateKey { get; set; }
        public string PayUrl { get; set; }
        public  string Charset { get; set; }
        public bool Test { get; set; }
        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropTFBPayment data = new DropTFBPayment();

            data.spid = this.SPID;
            data.sp_userid = this.SPUserID;
            data.spbillno = operatioin.Ref;
            data.money = ((int)(operatioin.Amount * 100)).ToString();
            data.cur_type = "1";

            data.return_url = APIGlobal.CustNotifyUrl + "/tfbpay";
            data.notify_url = APIGlobal.SrvNotifyUrl + "/tfbpay";
            data.memo = "cs";
            data.card_type = "1";
            data.bank_segment = this.Test ? "1004" : operatioin.Bank;
            data.user_type = "1";
            data.channel = "1";
            data.encode_type = "MD5";
            data.PayUrl = this.PayUrl;

            data.Account = operatioin.Account;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);


            SortedDictionary<string, string> valdic = new SortedDictionary<string, string>();
            valdic.Add("spid", data.spid);
            valdic.Add("sp_userid", data.sp_userid);
            valdic.Add("spbillno", data.spbillno);
            valdic.Add("money", data.money);
            valdic.Add("cur_type", data.cur_type);
            valdic.Add("return_url", data.return_url);
            valdic.Add("notify_url", data.notify_url);
            valdic.Add("memo", data.memo);
            valdic.Add("card_type", data.card_type);
            valdic.Add("bank_segment", data.bank_segment);
            valdic.Add("user_type", data.user_type);
            valdic.Add("channel", data.channel);
            valdic.Add("encode_type", data.encode_type);

            string paramSrc = CreateLinkString(valdic);
            string sign = Sign(paramSrc, this.MD5Key, Charset);
            string encryptSrc = paramSrc + "&sign=" + sign;

            string publicKey =TFBRSAHelper.RSAPublicKeyJava2DotNet(this.PublickKey);
            string cipherData = Convert.ToBase64String(TFBRSAHelper.RSAEncryptByPublicKey(Encoding.GetEncoding(Charset).GetBytes(encryptSrc), publicKey));
            string cipherDataUrlEncoded = UrlEncode(cipherData, Charset);

            data.PayUrl = string.Format("{0}?cipher_data={1}", this.PayUrl, cipherDataUrlEncoded);
            /*
            //string retdata = "fgkVmU8uX%2BJjbyLU9vQHiNxUOB7Cls%2FNG0wQmbylzGT40MDYkalNoljdK2D3TODQS4FyAoE9vjih%2BceyrCS267YVcK%2F6tctKArOq%2Bl%2BVZSBbO6gLUbJ2ptc%2BkUmvlUNJUmuzehbyqLCNns%2FnUeXMOXEpq2Rxhrr1c4GjwTtmegk73EHY%2BI9S9T%2FlBngAD8xx1aW%2F6LgCImPEmOjpzmmv2JU5f%2Bu0ijd53cTekTQ0wipA9nc0RswImmMHvveroGC3KCpXs5rcFZjnBx9ojR6SAynSfqzRNWbm7XUpgjGiyVgyiCWeLqS7WpTZLh0Progv%2FVbPm%2B2bqsVUi7yefc8eSkxZ99lDq%2FctttFTkSNXs8zwcvYYXqDFY9tYhGZnN1R3EqIHXoOYyWaAgxgRX76QmWHSVQ%2FF2PuQUCYTXgTjHo1kZEP7ArDpq%2F8F3DUpTF%2ByBKCbeKTzOCQP06Qf3A5lZ8dwLBUVwjdjEbNqx7MyXsjGmVr8J0qk%2BHZPbG48sy%2FA";
            string cipret = "UktHruYk/a8tsWSIMSzHmsLy+X5NhdhwooFscLyR6c7/18o6RwxaL/3OnaufAgLcQbQVqQZlkIo0SfKx1XChxA84LErd1FvHmZDKKZbxeFwtFgQ5MgBet6hqlPNBKiREkTWeBuCc+OqN5Kouezia/S3nvyiUCzwqtPLjZeL9hlKHcrA6GbLhIwpr8Xdd7OeBwfFOMJM9WkTdX5Utn7i6TgfYbLss+yOCtjLT/6q50cgdvcLG43Uo1fQCIATX3Cf0D/ojxneGhpiqyh28ofwevSw6/Ut6gXvscyUFfiPJCXyO8M7HMHNVt1gmEGY/GJZQjeTjr+6X0oAUrWcfte/KCJf9PRgwblu7px1CZWoY6fkNijPUXvzH4mnwh80e89c6tNtEdW7XkFaFGh5ZGHTnOxZbjqE06nhOhu4aQYKmWhgz7SSLi9o//Athw4uNuURdaPij4WBi4fw0K8+FGSRUNKsON+E5JGFV8SUcRk2yRNjsW3x2kmvoS0xK2c3lV8uJ";

            string privateKey = TFBRSAHelper.RSAPrivateKeyJava2DotNet(this.PrivateKey);
            string rawData = TFBRSAHelper.RSADecryptByPrivateKey(cipret, privateKey, "GB2312");

            SortedDictionary<string, string> map = new SortedDictionary<string, string>();
            string[] ret = rawData.Split('&');
            
            foreach(var item in ret)
            {
                string[] ret2 = item.Split('=');
                map.Add(ret2[0], ret2[1]);
            }
            string source = CreateLinkString(map);
            string signbak = map["sign"];
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] hasbyte = sha.ComputeHash(Encoding.GetEncoding("GB2312").GetBytes(source));
            string hasstring = Convert.ToBase64String(hasbyte);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters para = new RSAParameters();
            rsa.FromXmlString(publicKey);
            bool ok = rsa.VerifyData(UTF8Encoding.GetEncoding("GB2312").GetBytes(source), "sha1", Convert.FromBase64String(signbak));

            string tmp1 = Convert.ToBase64String(TFBRSAHelper.RSADecryptByPublicKey(Convert.FromBase64String(sign), publicKey));
            **/

            return data;
        }



        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            string transid = queryString["spbillno"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            string data = request.Params["cipher_data"];
            var map = this.ParseArgs(data);

            if (map.Keys.Contains("result") && map["result"] == "1")
            {
                return true;
            }
            return false;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            string data = request.Params["cipher_data"];
            var map = this.ParseArgs(data);

            if (map.Keys.Contains("result") && map["result"] == "1")
            {
                return true;
            }
            return false;
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            string ResultDesc = queryString["retmsg"];

            return ResultDesc;
        }



        string CreateLinkString(SortedDictionary<string, string> dicArrayPre)
        {
            StringBuilder prestr = new StringBuilder();

            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                bool need = (temp.Key.ToLower() != "sign" && !string.IsNullOrEmpty(temp.Value));
                if (need)
                {
                    prestr.Append(temp.Key + "=" + temp.Value + "&");
                }
            }
            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);
            return prestr.ToString();
        }

        /// <summary>
        /// 签名字符串
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>签名结果</returns>
        string Sign(string prestr, string key, string charset = "UTF-8")
        {
            StringBuilder sb = new StringBuilder(32);

            prestr = prestr + "&key=" + key;

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }



        string UrlEncode(string str, string charset = "UTF-8")
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = Encoding.GetEncoding(charset).GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }

        public SortedDictionary<string, string> ParseArgs(string data)
        {
            SortedDictionary<string, string> map = new SortedDictionary<string, string>();
            try
            {
                string privateKey = TFBRSAHelper.RSAPrivateKeyJava2DotNet(PrivateKey);
                string rawData = Encoding.GetEncoding(Charset).GetString(TFBRSAHelper.RSADecryptByPrivateKey(Convert.FromBase64String(data), privateKey));

                string[] ret = rawData.Split('&');

                foreach (var item in ret)
                {
                    string[] ret2 = item.Split('=');
                    map.Add(ret2[0], ret2[1]);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Parse data error:" + ex.ToString());
            }

            return map;

            
            
        }
        //bool Verify(string source, string sign)
        //{
        //    string publicKey = TFBRSAHelper.RSAPublicKeyJava2DotNet(this.PublickKey);

        //}
       

    }
}
