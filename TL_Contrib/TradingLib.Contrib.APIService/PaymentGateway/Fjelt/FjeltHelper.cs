using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace TradingLib.Contrib.Payment.Fjelt
{
    public class AES
    {
        /// <summary>
        /// iv值要设置成一模一样
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string Encrypt(string toEncrypt, string key, string iv)
        {

            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] ivArray = Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Base64Method.EncryptBase64(resultArray);
        }

        public static string Decrypt(string toDecrypt, string key, string iv)
        {

            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] ivArray = Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = Base64Method.DecryptBase64ForByte(toDecrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);

        }

        internal static string MakeMd5(string str)
        {
            if (string.IsNullOrEmpty(str)) throw new Exception("str is null");
            ASCIIEncoding objAsc = new ASCIIEncoding();
            MD5 objMD5 = new MD5CryptoServiceProvider();
            byte[] arrRndHashPwd = objMD5.ComputeHash(objAsc.GetBytes(str));
            string strRndHashPwd = "";
            foreach (byte b in arrRndHashPwd)
            {
                if (b < 16)
                    strRndHashPwd = strRndHashPwd + "0" + b.ToString("X");
                else
                    strRndHashPwd = strRndHashPwd + b.ToString("X");
            }
            return strRndHashPwd;
        }

        public class Base64Method
        {
            public static string EncryptBase64(byte[] bt)
            {
                string result = "";
                try
                {

                    MemoryStream ms = new MemoryStream();
                    ms.Write(bt, 0, bt.Length);
                    ms.Close();
                    result = Convert.ToBase64String(ms.ToArray());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return result.Replace('+', '-').Replace('/', '_');

            }

            public static byte[] DecryptBase64ForByte(string a_strString)
            {
                try
                {
                    byte[] Buffer;

                    Buffer = Convert.FromBase64String(a_strString.Replace('-', '+').Replace('_', '/'));


                    return Buffer;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
