using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using TradingLib.ServiceManager;
using TradingLib.API;
using TradingLib.Common;
using System.IO;
using System.Text;



namespace TraddingSrvCLI
{
    class Program
    {

        //private static string padRightEx(string str, int totalByteCount)
        //{
        //    Encoding coding = Encoding.GetEncoding("gb2312");
        //    int dcount = 0;
        //    foreach (char ch in str.ToCharArray())
        //    {
        //        if (coding.GetByteCount(ch.ToString()) == 2)
        //            dcount++;
        //    }
        //    string w = str.PadRight(totalByteCount - dcount);
        //    return w;
        //}

        //public static string FieldName(string field, int width)
        //{
        //    return padRightEx(field, width);
        //}


        const string PROGRAME = "TraddingSrvCLI";

        static void debug(string message)
        {
            Console.WriteLine(message);
        }
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            try
            {
                debug("*********");
                //debug(double.MaxValue.ToString());
                CoreDaemon cd = new CoreDaemon();
                cd.SendDebugEvent +=new DebugDelegate(debug);
                cd.Start();
                
            }
            catch (Exception ex)
            {

                //throw (ex);
                debug("error:" + ex.ToString());
                Util.Debug(ex.ToString() + ex.StackTrace.ToString());
                //LibUtil.NewLog(PROGRAME, "main function error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
            
        }

 
       
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Util.Debug(ex.ToString());
            //LibUtil.NewLog(PROGRAME, "crash" + ex.ToString(), QSEnumDebugLevel.ERROR);
            //LibUtil.NewLog(PROGRAME, "crash" + ex.StackTrace.ToString(), QSEnumDebugLevel.ERROR); ;

        }
    }

}
