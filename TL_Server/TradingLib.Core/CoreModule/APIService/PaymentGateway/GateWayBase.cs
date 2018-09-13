using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;
using NHttp;

using TradingLib.Contrib.Payment;

namespace TradingLib.Contrib.APIService
{
    public class GateWayBase
    {

        public QSEnumGateWayType GateWayType { get; set; }

        GateWayConfig _config;
        protected GateWayConfig Config { get { return _config; } }
        public GateWayBase(GateWayConfig config)
        {
            _config = config;
            this.SuccessReponse = "success";
            this.PayDirectUrl = string.Format("{0}/cash/depositdirect?ref=", APIGlobal.BaseUrl);
        }

        public string PayDirectUrl { get; set; }

        public bool Avabile { get { return _config.Avabile; } }

        /// <summary>
        /// 创建支付页面数据集
        /// </summary>
        /// <param name="operatioin"></param>
        /// <returns></returns>
        public virtual Drop CreatePaymentDrop(CashOperation operatioin, HttpRequest request)
        {
            return null;
        }



        /// <summary>
        /// 获取通知成功返回
        /// 每个第三方 成功通知 返回不同
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string SuccessReponse { get; set; }

        /// <summary>
        /// 检查远端回调访问是否合法 验签名
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public virtual bool CheckParameters(NHttp.HttpRequest request)
        {
            return false;
        }

        /// <summary>
        /// 检查支付结果
        /// </summary>
        /// <param name="request"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public virtual bool CheckPayResult(NHttp.HttpRequest request,CashOperation operation)
        {
            return false;
        }

        public virtual string GetResultComment(NHttp.HttpRequest request)
        {
            return string.Empty;
        }

        public static GateWayBase CreateGateWay(GateWayConfig config)
        {
            switch (config.GateWayType)
            { 
                case QSEnumGateWayType.BaoFu:
                    return new BaoFuGateway(config);
                case QSEnumGateWayType.PlugPay:
                    return new TradingLib.Contrib.Payment.PlugPay.PlugPayGateWay(config);
                default:
                    return null;
            }
        }

        public static CashOperation GetOperation(string gateway, HttpRequest request,out bool gatewayExist)
        {
            gatewayExist = true;
            switch (gateway)
            {
                case "BAOFU":
                    {

                        return BaoFuGateway.GetCashOperation(request.Params);

                    }
               
                case "PLUGPAY":
                    {
                        return TradingLib.Contrib.Payment.PlugPay.PlugPayGateWay.GetCashOperation(request.Params);
                    }
                default:
                    {
                        gatewayExist = false;
                        return null;// "Not Support Gateway";
                    }
            }
        }
        public virtual string DemoResponse(CashOperation op)
        {
            return string.Empty;
        }
    }
}
