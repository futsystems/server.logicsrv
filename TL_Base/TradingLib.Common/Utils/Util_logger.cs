using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace TradingLib.Common
{
    public static class Util_logger
    {
        const int MAXLENGTH = 100;
        public static void Status(this ILog logger, string component, string status)
        {
            string s = Util.padRightEx(string.Format(" {0} [{1}]", component, status), MAXLENGTH, '.');
            logger.Info(s);
        }

        public static void StatusLoad(this ILog logger, string component)
        {
            logger.Status(component, "LOAD");
        }

        public static void StatusInit(this ILog logger, string component)
        {
            logger.Status(component, "INIT");
        }

        public static void StatusStart(this ILog logger, string component)
        {
            logger.Status(component, "START");
        }

        public static void StatusStop(this ILog logger, string component)
        {
            logger.Status(component, "STOP");
        }

        public static void StatusDestory(this ILog logger, string component)
        {
            logger.Status(component, "DESTORY");
        }
    }
}
