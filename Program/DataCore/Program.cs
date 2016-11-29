using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.Common.DataFarm;

using Common.Logging;


namespace DataCoreSrv
{
    class Program
    {

        const string PROGRAME = "DataCore";
        static ILog logger = LogManager.GetLogger(PROGRAME);
        static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                DataCore df = new DataCore();

                df.Start();
                System.Threading.Thread.Sleep(int.MaxValue);
                logger.Info("******************** exit");
            }
            catch (Exception ex)
            {
                logger.Error("Run DataCore Error:" + ex.ToString());
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            logger.Error("UnhandledException:" + ex.ToString());
        }
    }
}
