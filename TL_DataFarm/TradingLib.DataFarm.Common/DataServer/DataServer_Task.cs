using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.DataFarm.Common
{
    public partial class DataServer
    {
        /// <summary>
        /// 注册定时任务
        /// </summary>
        void RegisterTask()
        {
            //logger.Info("[Register Connection WatchTask]");
            //DataTask task1 = new DataTask("ConnectionWathTask", TimeSpan.FromSeconds(2), delegate() { ClearDeadClient(); });
            //Global.TaskService.RegisterTask(task1);

            

            logger.Info("[Register IQFeed TimeTick Watch]");
            DataTask task2 = new DataTask("IQFeedTimeTickWatchTask", TimeSpan.FromSeconds(1), delegate() { CheckIQFeedTimeTick(); });
            Global.TaskService.RegisterTask(task2);

            //500ms发送一次行情快照 此处通过update进行判定是否需要发送
            logger.Info("[Register TickSnapshot Sender]");
            DataTask task3 = new DataTask("TickSnapshotSenderTask", TimeSpan.FromSeconds(0.5), delegate() { SendTickSnapshot(); });
            Global.TaskService.RegisterTask(task3);


            DataTask statisticTask = new DataTask("StatisticPrint", TimeSpan.FromSeconds(30), delegate() 
                {

                    logger.Info(string.Format("Tick:{0}-{1} Bar:{2}-{3} MinuteData:{4}-{5} TradeSplit:{6}-{7} PriceVol:{8}-{9} Other:{10}-{11}",
                        dfStatistic.TickSendCnt, dfStatistic.TickSendSize,
                        dfStatistic.BarDataSendCnt, dfStatistic.BarDataSendSize,
                        dfStatistic.MinuteDataSendCnt, dfStatistic.MinuteDataSendSize,
                        dfStatistic.TradeSplitSendCnt, dfStatistic.TradeSplitSendSize,
                        dfStatistic.PriceVolSendCnt, dfStatistic.PriceVolSendSize,
                        dfStatistic.OtherPktSendCnt, dfStatistic.OtherPktSendSize));
                
                });
            Global.TaskService.RegisterTask(statisticTask);
        }

        
    }
}
