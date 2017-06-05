using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;

//using System.Numerics;
using System.IO;
using ielpm_merchant_web_demo.com.ielpm.merchant.web.sdk;

namespace ielpm_merchant_code_demo.com.ielpm.merchant.code.sdk
{
    public class CertUtil
    {
        public static string SignCertPath { get; set; }

        public static string PublicCertPath { get; set; }

        public static string CertPassword { get; set; }
        /// <summary>
        /// 获取签名证书私钥
        /// </summary>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetSignProviderFromPfx()
        {
            X509Certificate2 pc = new X509Certificate2(SignCertPath,CertPassword);
            return (RSACryptoServiceProvider)pc.PrivateKey;
        }


        /// <summary>
        /// 通过证书id，获取验证签名的证书
        /// </summary>
        /// <param name="certId"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetValidateProviderFromPath()
        {
            X509Certificate2 pc = new X509Certificate2(PublicCertPath);
            return (RSACryptoServiceProvider)pc.PublicKey.Key;
        }
    }
}