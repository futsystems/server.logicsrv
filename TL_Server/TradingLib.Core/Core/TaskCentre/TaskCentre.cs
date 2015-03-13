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


namespace TradingLib.Core
{

    /// <summary>
    /// 系统内部实现了一个Task模型，用于定时的执行某些任务
    /// 任务分2种类型
    /// 1.几点几分几秒执行的任务
    /// 2.每隔多少时间执行的任务
    /// </summary>
    public partial class TaskCentre : BaseSrvObject, ICore, ITaskCentre
    {
        const string CoreName = "TaskCentre";
        public static Log Logger = new Log("TaskCentre_Error", true, true, Util.ProgramData(CoreName), true);//日志组件

        System.Timers.Timer _timer = null;
        System.Timers.Timer _timerSpecial = null;
        public string CoreId { get { return this.PROGRAME; } }

        IScheduler _scheduler = null;
        public TaskCentre():base(TaskCentre.CoreName)
        {

            ISchedulerFactory schedFact = new StdSchedulerFactory();
            _scheduler = schedFact.GetScheduler();
            
        }

        ConcurrentDictionary<string, ITask> taskUUIDMap = new ConcurrentDictionary<string, ITask>();

        /// <summary>
        /// 注册一个Task
        /// </summary>
        /// <param name="task"></param>
        public void RegisterTask(ITask task)
        {
            taskUUIDMap.TryAdd(task.TaskUUID, task);

            if (task.TaskType == QSEnumTaskType.CRON)
            {
                int i = 1;
            }
            if (task.TaskType != QSEnumTaskType.CIRCULATE)
            {
                IJobDetail job = JobBuilder.Create<CoreTask>()
                    .WithIdentity("Task-" + task.TaskUUID, "TaskGroup")
                    .UsingJobData("TaskUUID", task.TaskUUID)
                    .Build();

                string cronexpress = string.Format("{0} {1} {2} * * ?",task.TaskSecend,task.TaskMinute,task.TaskHour);
                
                cronexpress = string.IsNullOrEmpty(task.CronExpression) ? cronexpress : task.CronExpression;

                debug(" task xxxxxxxxxxx cron:" + cronexpress, QSEnumDebugLevel.ERROR);

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("Trigger-" + task.TaskUUID, "TriggerGroup")
                    .StartNow()
                    //0 0/5 * * * ?
                    .WithCronSchedule(cronexpress)
                    //   .WithSimpleSchedule()
                    //    //.WithSimpleSchedule(x => x
                    //    //    .WithIntervalInSeconds(2)
                    //    //    .RepeatForever())
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
            Util.StartStatus(this.PROGRAME);
            if (_timer == null)
            {
                _timer = new System.Timers.Timer();
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(TimeEvent);
                _timer.Interval = Const.TASKFREQ;
                _timer.Enabled = true;
                _timer.Start();
            }
            if (_timerSpecial == null)
            {
                _timerSpecial = new System.Timers.Timer();
                _timerSpecial.Elapsed += new System.Timers.ElapsedEventHandler(TimeEventSpecial);
                _timerSpecial.Interval = 1000;
                _timerSpecial.Enabled = true;
                _timerSpecial.Start();
            }
            _scheduler.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
            if (_timer != null)
            {
                _timer.Stop();
            }
            if (_timerSpecial != null)
            {
                _timerSpecial.Stop();
            }
        }

        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();
            _timer.Elapsed -= new System.Timers.ElapsedEventHandler(TimeEvent);
            _timer = null;
            _timerSpecial.Elapsed -= new System.Timers.ElapsedEventHandler(TimeEventSpecial);
            _timerSpecial = null;

        }
        /// <summary>
        /// 以秒为频率定时检查特定时间执行的任务
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void TimeEventSpecial(object source, System.Timers.ElapsedEventArgs e)
        {
            //Util.Debug("-----------------------------------------");
            if (!TLCtxHelper.IsReady) return;
            foreach (ITask t in TLCtxHelper.Ctx.TaskList.Where(task => task.TaskType == QSEnumTaskType.SPECIALTIME))
            {
                //Util.Debug("sec:" + DateTime.Now.Second.ToString() + " millisec:" + DateTime.Now.Millisecond.ToString(), QSEnumDebugLevel.INFO);
                //Util.Debug("Task:" + t.TaskName + " Memo" + t.GetTaskMemo());
                //t.CheckTask(e.SignalTime);
            }
        }

        /// <summary>
        /// 以特定扫描频率100ms定时运行时间间隔执行的任务
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void TimeEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            if (!TLCtxHelper.IsReady) return;
            foreach (ITask t in  TLCtxHelper.Ctx.TaskList.Where(task=>task.TaskType == QSEnumTaskType.CIRCULATE))
            {
                //Util.Debug("sec:" + DateTime.Now.Second.ToString() + " millisec:" + DateTime.Now.Millisecond.ToString(), QSEnumDebugLevel.INFO);
                t.CheckTask(e.SignalTime);
            }
        }

    }
    
}
