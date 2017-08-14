using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace TradingLib.Contrib.Payment.QianTong
{
    /// <summary>
    /// RSA工具类
    /// </summary>
    class RSAUtil
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="originalString">签名数据</param>
        /// <returns>base64签名转码字符串</returns>
        public static string sign(string privateKey, string originalString)
        {
            try
            {
                byte[] data = Encoding.GetEncoding("utf-8").GetBytes(originalString);
                byte[] md5Byte = new MD5CryptoServiceProvider().ComputeHash(data);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(privateKey);
                return Convert.ToBase64String(rsa.SignData(md5Byte, "SHA1"));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="PublicKey">公钥</param>
        /// <param name="signatureString">原串</param>
        /// <param name="originalString">签名串</param>
        /// <returns></returns>
        public static bool veryfy(string PublicKey, string originalString, string signatureString)
        {
            //将base64签名数据转码为字节   
            byte[] orgin = Convert.FromBase64String(originalString);
            byte[] signedBase64 = Convert.FromBase64String(signatureString);
            RSACryptoServiceProvider oRSA = new RSACryptoServiceProvider();
            oRSA.FromXmlString(PublicKey);
            return oRSA.VerifyData(new MD5CryptoServiceProvider().ComputeHash(orgin), "SHA1", signedBase64);
        }
    }
}
