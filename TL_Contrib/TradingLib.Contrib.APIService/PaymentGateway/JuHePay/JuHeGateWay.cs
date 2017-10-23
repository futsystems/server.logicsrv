﻿using Common.Logging;
using DotLiquid;
using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;

namespace TradingLib.Contrib.Payment.JuHe
{
    public class JuHeGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("JuHeGateWay");

        public JuHeGateWay(GateWayConfig config)
            : base(config)
        {

            this.GateWayType = QSEnumGateWayType.JuHe;
            var data = config.Config.DeserializeObject();
            this.PayUrl = data["PayUrl"].ToString();
            this.APPID = data["APPID"].ToString();
            this.Key = data["Key"].ToString(); 
        }

        string PayUrl = "https://119.23.246.110:9001/mps/v1/charges";
        string APPID = "app_OGEwZTZiMTZkN2E5";
        string Key = "sk_test_YmU1Njk3ZDEyMjNmZTE2Yjlk";
        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropJuHePayment data = new DropJuHePayment();
            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            data.order_no = operatioin.Ref;
            data.app_id = this.APPID;
            data.channel = "upacp_pc";
            data.Amount = ((int)(operatioin.Amount * 100)).ToString();
            data.client_ip = "127.0.0.1";
            data.subject = "充值";
            data.body = "充值";
            data.result_url = APIGlobal.CustNotifyUrl + "/juhe";
            data.notify_url = APIGlobal.SrvNotifyUrl + "/juhe";


            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("order_no", data.order_no);//必须唯一，不可重复
            parameters.Add("app[id]", data.app_id);
            parameters.Add("channel",data.channel);//请联系商务确认已开通的支付通道 upacp_wap：银联快捷  upacp_pc：银联网关   wx_wap：微信 H5 支付 alipay_wap：支付宝h5
            parameters.Add("amount", data.Amount);
            parameters.Add("client_ip", data.client_ip);
            parameters.Add("subject", data.subject);
            parameters.Add("body", data.body);
            parameters.Add("notify_url", data.notify_url);    //C/S架构请主动查询支付结果。
            parameters.Add("metadata","");
            parameters.Add("extra[result_url]",data.result_url);

            String str = HttpHelper.PostHttpResponseJson(this.PayUrl,null,parameters,this.Key);//status=200表示调用成功
            var respdata = str.DeserializeObject();
            logger.Info("response:" + respdata);
            try
            {
                data.url = respdata["credential"]["pay_url"].ToString();
            }
            catch (Exception ex)
            {

            }

            return data;
        }

        public static CashOperation GetCashOperation(NHttp.HttpRequest request)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string req = string.Empty;
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                req = Encoding.UTF8.GetString(data);
            }
            else
            {
                req = request.RawContent;
            }

            var respdata = req.DeserializeObject();
            string ordid = respdata["data"]["order_no"].ToString();

            return ORM.MCashOperation.SelectCashOperation(ordid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            string req = string.Empty;
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                req = Encoding.UTF8.GetString(data);
            }
            else
            {
                req = request.RawContent;
            }

            var Request = request.Params;

            return true;
        }

        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            string req = string.Empty;
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                req = Encoding.UTF8.GetString(data);
            }
            else
            {
                req = request.RawContent;
            }

            var respdata = req.DeserializeObject();
            string status = respdata["data"]["paid"].ToString();

            return status == "true";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            string req = string.Empty;
            if (request.ContentType == "application/json")
            {
                byte[] data = new byte[request.ContentLength];
                request.InputStream.Read(data, 0, request.ContentLength);
                request.InputStream.Position = 0;
                req = Encoding.UTF8.GetString(data);
            }
            else
            {
                req = request.RawContent;
            }

            var respdata = req.DeserializeObject();
            string status = respdata["data"]["paid"].ToString();
            return status == "true"  ? "支付成功" : "支付失败";
        }

    }
}
