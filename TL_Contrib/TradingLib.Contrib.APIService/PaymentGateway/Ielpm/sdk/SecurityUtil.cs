using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using ielpm_merchant_web_demo.com.ielpm.merchant.web.sdk;


namespace ielpm_merchant_code_demo.com.ielpm.merchant.code.sdk
{
    public class SecurityUtil
    {
        public static string ALGORITHM_SHA1 = "SHA1";

        /// <summary>
        /// 摘要计算
        /// </summary>
        /// <param name="dataStr"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] Sha1X16(string dataStr, Encoding encoding)
        {
            try
            {
                byte[] data = encoding.GetBytes(dataStr);
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] sha1Res = sha1.ComputeHash(data, 0, data.Length);
                return sha1Res;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 软签名
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] SignBySoft(RSACryptoServiceProvider provider, byte[] data)
        {
            byte[] res = null;
            try
            {
                HashAlgorithm hashalg = new SHA1CryptoServiceProvider();
                res = provider.SignData(data, hashalg);
            }
            catch (Exception e)
            {
                throw e;
            }
            if (null == res) { return null; }
            return res;
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="base64DecodingSignStr"></param>
        /// <param name="srcByte"></param>
        /// <returns></returns>
        public static bool ValidateBySoft(RSACryptoServiceProvider provider, byte[] base64DecodingSignStr, byte[] srcByte)
        {
            HashAlgorithm hashalg = new SHA1CryptoServiceProvider();
            return provider.VerifyData(srcByte, hashalg, base64DecodingSignStr);
        }


        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="encode">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        public static string EncodeBase64(Encoding encode, string source)
        {
            string encodeStr = "";
            byte[] bytes = encode.GetBytes(source);
            try
            {
                encodeStr = Convert.ToBase64String(bytes);
            }
            catch
            {
                encodeStr = source;
            }
            return encodeStr;
        }



        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="encode">解码采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解码的密文</param>
        /// <returns>解码后的字符串</returns>
        public static string DecodeBase64(Encoding encode, string result)
        {
            string decodeStr = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decodeStr = encode.GetString(bytes);
            }
            catch
            {
                decodeStr = result;
            }
            return decodeStr;
        }


        ///<summary>
        /// 加密
        /// </summary>
        /// <returns></returns>
        public static byte[] encryptedData(byte[] encData)
        {
            try
            {
                X509Certificate2 pc = new X509Certificate2(CertUtil.PublicCertPath);
                RSACryptoServiceProvider p = new RSACryptoServiceProvider();

                p = (RSACryptoServiceProvider)pc.PublicKey.Key;

                byte[] enBytes = p.Encrypt(encData, false);


                return enBytes;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;

        }

       

        //对数据通过公钥进行加密，并进行base64计算
        public static string EncryptData(string dataString, string encoding)
        {
            /** 使用公钥对数据加密 **/
            byte[] data = null;
            try
            {
                data = encryptedData(UTF8Encoding.UTF8.GetBytes(dataString));
                return EncodeBase64(Encoding.Default, System.Text.Encoding.Default.GetString(data));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }
    }
}