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
            var data = config.Config.DeserializeObject();

            //this.PayUrl = "http://pay.se7en2020.cc/qcpay/qcPayCon";
            //this.CompanyID = "QHQC_CS_PAY_06";
            
            //pkcs8_rsa_private_key_2048

            //this.PrivateKey = "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQD1TVAIhyo4xX2231SNkyfQOP8S67BSQ5Pj/eNSWB2McyxUjIn6SUCnHp98fd69fcJ3l16ucJx2MXTNSWwAabdoZy2S5EnV2VweIf/sfujaAuedrQGYRgjBq8wPMJaGDiqjI8PcwXS7Crn+RBiNs3hxZbwMgRqxg3Wj9AO52SJv1N+BapMD3+dhN42RLsxp+Uzk/nwqGiCY8cCAbhuSnxiH0QBZLFE0p7ic+LOBx1dbTUYhf4+lJ8wsolHQJziejCVYRlIsIqyQtZsY0OT2sJgnbrUQGJFC7XRCDtEP0QnDmnBAi90ngefGu1g0Wa7ZAso0vvt2yY1JpinQeGOpjX8XAgMBAAECggEAWE+vIoLW1n1zFuUsIZP632HFouArKFKf6ZK7xidm6+uC6CV4fxzkI8ZIkJ8YE4w0+K0MLyznhz2i2U03jqaAmxuaMqnxUwXfvhFre47s8Z/eFsv0KQf9mwvoeQd7CWQMQZNcrhsPlU71Su7lEA6uq9R+cNoB7CpxrcbpmHztguih5uou1W9wqy7i127m2kI5qejJNMH6qZBTXMOydm4fR/+fgG+WIfKsZ5dlbYO2x+NZanu/c4YlIt9A2Myo1fVOq41dcBL+EbH/WEMSn3RLiCyplgabUkZU3GnWCybJVQq/1dkKsIgEgHJhGS4aq2jB2c3I2j7iieRyfUAA6ZbK8QKBgQD7LG6z7YxUn409Q86IpI87VIJoA+RJNsTi0FlSiYEwQZwmuSxoVBYIRv1wPe2StmlwQ6dIMWfgKebMdgfAqVDJHaZqDUCqWhwQCbfZ5l7qb+kvzlnq6UizJtIZNs1tlk/o7cXHORgcOrMTpM1fs+PSB1Pr/PSPjOepSoYy1uszbQKBgQD6A/83TbmFUTJgCBbe6qmLEEucCtVNaseUev2NceFSpgEI7mnHqK6GXUZmeZcZ6M+YseOK1wMIwLGg6TiO6Qjg1L2gJXmgxVgxISt1EIw8wtNMJ8+Eo8w3D9Y+ZsCYCLQMXLmJeBK4xWYP4HpHGXPN8TmkNdjuoBbA6WTPhcCmEwKBgQD0zsXHUQypnikHQYOX4uxHo4LG5qQmR992QiQNMkBcTFZp16nUtcATqT1znKhliq0R4p602pKoBa8SMSg1O9Glc+SzEN+qKdGYQCCVnNd+ID1jfvhc4aseEgRM1wE6gnq4SKewGZHKUdwglUwmryqplsLRC+dB0tAoTJ6oBpmPsQKBgGg2/k1ZMrnlrK/m71GgU2fKNRooUd1hYyv7UHGDRL8kv02vw5wcLhOIEe5pRJjiOJaXoBehpHwjhmdij9bSQIcnZpu4w0hGuopQH0M+0JYLR/R4q8HIXTmeelmsjx9QkWyNwjERQgyPDAoWMiIs896mbLaMU460YIetQf5W7ITNAoGBANUkUR/0uozs82ZTNN+MT4z+AdzIZOPze0cnRMJyEWEDJyYBP+6C7/ck9AlSgXef+E36BeUybHIBvtVogu3czfZDh+R5Kegdq2LtQE3W4jWD45raYzlFvlnwmG8ThJkt2dkPtYM1pc/so1F5Wkt+hG9Rys5oi2UFy2Q0BJLJcmq1";
            //this.PublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA9U1QCIcqOMV9tt9UjZMn0Dj/EuuwUkOT4/3jUlgdjHMsVIyJ+klApx6ffH3evX3Cd5dernCcdjF0zUlsAGm3aGctkuRJ1dlcHiH/7H7o2gLnna0BmEYIwavMDzCWhg4qoyPD3MF0uwq5/kQYjbN4cWW8DIEasYN1o/QDudkib9TfgWqTA9/nYTeNkS7MaflM5P58KhogmPHAgG4bkp8Yh9EAWSxRNKe4nPizgcdXW01GIX+PpSfMLKJR0Cc4nowlWEZSLCKskLWbGNDk9rCYJ261EBiRQu10Qg7RD9EJw5pwQIvdJ4HnxrtYNFmu2QLKNL77dsmNSaYp0HhjqY1/FwIDAQAB";
            //七彩公钥
            //this.PublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAz+fsWNSBzpDB8bwxQVSwIl8umHRRKUUDo+glLFvMlBN0Fvg+RWYPlsxSfg0Ny6suK+2Yv+IZvcWmp+9YuTdgrYF/017hRbgf+UZGLySBa9DFlfiVWDpJfQrCbV3VEapUaPZEWvKBYexZu7xh2qCsJJN1rskrSpWLM+aiGE+TK7jCb3Vv8P5KSLjFIYbyGjpLf7TjopXrWjjtGP5CeE+BXRf3XFvHzrhvfIdnlFVfGpv0PIZExIou0xnqLu7UPcV5tSH9eQ2OqI97tLdRANvG9B6YfZ95+ExNWh/Yk4hA4d/8dqbNMhXiemHCS9+4H0M5OYguCeJ6vx5BEUjZQoIuZQIDAQAB";
            //pkcs8_rsa_private 1024
            //this.PrivateKey = "MIICeQIBADANBgkqhkiG9w0BAQEFAASCAmMwggJfAgEAAoGBAMZQMxko5lJGAm/dFmwhrfAO/ZfoBZRisqqNgF3EPYnnC+2pxKE82Cc6q3jzJDr4YdipDbdZFtraOz+VSktn+BLkWovicyTt4XmzswKTdA6jmSRlFzu/g/rpZdwQ0E79m2/VSlKLQhPpOBHHd+zaSwae/bi1v4F7x6QgucrzsXktAgMBAAECgYEAjN5BPdkAqmaSqpV4rPzLil+LodEtzXbChUYYbgLM191oar0SMJqAzHTvgu3ryXvQPU/wr1G4W2NzfLn2WVQUMU8q+HGdUPocfYePQRcMxwRX1saIE+10an0goio5SqqXb3KnKH/Y+SxMHmcGpLKjg73Z3Cd48w4d9jbssVOo2f0CQQDh0sgThAU05q0nBKfE3Z7HQnsNNRvh5kvAbyzOBIg6CBwgak2p5TJfflzkwIQmEvLTLofRKx3evAwhJdAu4+5TAkEA4NBSfvkNU2GOrcU40LjWyW7x5YCtgNgXLwR4IhW1AKMU6s//05zJOZXWabKCxKkEE+ICR9aGJFnLdEI2QZUKfwJBAKMvzZOEwLp0w8XBcHcr67R0jzWBHnAFGtKpxEfwMH03JRQrnYnbbaZiS0Cm+hgFc2I2asE1ljVB0MYXbDSmU68CQQCwTi+kLCTrMyL13CmGZq7rIfrKhXOMC+uCj/q2oFdmvH5299AlYFvesUeJdNlg6TutbB1lwTNJ5rHL5vEt1Xh5AkEApvBhJkZFamJBBpluvT5g8+KQvHIQp94J54ssnpBmCidN53Gq9wVZZv4aA+kNf34cFMtkdxHSNXgndlpEQLizNg==";
            //this.PublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDGUDMZKOZSRgJv3RZsIa3wDv2X6AWUYrKqjYBdxD2J5wvtqcShPNgnOqt48yQ6+GHYqQ23WRba2js/lUpLZ/gS5FqL4nMk7eF5s7MCk3QOo5kkZRc7v4P66WXcENBO/Ztv1UpSi0IT6TgRx3fs2ksGnv24tb+Be8ekILnK87F5LQIDAQAB";
            this.PayUrl = data["PayUrl"].ToString();
            this.CompanyID = data["CompanyID"].ToString();
            this.PrivateKey = data["PrivateKey"].ToString();
            this.PublicKey = data["PublickKey"].ToString();

            string privateKey = RSAHelper.RSAPrivateKeyJava2DotNet(this.PrivateKey);
            this.SuccessReponse = new { notify_status = "ok", notify_msg = "交易成功", sign = RSAHelper.RSASign("notify_msg=交易成功&notify_status=ok", privateKey) }.SerializeObject();
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
            data.bank_id = operatioin.Bank;




            string signSrc = string.Format("bank_id={8}&company_oid={0}&item_info={1}&item_name={2}&money_num={3}&notify_url={4}&order_id={5}&pay_type={6}&return_url={7}", data.company_oid, data.item_info, data.item_name, data.money_num, data.notify_url, data.order_id, data.pay_type, data.return_url,data.bank_id);

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
            string company_oid = queryString["company_oid"].ToString().Trim();
            string money_sum = queryString["money_sum"].ToString().Trim();

            string order_id = queryString["order_id"].ToString().Trim();
            string order_msg = queryString["order_msg"].ToString().Trim();
            string order_qhqc = queryString["order_qhqc"].ToString().Trim();
            string order_status = queryString["order_status"].ToString().Trim();
            
            string pay_type = queryString["pay_type"].ToString().Trim();

            string sign = queryString["sign"].ToString().Trim();
          
            /**
             *签名顺序按照参数名a到z的顺序排序，若遇到相同首字母，则看第二个字母，以此类推，
            *参数名1=参数值1&参数名2=参数值2&……&参数名n=参数值n
            **/
            //组织订单信息
            string signStr = string.Format("company_oid={0}&money_sum={1}&order_id={2}&order_msg={3}&order_qhqc={4}&order_status={5}&pay_type={6}", company_oid, money_sum, order_id, order_msg, order_qhqc, order_status, pay_type);


         
            //将智付公钥转换成C#专用格式
            var key = DinpayHelper.RSAPublicKeyJava2DotNet(this.PublicKey);
            //验签
            //bool result = DinpayHelper.ValidateRsaSign(signStr, key, sign);
            //return result;
            return order_status == "success";

        }


        public override bool CheckPayResult(NHttp.HttpRequest request, CashOperation operation)
        {
            var queryString = request.Params;
            return queryString["order_status"] == "success";
        }

        public override string GetResultComment(NHttp.HttpRequest request)
        {
            var queryString = request.Params;
            string ResultDesc = queryString["order_msg"];

            return ResultDesc;
        }
    }
}
