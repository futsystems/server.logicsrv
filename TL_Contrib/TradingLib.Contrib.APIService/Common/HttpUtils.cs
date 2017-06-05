using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.APIService
{
    public class HttpUtils
    {
        public static string UrlEncode(string str, Encoding code)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = code.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
    }
}
