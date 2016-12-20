using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace TradingLib.Common.DataFarm
{

    public class TaskService
    {
        IScheduler _scheduler = null;
        ILog logger = LogManager.GetLogger("TaskService");

        public TaskService()
        {
            
        }

        bool _inited = false;
        public void Init()
        {
            if (_inited) return;
            //初始化定时任务
            //设置Quartz后台任务线程数为2 这里累计增加3个线程(1个是触发线程，2个是任务运行线程)
            System.Collections.Specialized.NameValueCollection kv = new System.Collections.Specialized.NameValueCollection();
            kv["quartz.threadPool.threadCount"] = "2";

            ISchedulerFactory schedFact = new StdSchedulerFactory(kv);
            _scheduler = schedFact.GetScheduler();

            _scheduler.ListenerManager.AddTriggerListener(new TrgierListener(), GroupMatcher<TriggerKey>.AnyGroup());
            _scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
            _inited = true;
        }

        public void RegisterTask(DataTask task)
        {
            //将任务添加到本地map
            //taskUUIDMap.TryAdd(task.TaskUUID, task);
            JobDataMap jm = new JobDataMap();
            jm.Add("job",task);
            //定时任务注册到scheduler
            if (task.TaskType == QSEnumTaskType.SPECIALTIME)
            {
                IJobDetail job = JobBuilder.Create<DataJob>()
                    .WithIdentity("Task-" + task.Name, "TaskGroup")
                    //.UsingJobData("TaskUUID", task.TaskUUID)
                    .UsingJobData(jm)
                    .Build();
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("Trigger-" + task.Name, "TriggerGroup")
                    .StartNow()
                    .WithCronSchedule(task.CronExpression)
                    .Build();

                _scheduler.ScheduleJob(job, trigger);
            }

            if (task.TaskType == QSEnumTaskType.CIRCULATE)
            {
                IJobDetail job = JobBuilder.Create<DataJob>()
                    .WithIdentity("Task-" + task.Name, "TaskGroup")
                    //.UsingJobData("TaskUUID", task.TaskUUID)
                    .UsingJobData(jm)
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("Trigger-" + task.Name, "TriggerGroup")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithInterval(task.TaskInterval)
                        .RepeatForever()
                        )
                    .Build();
                _scheduler.ScheduleJob(job, trigger);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            logger.Info("Start Task Service");
            _scheduler.Start();
        }
    }
}
