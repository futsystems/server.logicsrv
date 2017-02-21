using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;
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
        }

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
        /// <summary>
        /// 检查远端回调访问是否合法
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public virtual bool CheckParameters(System.Collections.Specialized.NameValueCollection queryString)
        {
            return false;
        }

        public virtual bool CheckPayResult(System.Collections.Specialized.NameValueCollection queryString, CashOperation operation)
        {
            return false;
        }

        public virtual string GetResultComment(System.Collections.Specialized.NameValueCollection queryString)
        {
            return string.Empty;
        }

        public static GateWayBase CreateGateWay(GateWayConfig config)
        {
            switch (config.GateWayType)
            { 
                case QSEnumGateWayType.BaoFu:
                    return new BaoFuGateway(config);
                default:
                    return null;
            }
        }
    }
}
