using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using Quartz;

namespace TradingLib.Common.DataFarm
{
    public class DataJob : IJob
    {
        public DataJob()
        { 
        
        }

        public void Execute(IJobExecutionContext context)
        {
            //1.获得任务UUID
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            object task  = dataMap["job"];
            if (task is DataTaskBase)
            {
                (task as DataTaskBase).DoTask();
            }

            ////2.获得对应的任务
            //ITask task = TLCtxHelper.ModuleTaskCentre[taskuuid];

            //task.DoTask();
        }
    }

    /// <summary>
    /// 行情服务器任务基类
    /// 
    /// </summary>
    public class DataTaskBase
    {
        protected static ILog logger = LogManager.GetLogger("DataTask");

        
        public DataTaskBase(string name, QSEnumTaskType taskType, TimeSpan taskInterval, string cronExpression)
        {
            this.Name = name;
            this.TaskType = taskType;
            this.TaskInterval = taskInterval;
            this.CronExpression = cronExpression;

        }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 任务类别
        /// </summary>
        public QSEnumTaskType TaskType { get; set; }


        /// <summary>
        /// 执行间隔频率
        /// </summary>
        public TimeSpan TaskInterval { get; set; }


        /// <summary>
        /// 任务定时表达式 类似Cron
        /// </summary>
        public string CronExpression { get; set; }

        public virtual void DoTask()
        {
            logger.Info("DoTask");
        }
    }
}
