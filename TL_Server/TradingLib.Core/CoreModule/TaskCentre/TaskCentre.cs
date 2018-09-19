using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;


namespace TradingLib.Core
{

    /// <summary>
    /// 系统内部实现了一个Task模型，用于定时的执行某些任务
    /// 任务分2种类型
    /// 1.几点几分几秒执行的任务
    /// 2.每隔多少时间执行的任务
    /// </summary>
    public partial class TaskCentre : BaseSrvObject, IModuleTaskCentre
    {
        const string CoreName = "TaskCentre";
        public string CoreId { get { return this.PROGRAME; } }

        IScheduler _scheduler = null;
        public TaskCentre()
            : base(TaskCentre.CoreName)
        {
            //设置Quartz后台任务线程数为2 这里累计增加3个线程(1个是触发线程，2个是任务运行线程)
            System.Collections.Specialized.NameValueCollection kv = new System.Collections.Specialized.NameValueCollection();
            kv["quartz.threadPool.threadCount"] = "2";

            ISchedulerFactory schedFact = new StdSchedulerFactory(kv);
            _scheduler = schedFact.GetScheduler();

            _scheduler.ListenerManager.AddTriggerListener(new TrgierListener(), GroupMatcher<TriggerKey>.AnyGroup());
            _scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
        }

        ConcurrentDictionary<string, ITask> taskUUIDMap = new ConcurrentDictionary<string, ITask>();

        /// <summary>
        /// 注册一个Task
        /// </summary>
        /// <param name="task"></param>
        public void RegisterTask(ITask task)
        {
            //将任务添加到本地map
            taskUUIDMap.TryAdd(task.TaskUUID, task);

            //定时任务注册到scheduler
            if (task.TaskType == QSEnumTaskType.SPECIALTIME)
            {
                IJobDetail job = JobBuilder.Create<CoreTask>()
                    .WithIdentity("Task-" + task.TaskUUID, "TaskGroup")
                    .UsingJobData("TaskUUID", task.TaskUUID)
                    .Build();

                //debug(" task xxxxxxxxxxx cron:" + task.CronExpression, QSEnumDebugLevel.ERROR);

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("Trigger-" + task.TaskUUID, "TriggerGroup")
                    .StartNow()
                    .WithCronSchedule(task.CronExpression)
                    .Build();

                _scheduler.ScheduleJob(job, trigger);
            }

            if (task.TaskType == QSEnumTaskType.CIRCULATE)
            { 
                IJobDetail job = JobBuilder.Create<CoreTask>()
                    .WithIdentity("Task-" + task.TaskUUID, "TaskGroup")
                    .UsingJobData("TaskUUID", task.TaskUUID)
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("Trigger-" + task.TaskUUID, "TriggerGroup")
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
        /// 获得任务ITask
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ITask this[string uuid]
        {
            get
            {
                ITask target = null;
                if (taskUUIDMap.TryGetValue(uuid, out target))
                {
                    return target;
                }
                return null;
            }
        }


        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            logger.StatusStart(this.PROGRAME);
            _scheduler.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            logger.StatusStop(this.PROGRAME);
            _scheduler.Clear();
        }

        public override void Dispose()
        {
            logger.StatusDestory(this.PROGRAME);
            base.Dispose();
            //_timer.Elapsed -= new System.Timers.ElapsedEventHandler(TimeEvent);
            //_timer = null;
        }


        /// <summary>
        /// 以特定扫描频率100ms定时运行时间间隔执行的任务
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        //void TimeEvent(object source, System.Timers.ElapsedEventArgs e)
        //{
        //    if (!TLCtxHelper.IsReady) return;
        //    foreach (ITask t in taskUUIDMap.Values.Where(task => task.TaskType == QSEnumTaskType.CIRCULATE))
        //    {
        //        //Util.Debug("sec:" + DateTime.Now.Second.ToString() + " millisec:" + DateTime.Now.Millisecond.ToString(), QSEnumDebugLevel.INFO);
        //        t.DoTask(e.SignalTime);
        //    }
        //}

    }

}