using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 任务接口
    /// 在TLCtx中托管任务调度,任务列表使用ITask
    /// 接口
    /// 接口就是预留的规范，在使用时还没有具体的实现，在后期其他组件中才有实现该接口的相关对象定义
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// 所属对象UUID
        /// </summary>
        string UUID { get; }

        /// <summary>
        /// 任务UUID
        /// </summary>
        string TaskUUID { get; }

        /// <summary>
        /// 任务名称
        /// </summary>
        string TaskName { get; }

        /// <summary>
        /// 任务类别 定时/循环
        /// </summary>
        QSEnumTaskType TaskType { get; }

        /// <summary>
        /// Cron表达式
        /// </summary>
        string CronExpression { get; }

        /// <summary>
        /// 任务间隔
        /// </summary>
        TimeSpan TaskInterval { get; set; }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="triggertime"></param>
        void DoTask();
    }
}
