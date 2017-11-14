using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Common.Logging;

namespace FrontServer
{
    public class WatchDog
    {

        System.Timers.Timer timer = null;
        ILog logger = LogManager.GetLogger("WatchDog");

        MQServer _mqServer = null;
        const int INTERVAL = 3;
        public WatchDog(MQServer mqserver,CTPService.CTPServiceHost ctphost)
        {
            _mqServer = mqserver;
        }
        ManualResetEvent manualEvent = new ManualResetEvent(false);
        /// <summary>
        /// 启动服务并阻塞当前线程
        /// </summary>
        public void Join()
        {
            if (timer != null) return;
            timer = new System.Timers.Timer();
            timer.Interval = INTERVAL * 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
            logger.Info("WatchDog Started");

            //System.Threading.ThreadPool.QueueUserWorkItem(o => {
            //    while (true)
            //    {
            //        if (Console.ReadKey().Key == ConsoleKey.D)
            //        {
            //            logger.Info("Echo");
            //            Release();
            //        }
            //        else if (Console.ReadKey().Key == ConsoleKey.F4)
            //        {
            //            logger.Info("??");
            //            //捕捉Esc Set ManualResetEvent
            //            Release();
            //        }
            //    }
            //});
            manualEvent.WaitOne();
            

        }

        public void Release()
        {
            logger.Info("Stop WatchDog Service");
            manualEvent.Set();
        }
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (_mqServer.IsLive)
                {
                    if (DateTime.Now.Subtract(_mqServer.LastHeartBeatRecv).TotalSeconds >= INTERVAL * 10)
                    {
                        logger.Warn("MQServer's Backend Connectioin is dead");
                        _mqServer.Stop();
                    }
                    else
                    {
                        _mqServer.LogicHeartBeat();
                    }
                }

                if (_mqServer.IsStopped)
                {
                    _mqServer.Start();
                }
            }
            catch (Exception ex)
            {
                logger.Error("wath dog error:" + ex.ToString());
            }
        }
    }
}
