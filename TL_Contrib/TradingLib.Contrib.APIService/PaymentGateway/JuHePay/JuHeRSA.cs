using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Asn1;

namespace TradingLib.Contrib.Payment.JuHe
{

    /// <summary>
    /// RSA签名工具类。
    /// </summary>
    public class JuHeRSAUtil
    {

        /// <summary>
        /// java公钥转C#所需公钥
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static string RSAPublicKeyJava2DotNet(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        public static string RSAEncryptMore(string xmlPublicKey, string m_strEncryptString)
        {
            if (string.IsNullOrEmpty(m_strEncryptString))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(xmlPublicKey))
            {
                throw new ArgumentException("Invalid Public Key");
            }

            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                var inputBytes = Encoding.UTF8.GetBytes(m_strEncryptString);//有含义的字符串转化为字节流
                rsaProvider.FromXmlString(xmlPublicKey);//载入公钥
                int bufferSize = (rsaProvider.KeySize / 8) - 11;//单块最大长度
                var buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(inputBytes),
                     outputStream = new MemoryStream())
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
                    return Convert.ToBase64String(outputStream.ToArray());//转化为字节流方便传输
                }
            }
        }



        /// <summary>  
        /// RSA加密  
        /// </summary>  
        /// <param name="publicKeyCSharp"></param>  
        /// <param name="data"></param>  
        /// <returns></returns>  
        public static string EncryptCSharp(string publicKeyCSharp, string data, string encoding = "UTF-8")
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publicKeyCSharp);

            //☆☆☆☆.NET 4.6以后特有☆☆☆☆  
            //HashAlgorithmName hashName = new System.Security.Cryptography.HashAlgorithmName(hashAlgorithm);  
            //RSAEncryptionPadding padding = RSAEncryptionPadding.OaepSHA512;//RSAEncryptionPadding.CreateOaep(hashName);//.NET 4.6以后特有                 
            //cipherbytes = rsa.Encrypt(Encoding.GetEncoding(encoding).GetBytes(data), padding);  
            //☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆  

            //☆☆☆☆.NET 4.6以前请用此段代码☆☆☆☆  
            cipherbytes = rsa.Encrypt(Encoding.GetEncoding(encoding).GetBytes(data), false);

            return Convert.ToBase64String(cipherbytes);
        }



        /// <summary>
        /// 通过公钥分段解密数据
        /// </summary>
        /// <param name="str">密文数据</param>
        /// <param name="publicKey">公钥</param>
        /// <returns>明文数据</returns>
        public static string DecryptByPublicKey(string str, string publicKey)
        {
            str = str.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            //非对称加密算法，加解密用  
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());
            //解密  
            try
            {
                engine.Init(false, GetPublicKeyParameter(publicKey));
                byte[] byteData = Convert.FromBase64String(str);
                int inputLen = byteData.Length;
                int MAX_DECRYPT_BLOCK = 128;
                using (var crypStream = new MemoryStream())
                {
                    var offSet = 0;
                    int i = 0;
                    byte[] cache = null;
                    // 对数据分段解密  
                    while (inputLen - offSet > 0)
                    {
                        if (inputLen - offSet > MAX_DECRYPT_BLOCK)
                        {
                            cache = engine.ProcessBlock(byteData, offSet, MAX_DECRYPT_BLOCK);
                        }
                        else
                        {
                            cache = engine.ProcessBlock(byteData, offSet, inputLen - offSet);
                        }
                        i++;
                        offSet = i * MAX_DECRYPT_BLOCK;
                        crypStream.Write(cache, 0, cache.Length);
                    }
                    return System.Text.Encoding.UTF8.GetString(crypStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                return ex.Message;

            }
        }

        /// <summary>
        /// 获取公钥
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static AsymmetricKeyParameter GetPublicKeyParameter(string s)
        {
            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            byte[] publicInfoByte = Convert.FromBase64String(s);
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(publicInfoByte);//这里也可以从流中读取，从本地导入
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(publicInfoByte);
            return pubKey;
        }


    }
}