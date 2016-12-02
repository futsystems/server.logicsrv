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
        public WatchDog(MQServer mqserver)
        {
            _mqServer = mqserver;
        }
        ManualResetEvent manualEvent = new ManualResetEvent(true);
        public void Join()
        {
            if (timer != null) return;
            timer = new System.Timers.Timer();
            timer.Interval = INTERVAL * 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
            logger.Info("WatchDog Started");
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_mqServer.IsLive)
            {
                if (DateTime.Now.Subtract(_mqServer.LastHeartBeatRecv).TotalSeconds >= INTERVAL * 3)
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
    }
}
