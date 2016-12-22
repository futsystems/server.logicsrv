using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.DataFarm.Common;

using Common.Logging;


namespace TickCoreSrv
{
    class Program
    {
        const string PROGRAME = "TickStore";
        static ILog logger = LogManager.GetLogger(PROGRAME);

        static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                TickServer tickStoreSrv = new TickServer();
                tickStoreSrv.Start(true);
            }
            catch (Exception ex)
            {
                logger.Error("Run TickStore Error:" + ex.ToString());
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            logger.Error("UnhandledException:" + ex.ToString());
        }
    }
}
