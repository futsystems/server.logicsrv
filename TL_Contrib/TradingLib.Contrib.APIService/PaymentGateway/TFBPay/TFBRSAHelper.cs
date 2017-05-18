using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using DotLiquid;
using System.Net;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

namespace TradingLib.Contrib.APIService
{
    public class TFBRSAHelper
    {

        
        /// <summary>
        /// 利用公钥加密原始数据
        /// </summary>
        /// <param name="content"></param>
        /// <param name="publickey"></param>
        /// <returns></returns>
        public static byte[] RSAEncryptByPublicKey(byte[] inputBytes, string publicKey)
        {
            try
            {
                using (var rsaProvider = new RSACryptoServiceProvider())
                {
                    //var inputBytes = Encoding.GetEncoding(charset).GetBytes(content);//有含义的字符串转化为字节流
                    rsaProvider.FromXmlString(publicKey);//载入公钥
                    int bufferSize = (rsaProvider.KeySize / 8) - 11;//单块最大长度
                    var buffer = new byte[bufferSize];
                    using (MemoryStream inputStream = new MemoryStream(inputBytes), outputStream = new MemoryStream())
                    {
                        while (true)
                        { //分段加密
                            int readSize = inputStream.Read(buffer, 0, bufferSize);
                            if (readSize <= 0)
                            {
                                break;
                            }

                            var temp = new byte[readSize];
                            Array.Copy(buffer, 0, temp, 0, readSize);
                            var encryptedBytes = rsaProvider.Encrypt(temp, false);
                            outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                        }
                        return outputStream.ToArray();// Convert.ToBase64String(outputStream.ToArray());//做base64转码
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// RSA私钥解密
        /// </summary>
        /// <param name="encryptedContent"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static byte[] RSADecryptByPrivateKey(byte[] inputBytes, string privateKey)
        {
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                //var inputBytes = Convert.FromBase64String(encryptedContent);
                rsaProvider.FromXmlString(privateKey);
                int bufferSize = rsaProvider.KeySize / 8;
                var buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(inputBytes),
                     outputStream = new MemoryStream())
                {
                    while (true)
                    {
                        int readSize = inputStream.Read(buffer, 0, bufferSize);
                        if (readSize <= 0)
                        {
                            break;
                        }

                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var rawBytes = rsaProvider.Decrypt(temp, false);
                        outputStream.Write(rawBytes, 0, rawBytes.Length);
                    }
                    return outputStream.ToArray();// Encoding.GetEncoding(charset).GetString(outputStream.ToArray());
                }
            }
        }

        ///// <summary>
        ///// 公钥解密
        ///// </summary>
        ///// <param name="content"></param>
        ///// <param name="publickey"></param>
        ///// <returns></returns>
        //public static byte[] RSADecryptByPublicKey(byte[] inputBytes, string publickey)
        //{
        //    using (var rsaProvider = new RSACryptoServiceProvider())
        //    {
        //        rsaProvider.FromXmlString(publickey);
        //        int bufferSize = rsaProvider.KeySize / 8;
        //        var buffer = new byte[bufferSize];
        //        using (MemoryStream inputStream = new MemoryStream(inputBytes),
        //             outputStream = new MemoryStream())
        //        {
        //            while (true)
        //            {
        //                int readSize = inputStream.Read(buffer, 0, bufferSize);
        //                if (readSize <= 0)
        //                {
        //                    break;
        //                }

        //                var temp = new byte[readSize];
        //                Array.Copy(buffer, 0, temp, 0, readSize);
        //                var rawBytes = rsaProvider.Decrypt(temp, false);
        //                outputStream.Write(rawBytes, 0, rawBytes.Length);
        //            }
        //            return outputStream.ToArray();
        //        }
        //    }
        //}
        //RSA私钥格式转换
        /// <summary>
        /// 将RSA私钥转换成C#模式 私钥为Base64格式字符串
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 将Java以Base64格式储存的公钥转换成C# RSA公钥
        /// JAVA中的公私钥使用base64进行存储，解码成字节数组后，首先需要生成相应的配置对象（PKCS#8,、X.509 ），根据配置对象生成RSA公私钥。
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
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
