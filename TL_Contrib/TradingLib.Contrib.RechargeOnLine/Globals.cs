using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.RechargeOnLine
{
    public static class GWGlobals
    {
        public static TemplateHelper TemplateHelper { get; private set; }

        public static void RegisterTemplate(TemplateHelper th)
        {
            TemplateHelper = th;
        }
        public static PayGWInfo GWInfo { get; private set; }

        public static void RegisterPayGW(PayGWInfo info)
        {
            GWInfo = info;
        }

    }
}
