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

                string xmlfile = Util.GetConfigFile("error.xml");
                XmlDocument xmlDoc = null;
                if (File.Exists(xmlfile))
                {
                    xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlfile);
                }

                List<XMLRspInfo> rsplist = new List<XMLRspInfo>();
                XmlNode xn = xmlDoc.SelectSingleNode("errors");
                XmlNodeList errors = xn.ChildNodes;
                Util.Debug("total errors:" + errors.Count.ToString());
                foreach (XmlNode node in errors)
                {
                    try
                    {
                        
                    }
                    catch (Exception ex)
                    {
                        Util.Debug("error:" + ex.ToString());
                    }

                }


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
