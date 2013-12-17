using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace appdemo
{
    class Program
    {
        static void debug(string msg)
        {
            Console.WriteLine(msg);
        }
        static void Main(string[] args)
        {
            RSACryptoServiceProvider b = new RSACryptoServiceProvider(0x800);

            b.FromXmlString("<RSAKeyValue><Modulus>mAJtZbCEOO4yg+i0Gec1LDtRTOCZHqo0yy+Q+5UPwTuk+I4y7FZ5ohgmCqNVdNNzNsj3IUgggWXbAJI58XIvXeLqrPZ64Vn5DnTSXKWttzoH6pMWFSgKnmROBthgHxA/dgY7yNUZdmXo+yQaj9ehMDCfl3H0fWnZUE428yEb7js=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

            StreamReader reader = new StreamReader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.sn"));
            string[] strArray = reader.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < strArray.Length; i++)
            {
                debug(string.Format("#{0}:{1}", i, strArray[i]));
            }
            bool re = b.VerifyData(Encoding.UTF8.GetBytes(strArray[0]),"MD5", Convert.FromBase64String(strArray[1]));

            debug("verifyData ret:" + re.ToString());

            string dstr = "futs;xxx@xxx.com;100;667411488000000000";

            //Convert.ToBase64String();
            string inputinfo = strArray[0];

            string[] strArray2 = Encoding.UTF8.GetString(Convert.FromBase64String(inputinfo)).Split(new char[] { ';' });
            

            debug(string.Format("company:{0} email:{1} maxconn:{2} validdate:{3}", strArray2[0], strArray2[1], strArray2[2], new DateTime(long.Parse(strArray2[3]))));

            /*
            debug("----------------------------------------------------------------------");
            string lic = Convert.ToBase64String(Encoding.UTF8.GetBytes(dstr));

            byte[] licdata = Encoding.UTF8.GetBytes(lic);

            RSACryptoServiceProvider oRSA = new RSACryptoServiceProvider();
            string privatekey = oRSA.ToXmlString(true);//私钥
            string publickey = oRSA.ToXmlString(false);//公钥
            debug("pub:" + publickey);
            debug("");
            debug("pri:" + privatekey);

            //byte[] messagebytes = Encoding.UTF8.GetBytes(dstr);

            RSACryptoServiceProvider oRSA1 = new RSACryptoServiceProvider();
            oRSA1.FromXmlString(privatekey); //导入私钥 为数据签名
            byte[] sign = oRSA1.SignData(licdata, "MD5");

            debug("SIGNDATA:" + Convert.ToBase64String(sign));

            RSACryptoServiceProvider oRSA2 = new RSACryptoServiceProvider();
            oRSA2.FromXmlString(publickey);//公钥验证
            bool vret = oRSA2.VerifyData(licdata, "MD5", sign);

            debug("verify result:" + vret);

            strArray2 = Encoding.UTF8.GetString(Convert.FromBase64String(lic)).Split(new char[] { ';' });


            debug(string.Format("company:{0} email:{1} maxconn:{2} validdate:{3}", strArray2[0], strArray2[1], strArray2[2], new DateTime(long.Parse(strArray2[3]))));


            using (FileStream fs = new FileStream("lic", FileMode.Create))
            {
                //实例化一个StreamWriter-->与fs相关联  
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(privatekey);
                    sw.WriteLine(publickey);
                    sw.WriteLine(lic);
                    sw.WriteLine(Convert.ToBase64String(sign));

                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }      **/

        }
    }
}
