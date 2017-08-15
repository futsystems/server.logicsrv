using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;


namespace TradingLib.Contrib.Payment
{
    public class RSAUtil2
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="signStr"></param>
        /// <param name="privateKey"></param>
        /// <param name="encoding"></param>
        /// <param name="halg"></param>
        /// <returns></returns>
        public static string Sign(string signStr, string privateKey, string encoding = "UTF-8", string halg = "SHA1")
        {
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    var k = RSAPrivateKeyJava2DotNet(privateKey);
                    rsa.FromXmlString(k);
                    byte[] signBytes = rsa.SignData(Encoding.GetEncoding(encoding).GetBytes(signStr), halg);
                    return Convert.ToBase64String(signBytes);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="publicKey"></param>
        /// <param name="signedData"></param>
        /// <param name="encoding"></param>
        /// <param name="halg"></param>
        /// <returns></returns>
        public static bool Verify(string plainText, string publicKey, string signedData, string encoding = "UTF-8", string halg = "SHA1")
        {
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    var k = RSAPublicKeyJava2DotNet(publicKey);
                    rsa.FromXmlString(k);
                    return rsa.VerifyData(Encoding.GetEncoding(encoding).GetBytes(plainText), halg, Convert.FromBase64String(signedData));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        //RSA私钥格式转换
        public static string RSAPrivateKeyJava2DotNet(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
            return string.Format(
                "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned())
            );
        }

        //智付公钥格式转换
        public static string RSAPublicKeyJava2DotNet(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format(
                "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned())
            );
        }

    }
}
