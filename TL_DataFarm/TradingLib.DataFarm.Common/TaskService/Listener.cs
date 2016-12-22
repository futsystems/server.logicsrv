using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Quartz;

namespace TradingLib.DataFarm.Common
{
    public class JobListener : IJobListener
    {
        string _name = "JobListener";
        public string Name { get { return _name; } }

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
                
            }
            catch (Exception ex)
            {
                Util.Error("JobExecuted Handle  Error:" + ex.ToString(), "JobListener");
            }
        }
    }


    public class TrgierListener : ITriggerListener
    {
        public TrgierListener()
        {

        }

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
             
            }
            catch (Exception ex)
            {
                
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
           

        }
    }
}
