using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Quartz;
using Common.Logging;

namespace TradingLib.Core
{
    public class JobListener : IJobListener
    {
        string _name = "JobListener";
        public string Name { get { return _name; } }

        static ILog logger = LogManager.GetLogger("JobListener");
        /// <summary>
        /// 任务即将运行
        /// </summary>
        /// <param name="context"></param>
        public void JobToBeExecuted(IJobExecutionContext context)
        {
            //Util.Debug("JobToBeExecuted");
        }

        /// <summary>
        /// 任务在trigger vote中被拒绝运行
        /// </summary>
        /// <param name="context"></param>
        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            //Util.Debug("JobExecutionVetoed");
        }

        /// <summary>
        /// 任务运行完成
        /// 如果运行过程中产生Exception,则异常会被Quartz捕获并且封装成jobException
        /// 因此在任务中我们需要按实际运行情况捕获异常，通过异常来判断任务是否被正常执行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jobException"></param>
        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            //Util.Debug("JobWasExecuted");
            //Util.Debug("firetime utc:" + context.FireTimeUtc.ToString() + " taskuuid:" + context.JobDetail.JobDataMap.GetString("TaskUUID") + " nextFiretime:" + context.NextFireTimeUtc.ToString());

            try
            {
                //获得该job对应的Task对象
                string taskuuid = context.JobDetail.JobDataMap.GetString("TaskUUID");
                ITask task = TLCtxHelper.ModuleTaskCentre[taskuuid];

                if (task.TaskType == QSEnumTaskType.SPECIALTIME)
                {
                    //通过jobException来判断任务运行是否有异常发生，如果产生异常则触发对应的系统时间，在相关扩展模块中会将异常记录到数据库，同时任务执行状态标注为False
                    if (jobException != null)
                    {
                        TLCtxHelper.EventSystem.FireTaskErrorEvent(this, TaskEventArgs.TaskFail(task, jobException));
                    }
                    else
                    {
                        TLCtxHelper.EventSystem.FireSpecialTimeEvent(this, TaskEventArgs.TaskSuccess(task));
                    }
                }
                if (task.TaskType == QSEnumTaskType.CIRCULATE)
                {
                    //Util.Info("Task:" + task.TaskName + " InterVal:" + task.TaskInterval);
                }
            }
            catch (Exception ex)
            {
                logger.Error("JobExecuted Handle  Error:" + ex.ToString());
            }
        }
    }
    public class TrgierListener : ITriggerListener
    {
        public TrgierListener()
        {

        }
        static ILog logger = LogManager.GetLogger("TriggerListener");
        string _name = "TriggerListener";
        public string Name { get { return _name; } }

        /// <summary>
        /// 触发器触发时调用
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        public void TriggerFired(ITrigger trigger, IJobExecutionContext context)
        {
            //Util.Debug("triggerFired");
        }

        /// <summary>
        /// 判断是否需要运行任务，返回false则会运行 返回true则对应的任务不运行
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool VetoJobExecution(ITrigger trigger, IJobExecutionContext context)
        {
            //Util.Debug("VetoJobExecution");
            return false;
        }

        /// <summary>
        /// 触发缺失事件
        /// </summary>
        /// <param name="trigger"></param>
        public void TriggerMisfired(ITrigger trigger)
        {
            try
            {
                string taskuuid = trigger.JobDataMap.GetString("TaskUUID");
                ITask task = TLCtxHelper.ModuleTaskCentre[taskuuid];
                TLCtxHelper.EventSystem.FireTaskErrorEvent(this, TaskEventArgs.TaskFail(task, new Exception("Task Misfired")));
            }
            catch (Exception ex)
            {
                logger.Error("TriggerMisfired Handle Error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 任务执行成功事件
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="triggerInstructionCode"></param>
        public void TriggerComplete(ITrigger trigger, IJobExecutionContext context, Quartz.SchedulerInstruction triggerInstructionCode)
        {
            //Util.Debug("TriggerComplete");
            
        }
    }
}