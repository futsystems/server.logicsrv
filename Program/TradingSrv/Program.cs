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
using TradingLib.BrokerXAPI;

using System.Runtime.InteropServices;

using Quartz;
using Quartz.Impl;

namespace TraddingSrvCLI
{

    class Program
    {

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
            //ISchedulerFactory schedFact = new StdSchedulerFactory();
            //IScheduler sched = schedFact.GetScheduler();
            //sched.Start();
            //// define the job and tie it to our HelloJob class
            //IJobDetail job = JobBuilder.Create<demoJob>()
            //    .WithIdentity("myJob", "group1")
            //    .Build();

            //// Trigger the job to run now, and then every 40 seconds
            //ITrigger trigger = TriggerBuilder.Create()
            //  .WithIdentity("myTrigger", "group1")
            //  .StartNow()
            //  .WithCronSchedule("0/2 * * * * ?")
            //  //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(9, 10))
            //    //.WithSimpleSchedule()
            //    //.WithSimpleSchedule(x => x
            //    //    .WithIntervalInSeconds(2)
            //    //    .RepeatForever())
            //  .Build();

            //sched.ScheduleJob(job, trigger);

            //Util.sleep(1000000);


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
