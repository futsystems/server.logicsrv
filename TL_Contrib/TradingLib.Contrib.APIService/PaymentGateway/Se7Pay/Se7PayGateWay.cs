using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using DotLiquid;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Contrib.APIService;


namespace TradingLib.Contrib.Payment.Se7Pay
{
    public class Se7PayGateWay : GateWayBase
    {
        static ILog logger = LogManager.GetLogger("Se7PayGateWay");

        public Se7PayGateWay(GateWayConfig config)
            : base(config)
        {
            this.GateWayType = QSEnumGateWayType.Se7Pay;

            this.PayUrl = "http://pay.se7en2020.cc/qcpay/qcPayCon";
            this.CompanyID = "QHQC_CS_PAY_06";
            this.PrivateKey = "MIIEpAIBAAKCAQEA9U1QCIcqOMV9tt9UjZMn0Dj/EuuwUkOT4/3jUlgdjHMsVIyJ+klApx6ffH3evX3Cd5dernCcdjF0zUlsAGm3aGctkuRJ1dlcHiH/7H7o2gLnna0BmEYIwavMDzCWhg4qoyPD3MF0uwq5/kQYjbN4cWW8DIEasYN1o/QDudkib9TfgWqTA9/nYTeNkS7MaflM5P58KhogmPHAgG4bkp8Yh9EAWSxRNKe4nPizgcdXW01GIX+PpSfMLKJR0Cc4nowlWEZSLCKskLWbGNDk9rCYJ261EBiRQu10Qg7RD9EJw5pwQIvdJ4HnxrtYNFmu2QLKNL77dsmNSaYp0HhjqY1/FwIDAQABAoIBAFhPryKC1tZ9cxblLCGT+t9hxaLgKyhSn+mSu8YnZuvrgugleH8c5CPGSJCfGBOMNPitDC8s54c9otlNN46mgJsbmjKp8VMF374Ra3uO7PGf3hbL9CkH/ZsL6HkHewlkDEGTXK4bD5VO9Uru5RAOrqvUfnDaAewqca3G6Zh87YLooebqLtVvcKsu4tdu5tpCOanoyTTB+qmQU1zDsnZuH0f/n4BvliHyrGeXZW2DtsfjWWp7v3OGJSLfQNjMqNX1TquNXXAS/hGx/1hDEp90S4gsqZYGm1JGVNxp1gsmyVUKv9XZCrCIBIByYRkuGqtowdnNyNo+4onkcn1AAOmWyvECgYEA+yxus+2MVJ+NPUPOiKSPO1SCaAPkSTbE4tBZUomBMEGcJrksaFQWCEb9cD3tkrZpcEOnSDFn4CnmzHYHwKlQyR2mag1AqlocEAm32eZe6m/pL85Z6ulIsybSGTbNbZZP6O3FxzkYHDqzE6TNX7Pj0gdT6/z0j4znqUqGMtbrM20CgYEA+gP/N025hVEyYAgW3uqpixBLnArVTWrHlHr9jXHhUqYBCO5px6iuhl1GZnmXGejPmLHjitcDCMCxoOk4jukI4NS9oCV5oMVYMSErdRCMPMLTTCfPhKPMNw/WPmbAmAi0DFy5iXgSuMVmD+B6RxlzzfE5pDXY7qAWwOlkz4XAphMCgYEA9M7Fx1EMqZ4pB0GDl+LsR6OCxuakJkffdkIkDTJAXExWadep1LXAE6k9c5yoZYqtEeKetNqSqAWvEjEoNTvRpXPksxDfqinRmEAglZzXfiA9Y374XOGrHhIETNcBOoJ6uEinsBmRylHcIJVMJq8qqZbC0QvnQdLQKEyeqAaZj7ECgYBoNv5NWTK55ayv5u9RoFNnyjUaKFHdYWMr+1Bxg0S/JL9Nr8OcHC4TiBHuaUSY4jiWl6AXoaR8I4ZnYo/W0kCHJ2abuMNIRrqKUB9DPtCWC0f0eKvByF05nnpZrI8fUJFsjcIxEUIMjwwKFjIiLPPepmy2jFOOtGCHrUH+VuyEzQKBgQDVJFEf9LqM7PNmUzTfjE+M/gHcyGTj83tHJ0TCchFhAycmAT/ugu/3JPQJUoF3n/hN+gXlMmxyAb7VaILt3M32Q4fkeSnoHati7UBN1uI1g+Oa2mM5Rb5Z8JhvE4SZLdnZD7WDNaXP7KNReVpLfoRvUcrOaItlBctkNASSyXJqtQ==";
            this.PublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAz+fsWNSBzpDB8bwxQVSwIl8umHRRKUUDo+glLFvMlBN0Fvg+RWYPlsxSfg0Ny6suK+2Yv+IZvcWmp+9YuTdgrYF/017hRbgf+UZGLySBa9DFlfiVWDpJfQrCbV3VEapUaPZEWvKBYexZu7xh2qCsJJN1rskrSpWLM+aiGE+TK7jCb3Vv8P5KSLjFIYbyGjpLf7TjopXrWjjtGP5CeE+BXRf3XFvHzrhvfIdnlFVfGpv0PIZExIou0xnqLu7UPcV5tSH9eQ2OqI97tLdRANvG9B6YfZ95+ExNWh/Yk4hA4d/8dqbNMhXiemHCS9+4H0M5OYguCeJ6vx5BEUjZQoIuZQIDAQAB";

            //this.PrivateKey ="MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALf/+xHa1fDTCsLYPJLHy80aWq3djuV1T34sEsjp7UpLmV9zmOVMYXsoFNKQIcEzei4QdaqnVknzmIl7n1oXmAgHaSUF3qHjCttscDZcTWyrbXKSNr8arHv8hGJrfNB/Ea/+oSTIY7H5cAtWg6VmoPCHvqjafW8/UP60PdqYewrtAgMBAAECgYEAofXhsyK0RKoPg9jA4NabLuuuu/IU8ScklMQIuO8oHsiStXFUOSnVeImcYofaHmzIdDmqyU9IZgnUz9eQOcYg3BotUdUPcGgoqAqDVtmftqjmldP6F6urFpXBazqBrrfJVIgLyNw4PGK6/EmdQxBEtqqgXppRv/ZVZzZPkwObEuECQQDenAam9eAuJYveHtAthkusutsVG5E3gJiXhRhoAqiSQC9mXLTgaWV7zJyA5zYPMvh6IviX/7H+Bqp14lT9wctFAkEA05ljSYShWTCFThtJxJ2d8zq6xCjBgETAdhiH85O/VrdKpwITV/6psByUKp42IdqMJwOaBgnnct8iDK/TAJLniQJABdo+RodyVGRCUB2pRXkhZjInbl+iKr5jxKAIKzveqLGtTViknL3IoD+Z4b2yayXg6H0g4gYj7NTKCH1h1KYSrQJBALbgbcg/YbeU0NF1kibk1ns9+ebJFpvGT9SBVRZ2TjsjBNkcWR2HEp8LxB6lSEGwActCOJ8Zdjh4kpQGbcWkMYkCQAXBTFiyyImO+sfCccVuDSsWS+9jrc5KadHGIvhfoRjIj2VuUKzJ+mXbmXuXnOYmsAefjnMCI6gGtaqkzl527tw=";

            //pkcs8_rsa_private_key_2048

            this.PrivateKey = "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQD1TVAIhyo4xX2231SNkyfQOP8S67BSQ5Pj/eNSWB2McyxUjIn6SUCnHp98fd69fcJ3l16ucJx2MXTNSWwAabdoZy2S5EnV2VweIf/sfujaAuedrQGYRgjBq8wPMJaGDiqjI8PcwXS7Crn+RBiNs3hxZbwMgRqxg3Wj9AO52SJv1N+BapMD3+dhN42RLsxp+Uzk/nwqGiCY8cCAbhuSnxiH0QBZLFE0p7ic+LOBx1dbTUYhf4+lJ8wsolHQJziejCVYRlIsIqyQtZsY0OT2sJgnbrUQGJFC7XRCDtEP0QnDmnBAi90ngefGu1g0Wa7ZAso0vvt2yY1JpinQeGOpjX8XAgMBAAECggEAWE+vIoLW1n1zFuUsIZP632HFouArKFKf6ZK7xidm6+uC6CV4fxzkI8ZIkJ8YE4w0+K0MLyznhz2i2U03jqaAmxuaMqnxUwXfvhFre47s8Z/eFsv0KQf9mwvoeQd7CWQMQZNcrhsPlU71Su7lEA6uq9R+cNoB7CpxrcbpmHztguih5uou1W9wqy7i127m2kI5qejJNMH6qZBTXMOydm4fR/+fgG+WIfKsZ5dlbYO2x+NZanu/c4YlIt9A2Myo1fVOq41dcBL+EbH/WEMSn3RLiCyplgabUkZU3GnWCybJVQq/1dkKsIgEgHJhGS4aq2jB2c3I2j7iieRyfUAA6ZbK8QKBgQD7LG6z7YxUn409Q86IpI87VIJoA+RJNsTi0FlSiYEwQZwmuSxoVBYIRv1wPe2StmlwQ6dIMWfgKebMdgfAqVDJHaZqDUCqWhwQCbfZ5l7qb+kvzlnq6UizJtIZNs1tlk/o7cXHORgcOrMTpM1fs+PSB1Pr/PSPjOepSoYy1uszbQKBgQD6A/83TbmFUTJgCBbe6qmLEEucCtVNaseUev2NceFSpgEI7mnHqK6GXUZmeZcZ6M+YseOK1wMIwLGg6TiO6Qjg1L2gJXmgxVgxISt1EIw8wtNMJ8+Eo8w3D9Y+ZsCYCLQMXLmJeBK4xWYP4HpHGXPN8TmkNdjuoBbA6WTPhcCmEwKBgQD0zsXHUQypnikHQYOX4uxHo4LG5qQmR992QiQNMkBcTFZp16nUtcATqT1znKhliq0R4p602pKoBa8SMSg1O9Glc+SzEN+qKdGYQCCVnNd+ID1jfvhc4aseEgRM1wE6gnq4SKewGZHKUdwglUwmryqplsLRC+dB0tAoTJ6oBpmPsQKBgGg2/k1ZMrnlrK/m71GgU2fKNRooUd1hYyv7UHGDRL8kv02vw5wcLhOIEe5pRJjiOJaXoBehpHwjhmdij9bSQIcnZpu4w0hGuopQH0M+0JYLR/R4q8HIXTmeelmsjx9QkWyNwjERQgyPDAoWMiIs896mbLaMU460YIetQf5W7ITNAoGBANUkUR/0uozs82ZTNN+MT4z+AdzIZOPze0cnRMJyEWEDJyYBP+6C7/ck9AlSgXef+E36BeUybHIBvtVogu3czfZDh+R5Kegdq2LtQE3W4jWD45raYzlFvlnwmG8ThJkt2dkPtYM1pc/so1F5Wkt+hG9Rys5oi2UFy2Q0BJLJcmq1";
            this.PublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA9U1QCIcqOMV9tt9UjZMn0Dj/EuuwUkOT4/3jUlgdjHMsVIyJ+klApx6ffH3evX3Cd5dernCcdjF0zUlsAGm3aGctkuRJ1dlcHiH/7H7o2gLnna0BmEYIwavMDzCWhg4qoyPD3MF0uwq5/kQYjbN4cWW8DIEasYN1o/QDudkib9TfgWqTA9/nYTeNkS7MaflM5P58KhogmPHAgG4bkp8Yh9EAWSxRNKe4nPizgcdXW01GIX+PpSfMLKJR0Cc4nowlWEZSLCKskLWbGNDk9rCYJ261EBiRQu10Qg7RD9EJw5pwQIvdJ4HnxrtYNFmu2QLKNL77dsmNSaYp0HhjqY1/FwIDAQAB";

            //pkcs8_rsa_private 1024
            //this.PrivateKey = "MIICeQIBADANBgkqhkiG9w0BAQEFAASCAmMwggJfAgEAAoGBAMZQMxko5lJGAm/dFmwhrfAO/ZfoBZRisqqNgF3EPYnnC+2pxKE82Cc6q3jzJDr4YdipDbdZFtraOz+VSktn+BLkWovicyTt4XmzswKTdA6jmSRlFzu/g/rpZdwQ0E79m2/VSlKLQhPpOBHHd+zaSwae/bi1v4F7x6QgucrzsXktAgMBAAECgYEAjN5BPdkAqmaSqpV4rPzLil+LodEtzXbChUYYbgLM191oar0SMJqAzHTvgu3ryXvQPU/wr1G4W2NzfLn2WVQUMU8q+HGdUPocfYePQRcMxwRX1saIE+10an0goio5SqqXb3KnKH/Y+SxMHmcGpLKjg73Z3Cd48w4d9jbssVOo2f0CQQDh0sgThAU05q0nBKfE3Z7HQnsNNRvh5kvAbyzOBIg6CBwgak2p5TJfflzkwIQmEvLTLofRKx3evAwhJdAu4+5TAkEA4NBSfvkNU2GOrcU40LjWyW7x5YCtgNgXLwR4IhW1AKMU6s//05zJOZXWabKCxKkEE+ICR9aGJFnLdEI2QZUKfwJBAKMvzZOEwLp0w8XBcHcr67R0jzWBHnAFGtKpxEfwMH03JRQrnYnbbaZiS0Cm+hgFc2I2asE1ljVB0MYXbDSmU68CQQCwTi+kLCTrMyL13CmGZq7rIfrKhXOMC+uCj/q2oFdmvH5299AlYFvesUeJdNlg6TutbB1lwTNJ5rHL5vEt1Xh5AkEApvBhJkZFamJBBpluvT5g8+KQvHIQp94J54ssnpBmCidN53Gq9wVZZv4aA+kNf34cFMtkdxHSNXgndlpEQLizNg==";
            //this.PublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDGUDMZKOZSRgJv3RZsIa3wDv2X6AWUYrKqjYBdxD2J5wvtqcShPNgnOqt48yQ6+GHYqQ23WRba2js/lUpLZ/gS5FqL4nMk7eF5s7MCk3QOo5kkZRc7v4P66WXcENBO/Ztv1UpSi0IT6TgRx3fs2ksGnv24tb+Be8ekILnK87F5LQIDAQAB";

        }


