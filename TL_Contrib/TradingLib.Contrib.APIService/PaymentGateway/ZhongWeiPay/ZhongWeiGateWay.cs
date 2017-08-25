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
using TradingLib.Contrib.APIService;

namespace TradingLib.Contrib.Payment.ZhongWei
{
    public class ZhongWeiPayGateWay:GateWayBase
    {
        static ILog logger = LogManager.GetLogger("ZhongWeiPayGateWay");

        public ZhongWeiPayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.ZhongWeiPay;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.Account = data["MerID"].ToString();
            this.MD5Key = data["MD5Key"].ToString();

            this.SuccessReponse = "SUCCESS";
        }

        public string Account = "901497239068248535";
        public string MD5Key = "rALmPEVoPCvcVBJD5bj0Ls";
        public string PayUrl = "http://pay.zhongweipay.net/api/core.php";

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropZhongWeiPayment data = new DropZhongWeiPayment();
            data.account_no = this.Account;
            data.method = "00000003";
            data.productId = "01";
            data.version = "v1.0";
            data.nonce_str = operatioin.Ref;
            data.pay_tool = "wgzfxf";
            data.order_sn = operatioin.Ref;
            data.money = operatioin.Amount.ToFormatStr();
            data.ex_field = "";
            data.body = "充值卡";
            data.bankCode = operatioin.Bank;
            data.return_url = APIGlobal.CustNotifyUrl + "/zhongweipay";
            data.notify = APIGlobal.SrvNotifyUrl + "/zhongweipay";

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("account_no", data.account_no);
            dic.Add("method", data.method);
            dic.Add("productId", data.productId);
            dic.Add("version", data.version);
            dic.Add("nonce_str", data.nonce_str);
            dic.Add("pay_tool", data.pay_tool);
            dic.Add("order_sn", data.order_sn);
            dic.Add("money", data.money);
            dic.Add("ex_field", data.ex_field);
            dic.Add("body", data.body);
            dic.Add("bankCode", data.bankCode);
            dic.Add("notify", data.notify);
            dic.Add("return_url", data.return_url);

            string srcStr = CreateLinkString(dic);
            data.signature = MD5Sign(srcStr, MD5Key);


            return data;
        }

        public static CashOperation GetCashOperation(NHttp.HttpRequest request)
        {
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                string req = Encoding.UTF8.GetString(data);
                var ret = req.DeserializeObject();

                var orderref = ret["order_sn"].ToString();

                return ORM.MCashOperation.SelectCashOperation(orderref);
            }
            else
            {
                var orderref = request.Params["order_sn"];
                return ORM.MCashOperation.SelectCashOperation(orderref);
            }

        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("res_code", request.Params["res_code"]);
            dic.Add("res_msg", request.Params["res_msg"]);
            dic.Add("nonce_str", request.Params["nonce_str"]);
            dic.Add("status", request.Params["status"]);
            dic.Add("order_sn", request.Params["order_sn"]);
            dic.Add("money", request.Params["money"]);
            dic.Add("ex_field", request.Params["ex_field"]);

             string srcStr = CreateLinkString(dic);
             bool ret = request.Params["signature"] == MD5Sign(srcStr, MD5Key);

            return ret;
        }


        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                string req = Encoding.UTF8.GetString(data);
                var ret = req.DeserializeObject();
                return ret["status"].ToString() == "1";
            }
            else
            {
                return  request.Params["status"] == "1";
            }
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                string req = Encoding.UTF8.GetString(data);
                var ret = req.DeserializeObject();
                return ret["status"].ToString() == "1" ? "交易成功" : "交易失败";
            }
            else
            {
                return request.Params["status"] == "1" ? "交易成功" : "交易失败";
            }
        }



        public static string MD5Sign(string strToBeEncrypt, string md5Key)
        {
            string strSrc = strToBeEncrypt + md5Key;

            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] FromData = System.Text.Encoding.GetEncoding("utf-8").GetBytes(strSrc);
            Byte[] TargetData = md5.ComputeHash(FromData);
            string Byte2String = "";
            for (int i = 0; i < TargetData.Length; i++)
            {
                Byte2String += TargetData[i].ToString("x2");
            }
            return Byte2String.ToLower();
        }


        /// <summary>
        /// 1.除去数组中的空值和签名参数并以字母a到z的顺序排序
        /// 2.把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        public static string CreateLinkString(Dictionary<string, string> dicArrayPre)
        {
            SortedDictionary<string, string> dicArray = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                if (temp.Key.ToLower() != "signature" && !string.IsNullOrEmpty(temp.Value))
                {
                    dicArray.Add(temp.Key, temp.Value);
                }
            }

            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }
    }
}
