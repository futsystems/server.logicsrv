using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.APIService
{
    public class APIGlobal
    {
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
    }
}
