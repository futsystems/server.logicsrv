using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using System.IO;
using System.Text;
using TradingLib.BrokerXAPI;
using Common.Logging;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Text;


namespace TraddingSrvCLI
{
    class Program
    {
        static ILog log = Common.Logging.LogManager.GetLogger(Assembly.GetExecutingAssembly().GetName().Name);

        //static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        const string PROGRAME = "LogicSrv";

        static void debug(string message)
        {
            Console.WriteLine(message);
        }

        static void printleft(string msg, string msg1)
        {
            int len = Console.LargestWindowWidth / 2;
            int len2 = (len - msg.Length);
            string s =msg +msg1.PadLeft(len2-1);
            Console.WriteLine(s);
        }


        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                debug("********* start core daemon *********");
                //DateTime now = DateTime.Now;

                //DateTime start = new DateTime(now.Year, 1, 1);
                //HolidayCalculator hc = new HolidayCalculator(start, Util.GetConfigFile("holiday/usa.xml"));

                ////Console.WriteLine("\nHere are holidays for the 12 months following " + date.ToString("D") + ":");
                //foreach (HolidayCalculator.Holiday h in hc.OrderedHolidays)
                //{
                //    Console.WriteLine(h.Name + " - " + h.Date.ToString("D"));
                //}
                var core = new CoreThread();
                core.Run();              
            }
            catch (Exception ex)
            {
                debug("error:" + ex.ToString());
                Util.Debug(ex.ToString() + ex.StackTrace.ToString());
            }
        }

 
       
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Util.Debug(ex.ToString());
        }
    }

}
