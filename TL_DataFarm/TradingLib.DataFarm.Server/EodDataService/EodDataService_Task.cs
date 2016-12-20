using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.Common.DataFarm
{

    public partial class EodDataService
    {
        /// <summary>
        /// 初始化任务
        /// 每个品种开盘前5分钟定时任务 用于执行该品种的开盘前初始化操作
        /// </summary>
        void InitMarketDayTask()
        {
            Dictionary<int, List<SecurityFamily>> openTimeMap = new Dictionary<int, List<SecurityFamily>>();
            Dictionary<int, List<SecurityFamily>> closeTimeMap = new Dictionary<int, List<SecurityFamily>>();
            foreach (var security in MDBasicTracker.SecurityTracker.Securities)
            {
                MarketDay md = GetCurrentMarketDay(security);
                if (md == null) continue;

                int localPreOpenTime = security.Exchange.ConvertToSystemTime(md.MarketOpen.AddMinutes(-PREOPENMINITE)).ToTLTime();//将品种开盘时间转换成本地时间 提前5分钟进入开盘状态
                List<SecurityFamily> target = null;
                if (!openTimeMap.TryGetValue(localPreOpenTime, out target))
                {
                    target = new List<SecurityFamily>();
                    openTimeMap.Add(localPreOpenTime, target);
                }
                target.Add(security);

                int localPostCloseTime = security.Exchange.ConvertToSystemTime(md.MarketClose.AddMinutes(15)).ToTLTime();//将品种收盘时间转换成本地时间 延迟15分钟进入收盘状态
                if (!closeTimeMap.TryGetValue(localPostCloseTime, out target))
                {
                    target = new List<SecurityFamily>();
                    closeTimeMap.Add(localPostCloseTime, target);
                }
                target.Add(security);
            }
            foreach (var p in openTimeMap)
            {
                RegisterOpenTask(p.Key, p.Value);
            }
            foreach (var p in closeTimeMap)
            {
                RegisterCloseTask(p.Key, p.Value);
            }

            //logger.Info("MarketDayTask Registed");
        }

        /// <summary>
        /// 注册开盘Task
        /// </summary>
        /// <param name="time"></param>
        /// <param name="list"></param>
        void RegisterOpenTask(int time, List<SecurityFamily> list)
        {
            DateTime dt = Util.ToDateTime(Util.ToTLDate(), time);
            logger.Info(string.Format("[Register Open Task,Time:{0} Sec:{1}]", time, string.Join(",", list.Select(sec => sec.Code).ToArray())));
            DataTask task = new DataTask("OpenTask-" + dt.ToString("HH:mm:ss"), string.Format("{0} {1} {2} * * ?", dt.Second, dt.Minute, dt.Hour), delegate() { OpenMarket(list); });
            Global.TaskService.RegisterTask(task);
        }

        void RegisterCloseTask(int time, List<SecurityFamily> list)
        {
            DateTime dt = Util.ToDateTime(Util.ToTLDate(), time);
            logger.Info(string.Format("[Register Close Task,Time:{0} Sec:{1}]", time, string.Join(",", list.Select(sec => sec.Code).ToArray())));
            DataTask task = new DataTask("CloseTask-" + dt.ToString("HH:mm:ss"), string.Format("{0} {1} {2} * * ?", dt.Second, dt.Minute, dt.Hour), delegate() { logger.Info("Task:" + time.ToString()); });
            Global.TaskService.RegisterTask(task);

        }
    }
}
