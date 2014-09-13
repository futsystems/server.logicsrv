////创建类
//using System;
//using System.IO;
//using System.Net;
//using System.Text;
//using System.Text.RegularExpressions;
////using System.Windows.Forms;

//namespace TradingLib.Common
//{


//    /// <summary>
//    ///GetOutIP 的摘要说明
//    /// </summary>
//    public class WebFunction
//    {
//        public static string IP2Location(string ip)
//        {
//            try
//            {
//                String direction = "";
//                WebRequest request = WebRequest.Create("http://www.ip138.com/ips138.asp?ip=" + ip);
//                using (WebResponse response = request.GetResponse())
//                using (StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GB18030")))
//                {
//                    direction = stream.ReadToEnd();
//                }
//                //Search for the ip in the html
//                int first = direction.IndexOf("本站主数据：") + 6;
//                int last = direction.LastIndexOf("</li><li>参考数据一");
//                direction = direction.Substring(first, last - first);


//                return direction;
//            }
//            catch (Exception ex)
//            {
//                return string.Empty;
//            }
//        }

//        public static bool demo()
//        {
//            try
//            {
//                string str = "";
//                WebRequest request = WebRequest.Create("http://www.if888.com.cn/services/demo");
//                //模拟成ajax request
//                request.Headers.Add("x-requested-with:XMLHttpRequest");
//                using (StreamReader stream = new StreamReader(request.GetResponse().GetResponseStream()))
//                {
//                    str = stream.ReadToEnd();
//                    JRequestType type = JSONHelper.FromJSONString<JRequestType>(str);
//                    return type.IsAjax;
//                }
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//        }
//        public static double IPDelay()
//        {
//            try
//            {
//                string str = "";
//                double d = 0;
//                WebRequest request = WebRequest.Create("http://www.if888.com.cn/services/ipdelay/");
//                request.Headers.Add("x-requested-with:XMLHttpRequest");
//                using (StreamReader stream = new StreamReader(request.GetResponse().GetResponseStream()))
//                {
//                    str = stream.ReadToEnd();
//                    JIPDelay delay = JSONHelper.FromJSONString<JIPDelay>(str);
//                    d= delay.IPDelay;
//                }
//                request.Abort();
                
//                return d;
//            }
//            catch (Exception ex)
//            {
                
//                return 0;
//            }
//        }

//        public static string netIP()
//        {
//            try
//            {
//                string ipadd = string.Empty;
//                String direction = "";
//                WebRequest request = WebRequest.Create("http://cps.if888.com.cn/services/ipaddress/");
//                request.Headers.Add("x-requested-with:XMLHttpRequest");
//                using (StreamReader stream = new StreamReader(request.GetResponse().GetResponseStream()))
//                {
//                    direction = stream.ReadToEnd();

//                }
            
//                JIPAddress ipaddress = JSONHelper.FromJSONString<JIPAddress>(direction);
//                ipadd = ipaddress.IPAddress;
//                request.Abort();
//                return ipadd;
//            }
//            catch (Exception ex)
//            {

//                return string.Empty;
//            }
            
            


//        }
//    }
//}
