using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;
using NHttp;

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
        public virtual Drop CreatePaymentDrop(CashOperation operatioin)
        {
            return null;
        }

        public string SuccessReponse { get; set; }
        /// <summary>
        /// 检查远端回调访问是否合法
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public virtual bool CheckParameters(NHttp.HttpRequest request)
        {
            return false;
        }

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
                case QSEnumGateWayType.AliPay:
                    return new AliPayGateWay(config);
                case QSEnumGateWayType.IPS:
                    return new IPSGateWay(config);
                default:
                    return null;
            }
        }

        public virtual string DemoResponse(CashOperation op)
        {
            return string.Empty;
        }
    }
}
