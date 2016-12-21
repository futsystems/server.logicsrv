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

namespace TradingLib.Common.DataFarm
{
    public partial class DataServer
    {
        /// <summary>
        /// 注册定时任务
        /// </summary>
        void RegisterTask()
        {
            logger.Info("[Register Connection WatchTask]");
            DataTask task1 = new DataTask("ConnectionWathTask", TimeSpan.FromSeconds(2), delegate() { ClearDeadClient(); });
            Global.TaskService.RegisterTask(task1);

            logger.Info("[Register IQFeed TimeTick Watch]");
            DataTask task2 = new DataTask("IQFeedTimeTickWatchTask", TimeSpan.FromSeconds(1), delegate() { CheckIQFeedTimeTick(); });
            Global.TaskService.RegisterTask(task2);
        }
    }
}
