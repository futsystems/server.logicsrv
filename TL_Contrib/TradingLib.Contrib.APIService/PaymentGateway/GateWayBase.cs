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

                        
                        //string prikey = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAK+LzCZnUWIsRSxKyGZrZI+BU+Y+wnTXPpVbKcm5LT1fg/+o7aQR6B7pheWSEH5xLiFmtUkWSgZ7tYJhjovJkwgIJ91BQBg3rVT3xPCjeVu88mrdvzQOe6sS5WNPu3Wxbht9uACO16zupdDrruhjRUaCX5tkLukccU3bqp9FpkkNAgMBAAECgYBx8mB1nSLqgqnz8ibatGL185CuJ5a5mO36rM4XLqf66oEX9mMq2KS/S/2p4oHqUTUMYUrTQjCSvMI4+3I3soRI4k4J5VsyP9zHyHzafvNUTUyp2ybaVgmh3oxU4sx015fd+3Qc219l+Jdod+rIi68NJqhhMUU+q7yxmesCUCkZAQJBAOWH5bu9FmFIiSjWHVj6XE0904KOWSoHsenymzMZfM0s1kck1hUvwntUcmUhkiuz4BBmiKOy65MtNyJ6ChE3UP0CQQDDyi/gX/xOhCOpWoDMnYyKGyQH7GMJBIwK/X80Yha3Qtl/WrdqrpNV/ZHyQJgcIQFoMNLbNotoUOMAjthkrR1RAkAU5RAmzQnShVXnH8bAKNpqNayhf+/iAZ1SnMFAH5va2bAP/ex3NUfRDljzl+DElbVaCNt7e3gyh7UzMETmWFDJAkAwFtw1jz3ohxo/QYR7PYNEdLAf5hbZIy3GkUcKNcGAl8HWPxDn+iMkLtkHGIiD+DNhRQS1ZStOnvdyrqNF7yNRAkEAxm2MZmPHl+7jbDjHG6c+3SE6e0s7iZyatgh2gosKXdpqUWe3zVXPN04kLarZ7tasl1IBqHr1LpzdHEUReiNRBQ==";

                        //string privateKey = TFBRSAHelper.RSAPrivateKeyJava2DotNet(prikey);
                        //string rawData = Encoding.GetEncoding(charset).GetString(TFBRSAHelper.RSADecryptByPrivateKey(Convert.FromBase64String(data), privateKey));
                    }
                case "MOBOPAY":
                    {
                        return MoBoGateWay.GetCashOperation(request.Params);
                    }
                case "SUIXINGPAY":
                    {
                        return SuiXingPayGateWay.GetCashOperation(request.Params);
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
