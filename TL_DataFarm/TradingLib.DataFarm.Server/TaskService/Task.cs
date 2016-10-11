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
            if (task is DataTask)
            {
                (task as DataTask).DoTask();
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
    public class DataTask
    {
        protected static ILog logger = LogManager.GetLogger("DataTask");

        /// <summary>
        /// 循环任务
        ///  string.Format("{0} {1} {2} * * ?", secend, minute, hour)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="taskInterval"></param>
        /// <param name="action"></param>
        public DataTask(string name, TimeSpan taskInterval, Action action)
        {
            this.Name = name;
            this.TaskInterval = taskInterval;
            this.TaskType = QSEnumTaskType.CIRCULATE;
            this.CronExpression = string.Empty;
            this.TaskDelegate = action;
        }

        /// <summary>
        /// 定时任务
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cron"></param>
        /// <param name="action"></param>
        public DataTask(string name, string cron, Action action)
        {
            this.Name = name;
            this.CronExpression = cron;
            this.TaskType = QSEnumTaskType.SPECIALTIME;
            this.TaskDelegate = action;
        }

        //public DataTaskBase(string name, QSEnumTaskType taskType, TimeSpan taskInterval, string cronExpression)
        //{
        //    this.Name = name;
        //    this.TaskType = taskType;
        //    this.TaskInterval = taskInterval;
        //    this.CronExpression = cronExpression;
            
        //}

        

        public Action TaskDelegate { get; set; }

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
            if (this.TaskDelegate != null)
            {
                this.TaskDelegate();
            }
            else
            {
                logger.Info("NoTask");
            }
        }
    }
}
