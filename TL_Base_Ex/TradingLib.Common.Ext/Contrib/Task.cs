using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

using Quartz;
using Quartz.Impl;

namespace TradingLib.Common
{
    /// <summary>
    /// 利用全局Task UUID对应的可运行对象
    /// 然后任务模块封装后 通过UUID来获得对应的委托 从而运行。
    /// 利用UUID进行参数设置
    /// </summary>
    public class CoreTask:IJob 
    {
        public void Execute(IJobExecutionContext context)
        {
            //1.获得任务UUID
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            string taskuuid = dataMap.GetString("TaskUUID");

            //2.获得对应的任务
            ITask task = TLCtxHelper.ModuleTaskCentre[taskuuid];

            task.DoTask(DateTime.Now);
        }

    }
}
