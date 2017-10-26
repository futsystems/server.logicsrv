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
                case QSEnumGateWayType.UnsPay:
                    return new UnspayGateWay(config);
                case QSEnumGateWayType.DinPay:
                    return new DinpayGateWay(config);
                case QSEnumGateWayType.ChinagPay:
                    return new ChinagpayGateWay(config);
                case QSEnumGateWayType.Cai1Pay:
                    return new Cai1payGateWay(config);
                case QSEnumGateWayType.GoPay:
                    return new GoPayGateWay(config);
                case QSEnumGateWayType.TFBPay:
                    return new TFBPayGateWay(config);
                case QSEnumGateWayType.MoBoPay:
                    return new MoBoGateWay(config);
                case QSEnumGateWayType.SuiXingPay:
                    return new SuiXingPayGateWay(config);
                case QSEnumGateWayType.IELPMPay:
                    return new IELPMPayGateWay(config);
                case QSEnumGateWayType.ETonePay:
                    return new TradingLib.Contrib.Payment.ETone.ETonePayGateWay(config);
                case QSEnumGateWayType.ZhiHPay:
                    return new TradingLib.Contrib.Payment.ZhiHPay.ZhiHPayGateWay(config);
                case QSEnumGateWayType.FZPay:
                    return new TradingLib.Contrib.Payment.FZPay.FZPayGateWay(config);
                case QSEnumGateWayType.DDBillPay:
                    return new TradingLib.Contrib.Payment.DDBill.DDBilPayGateWay(config);
                case QSEnumGateWayType.GaoHuiTong:
                    return new TradingLib.Contrib.Payment.GaoHuiTong.GaoHuiTongGateWay(config);
                case QSEnumGateWayType.Ecpss:
                    return new TradingLib.Contrib.Payment.Ecpss.EcpssGateWay(config);
                case QSEnumGateWayType.Se7Pay:
                    return new TradingLib.Contrib.Payment.Se7Pay.Se7PayGateWay(config);
                case QSEnumGateWayType.QianTong:
                    return new TradingLib.Contrib.Payment.QianTong.QianTongGateWay(config);
                case QSEnumGateWayType.Fjelt:
                    return new TradingLib.Contrib.Payment.Fjelt.FjeltGateWay(config);
                case QSEnumGateWayType.XiaoXiaoPay:
                    return new TradingLib.Contrib.Payment.XiaoXiao.XiaoXiaoPayGateWay(config);
                case QSEnumGateWayType.ZhongWeiPay:
                    return new TradingLib.Contrib.Payment.ZhongWei.ZhongWeiPayGateWay(config);
                case QSEnumGateWayType.P101KA:
                    return new TradingLib.Contrib.Payment.P101KA.P101KAGateWay(config);
                case QSEnumGateWayType.NewPay:
                    return new TradingLib.Contrib.Payment.NewPay.NewPayGateWay(config);
                case QSEnumGateWayType.HuiCX:
                    return new TradingLib.Contrib.Payment.HuiCX.HuiCXGateWay(config);
                case QSEnumGateWayType.GGTong:
                    return new TradingLib.Contrib.Payment.GGTong.GGTongGateWay(config);
                case QSEnumGateWayType.UnionPay:
                    return new TradingLib.Contrib.Payment.UnionPay.UnionPayGateWay(config);
                case QSEnumGateWayType.OpenEPay:
                    return new TradingLib.Contrib.Payment.OpenEPay.OpenEPayGateWay(config);
                case QSEnumGateWayType.Pay848:
                    return new TradingLib.Contrib.Payment.Pay848.Pay848GateWay(config);
                case QSEnumGateWayType.Shopping98:
                    return new TradingLib.Contrib.Payment.Shopping98.Shopping98GateWay(config);
                case QSEnumGateWayType.JoinPay:
                    return new TradingLib.Contrib.Payment.JoinPay.JoinPayGateWay(config);
                case QSEnumGateWayType.JuHe:
                    return new TradingLib.Contrib.Payment.JuHe.JuHeGateWay(config);
                case QSEnumGateWayType.SumPay:
                    return new TradingLib.Contrib.Payment.SumPay.SumPayGateWay(config);
                case QSEnumGateWayType.HMPay:
                    return new TradingLib.Contrib.Payment.HMPay.HMPayGateWay(config);
                case QSEnumGateWayType.C9Pay:
                    return new TradingLib.Contrib.Payment.C9Pay.C9PayGateWay(config);
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
                case "ALIPAY":
                    {
                        return AliPayGateWay.GetCashOperation(request.Params);
                    }
                case "IPS":
                    {
                        return IPSGateWay.GetCashOperation(request.Params);
                        //var ret = request.Params["paymentResult"];
                        //logger.Info("ret:" + ret);
                        //<Ips><GateWayRsp><head><ReferenceID></ReferenceID><RspCode>000000</RspCode><RspMsg><![CDATA[交易成功！]]></RspMsg><ReqDate>20170305091219</ReqDate><RspDate>20170305091340</RspDate><Signature>f04f976b94172bc47bffa8c27491c347</Signature></head><body><MerBillNo>636243016596374014</MerBillNo><CurrencyType>156</CurrencyType><Amount>0.01</Amount><Date>20170305</Date><Status>Y</Status><Msg><![CDATA[支付成功！]]></Msg><IpsBillNo>BO20170305091102004402</IpsBillNo><IpsTradeNo>2017030509121984702</IpsTradeNo><RetEncodeType>17</RetEncodeType><BankBillNo>7109877764</BankBillNo><ResultType>0</ResultType><IpsBillTime>20170305091340</IpsBillTime></body></GateWayRsp></Ips>
                        //break;
                    }
                case "UNSPAY":
                    {
                        return UnspayGateWay.GetCashOperation(request.Params);
                    }
                case "DINPAY":
                    {
                        return DinpayGateWay.GetCashOperation(request.Params);
                    }
                case "CHINAGPAY":
                    {
                        return ChinagpayGateWay.GetCashOperation(request.Params);
                    }
                case "CAI1PAY":
                    {
                        return Cai1payGateWay.GetCashOperation(request.Params);
                    }
                case "GOPAY":
                    {
                        return GoPayGateWay.GetCashOperation(request.Params);
                    }
                case "TFBPAY":
                    {
                        string data = request.Params["cipher_data"];
                        string transid = string.Empty;

                        //获得所有设置的天付宝网关 由于天付宝网关全密封送 导致无法获得对应Operation需要先尝试解密
                        IEnumerable<GateWayBase> gateways = APITracker.GateWayTracker.GetTFBGateways();
                        foreach (var item in gateways)
                        {
                            TFBPayGateWay gw = item as TFBPayGateWay;
                            if (gw == null) continue;
                            var map = gw.ParseArgs(data);
                            if (map.Keys.Contains("spbillno"))
                            {
                                transid = map["spbillno"];
                            }
                        }
                        return ORM.MCashOperation.SelectCashOperation(transid);
                    }
                case "MOBOPAY":
                    {
                        return MoBoGateWay.GetCashOperation(request.Params);
                    }
                case "SUIXINGPAY":
                    {
                        return SuiXingPayGateWay.GetCashOperation(request.Params);
                    }
                case "IELPMPAY":
                    {
                        return IELPMPayGateWay.GetCashOperation(request.Params);
                    }
                case "ETONEPAY":
                    {
                        return TradingLib.Contrib.Payment.ETone.ETonePayGateWay.GetCashOperation(request.Params);
                    }
                case "ZHIHPAY":
                    {
                        return TradingLib.Contrib.Payment.ZhiHPay.ZhiHPayGateWay.GetCashOperation(request.Params);
                    }
                case "FZPAY":
                    {
                        return TradingLib.Contrib.Payment.FZPay.FZPayGateWay.GetCashOperation(request.Params);
                    }
                case "DDBILLPAY":
                    {
                        return TradingLib.Contrib.Payment.DDBill.DDBilPayGateWay.GetCashOperation(request.Params);
                    }
                case "GAOHUITONG":
                    {
                        return TradingLib.Contrib.Payment.GaoHuiTong.GaoHuiTongGateWay.GetCashOperation(request.Params);
                    }
                case "ECPSS":
                    {
                        return TradingLib.Contrib.Payment.Ecpss.EcpssGateWay.GetCashOperation(request.Params);
                    }
                case "SE7PAY":
                    {
                        return TradingLib.Contrib.Payment.Se7Pay.Se7PayGateWay.GetCashOperation(request.Params);
                    }
                case "QIANTONG":
                    {
                        string transID = string.Empty;
                        IEnumerable<GateWayBase> gws = APITracker.GateWayTracker.GetGateway(QSEnumGateWayType.QianTong);
                        foreach (var item in gws)
                        {
                            TradingLib.Contrib.Payment.QianTong.QianTongGateWay gw = item as TradingLib.Contrib.Payment.QianTong.QianTongGateWay;
                            if (gw == null) continue;
                            var val = gw.ParseTransID(request);
                            if (!string.IsNullOrEmpty(val))
                            {
                                transID = val;
                            }
                        }
                        return ORM.MCashOperation.SelectCashOperation(transID);
                       
                    }
                case "FJELT":
                    {
                        return TradingLib.Contrib.Payment.Fjelt.FjeltGateWay.GetCashOperation(request.Params);
                    }

                case "XIAOXIAOPAY":
                    {
                        return TradingLib.Contrib.Payment.XiaoXiao.XiaoXiaoPayGateWay.GetCashOperation(request);
                    }
                case "ZHONGWEIPAY":
                    {
                        return TradingLib.Contrib.Payment.ZhongWei.ZhongWeiPayGateWay.GetCashOperation(request);
                    }
                case "P101KA":
                    {
                        return TradingLib.Contrib.Payment.P101KA.P101KAGateWay.GetCashOperation(request.Params);
                    }
                case "NEWPAY":
                    {
                        return TradingLib.Contrib.Payment.NewPay.NewPayGateWay.GetCashOperation(request.Params);
                    }
                case "HUICX":
                    {
                        return TradingLib.Contrib.Payment.HuiCX.HuiCXGateWay.GetCashOperation(request.Params);
                    }
                case "GGTONG":
                    {
                        return TradingLib.Contrib.Payment.GGTong.GGTongGateWay.GetCashOperation(request.Params);
                    }
                case "UNIONPAY":
                    {
                        return TradingLib.Contrib.Payment.UnionPay.UnionPayGateWay.GetCashOperation(request.Params);
                    }
                case "OPENEPAY":
                    {
                        return TradingLib.Contrib.Payment.OpenEPay.OpenEPayGateWay.GetCashOperation(request.Params);
                    }
                case "PAY848":
                    {
                        return TradingLib.Contrib.Payment.Pay848.Pay848GateWay.GetCashOperation(request.Params);
                    }
                case "SHOPPING98":
                    {
                        return TradingLib.Contrib.Payment.Shopping98.Shopping98GateWay.GetCashOperation(request.Params);
                    }
                case "JOINPAY":
                    {
                        return TradingLib.Contrib.Payment.JoinPay.JoinPayGateWay.GetCashOperation(request.Params);
                    }
                case "JUHE":
                    {
                        return TradingLib.Contrib.Payment.JuHe.JuHeGateWay.GetCashOperation(request);
                    }
                case "SUMPAY":
                    {
                        return TradingLib.Contrib.Payment.SumPay.SumPayGateWay.GetCashOperation(request.Params);
                    }
                case "HMPAY":
                    {
                        return TradingLib.Contrib.Payment.HMPay.HMPayGateWay.GetCashOperation(request.Params);
                    }
                case "C9PAY":
                    {
                        return TradingLib.Contrib.Payment.C9Pay.C9PayGateWay.GetCashOperation(request.Params);
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
