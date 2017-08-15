using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Net;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.Payment.XiaoXiao
{
    public class XiaoXiaoHepler
    {
        public static string sendPost(string Url, Hashtable myparams)
        {
            StringBuilder param = new StringBuilder();
            foreach (DictionaryEntry de in myparams)
            {
                Object value = de.Value;

                param.Append(de.Key).Append("=").Append(get_uft8(value.ToString())).Append("&");
            }

            string strparam = param.ToString().Substring(0, param.Length - 1);
            byte[] postData = Encoding.UTF8.GetBytes(strparam);//编码，尤其是汉字，事先要看下抓取网页的编码方式  
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  
            byte[] responseData = webClient.UploadData(new Uri(Url), "POST", postData);//得到返回字符流  
            string retString = Encoding.UTF8.GetString(responseData);//解码  

            return retString;
        }

        public static string createParams(Object obj)
        {
            if (obj == null)
            {
                return null;
            }
            Hashtable myparams = transBean2Map(obj);
            return createParams(myparams);
        }

        private static Hashtable transBean2Map(Object obj)
        {
            Hashtable map = new Hashtable();
            foreach (var p in obj.GetType().GetProperties())
            {
                map.Add(p.Name, p.GetValue(obj, null));
            }
            return map;
        }


        public static string createParams(Hashtable myparams)
        {
            StringBuilder builder = new StringBuilder();
            //需要先讲hashtable按照key排序。
            ArrayList al = new ArrayList(myparams.Keys);
            al.Sort();
            for (int i = 0; i < al.Count; i++)
            {
                string key = al[i].ToString();
                Object value = myparams[key];
                if (value != null && value.GetType().Equals(typeof(double)))
                    builder.Append(key).Append("=").Append(Math.Round(double.Parse(value.ToString()), 2).ToString("f2")).Append("&");
                else if (!isEmpty(value))
                {

                    builder.Append(key).Append("=").Append(value.ToString()).Append("&");
                }

            }
            return builder.ToString().Substring(0, builder.Length - 1);
        }
        private static bool isEmpty(Object myobject)
        {
            return myobject == null || myobject.Equals("");
        }

        //将字符串转为UTF-8格式
        public static string get_uft8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }
    }
}
