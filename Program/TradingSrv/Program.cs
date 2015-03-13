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
//using NLog;

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
            log.Debug("it is debug here???????????????");
            log.Error("some error happend");

            //logger.Debug("it is log to here");

            //Util.sleep(100000);
            debug("intsize:" + sizeof(int).ToString() + " doublesize:" + sizeof(double).ToString() + " boolsize:" + sizeof(bool).ToString());
            debug("OrderSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XOrderField)).ToString());
            debug("TradeSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XTradeField)).ToString());
            debug("ErrorSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XErrorField)).ToString());
            //XServerInfoField
            //debug("XServerInfoFieldSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XServerInfoField)).ToString());
            ////XUserInfoField
            debug("XUserInfoFieldSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XUserInfoField)).ToString());
            debug("XRspUserLoginFieldSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XRspUserLoginField)).ToString());
            debug("XOrderActionFieldSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XOrderActionField)).ToString());
            //return;
            //Util.Debug("Orders:" +TradingLib.Mixins.LitJson.JsonMapper.ToJson(new XOrderField()),QSEnumDebugLevel.WARNING);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                debug("********* start core daemon *********");
                CoreDaemon cd = new CoreDaemon();
                //cd.SendDebugEvent +=new DebugDelegate(debug);
                //启动核心守护
                cd.Start();
                
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