        string PayUrl = string.Empty;
        string CompanyID = string.Empty;
        string PrivateKey = "";
        string PublicKey = "";

        public override Drop CreatePaymentDrop(CashOperation operatioin)
        {
            DropSe7Payment data = new DropSe7Payment();

            data.company_oid = this.CompanyID;
            data.order_id = operatioin.Ref;
            data.item_name = "充值卡";
            data.item_info = "充值卡";
            data.money_num = operatioin.Amount.ToFormatStr();

            data.return_url = APIGlobal.CustNotifyUrl + "/se7pay";
            data.notify_url = APIGlobal.SrvNotifyUrl + "/se7pay";
            data.pay_type = "1";


            string signSrc = string.Format("company_oid={0}&item_info={1}&item_name={2}&money_num={3}&notify_url={4}&order_id={5}&pay_type={6}&return_url={7}", data.company_oid, data.item_info, data.item_name, data.money_num, data.notify_url, data.order_id, data.pay_type, data.return_url);

            string privateKey = RSAHelper.RSAPrivateKeyJava2DotNet(this.PrivateKey);
            string publicKey = RSAHelper.RSAPublicKeyJava2DotNet(this.PublicKey);

            data.sign = RSAHelper.RSASign(signSrc, privateKey);
            bool ret = RSAHelper.ValidateRsaSign(signSrc, publicKey, data.sign);

            data.PayUrl = this.PayUrl;
            data.Amount = string.Format("{0}[{1}]", operatioin.Amount.ToFormatStr(), operatioin.Amount.ToChineseStr());
            data.Ref = operatioin.Ref;
            data.Operation = Util.GetEnumDescription(operatioin.OperationType);

            return data;
        }

