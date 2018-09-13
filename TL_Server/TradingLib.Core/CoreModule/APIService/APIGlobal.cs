using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.APIService
{
    public class APIGlobal
    {
        /// <summary>
        /// 本地外网地址
        /// </summary>
        public static string LocalIPAddress = "127.0.0.1";

        public static string BaseUrl = "http://49.70.196.44:9999";

        public static string CustNotifyUrl
        {
            get {
                return BaseUrl + "/cash/custnotify";
            }
        }

        public static string SrvNotifyUrl
        {
            get
            {
                return BaseUrl + "/cash/srvnotify";
            }
        }

        public static List<string> ConfigServerIPList = new List<string>();
    }
}
