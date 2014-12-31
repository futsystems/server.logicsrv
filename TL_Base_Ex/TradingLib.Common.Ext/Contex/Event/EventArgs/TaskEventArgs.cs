using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 任务事件参数
    /// </summary>
    public class TaskEventArgs : EventArgs
    {
        /// <summary>
        /// 任务对象
        /// </summary>
        public ITask Task { get; set; }

        /// <summary>
        /// 任务是否正常执行
        /// 如果没有产生运行异常则 执行成功
        /// </summary>
        public bool IsSuccess { get { return this.InnerException == null; } }

        /// <summary>
        /// 执行任务时 内部异常
        /// </summary>
        public Exception InnerException { get; set; }


        public static TaskEventArgs TaskSuccess(ITask task)
        {
            TaskEventArgs arg = new TaskEventArgs();
            arg.Task = task;

            arg.InnerException = null;
            return arg;
        }

        public static TaskEventArgs TaskFail(ITask task, Exception ex)
        {
            TaskEventArgs arg = new TaskEventArgs();
            arg.Task = task;

            arg.InnerException = ex;
            return arg;
        }

    }

}