        public static CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        {
            //宝付远端回调提供TransID参数 为本地提供的递增的订单编号
            string transid = queryString["order_id"];
            return ORM.MCashOperation.SelectCashOperation(transid);
        }

        public override bool CheckParameters(NHttp.HttpRequest request)
        {
            //获取智付反馈信息
            var queryString = request.Params;
            string merchant_code = queryString["merchant_code"].ToString().Trim();
            string notify_type = queryString["notify_type"].ToString().Trim();
            string notify_id = queryString["notify_id"].ToString().Trim();
            string interface_version = queryString["interface_version"].ToString().Trim();
            string sign_type = queryString["sign_type"].ToString().Trim();
            string dinpaysign = queryString["sign"].ToString().Trim();
            string order_no = queryString["order_no"].ToString().Trim();
            string order_time = queryString["order_time"].ToString().Trim();
            string order_amount = queryString["order_amount"].ToString().Trim();
            string extra_return_param = queryString["extra_return_param"];
            string trade_no = queryString["trade_no"].ToString().Trim();
            string trade_time = queryString["trade_time"].ToString().Trim();
            string trade_status = queryString["trade_status"].ToString().Trim();
            string bank_seq_no = queryString["bank_seq_no"];
            /**
             *签名顺序按照参数名a到z的顺序排序，若遇到相同首字母，则看第二个字母，以此类推，
            *参数名1=参数值1&参数名2=参数值2&……&参数名n=参数值n
            **/
            //组织订单信息
            string signStr = "";

            if (null != bank_seq_no && bank_seq_no != "")
            {
                signStr = signStr + "bank_seq_no=" + bank_seq_no.ToString().Trim() + "&";
            }

            if (null != extra_return_param && extra_return_param != "")
            {
                signStr = signStr + "extra_return_param=" + extra_return_param + "&";
            }
            signStr = signStr + "interface_version=V3.0" + "&";
            signStr = signStr + "merchant_code=" + merchant_code + "&";


            if (null != notify_id && notify_id != "")
            {
                signStr = signStr + "notify_id=" + notify_id + "&notify_type=" + notify_type + "&";
            }

            signStr = signStr + "order_amount=" + order_amount + "&";
            signStr = signStr + "order_no=" + order_no + "&";
            signStr = signStr + "order_time=" + order_time + "&";
            signStr = signStr + "trade_no=" + trade_no + "&";
            signStr = signStr + "trade_status=" + trade_status + "&";

            if (null != trade_time && trade_time != "")
            {
                signStr = signStr + "trade_time=" + trade_time;
            }
            //将智付公钥转换成C#专用格式
            var key = DinpayHelper.RSAPublicKeyJava2DotNet(this.PublicKey);
            //验签
            bool result = DinpayHelper.ValidateRsaSign(signStr, key, dinpaysign);
            return result;
        }


        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["trade_status"] == "SUCCESS";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            string ResultDesc = queryString["trade_status"];

            return ResultDesc;
        }
    }
}
