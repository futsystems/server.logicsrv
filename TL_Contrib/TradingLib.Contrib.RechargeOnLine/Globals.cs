using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.RechargeOnLine
{
    public static class GWGlobals
    {
        public static TemplateHelper TemplateHelper { get; set; }

        public static PayGWInfo PayGWInfo { get; set; }
        /// <summary>
        /// 页面跳转地址
        /// </summary>
        public static string PageUrl { get; set; }

        /// <summary>
        /// 服务器端通知地址
        /// </summary>
        public static string NotifyUrl { get; set; }
    }
}
