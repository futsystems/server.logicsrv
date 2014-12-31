using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface ITask
    {
        /// <summary>
        /// 所属对象UUID
        /// </summary>
        string UUID { get; }

        /// <summary>
        /// 任务名称
        /// </summary>
        string TaskName { get; }

        /// <summary>
        /// 任务类别 定时/循环
        /// </summary>
        QSEnumTaskType TaskType { get; }

        /// <summary>
        /// 任务间隔
        /// </summary>
        TimeSpan TaskInterval { get; set; }

        /// <summary>
        /// 定时任务小时
        /// </summary>
        int TaskHour { get; }

        /// <summary>
        /// 定时任务分钟
        /// </summary>
        int TaskMinute { get; }

        /// <summary>
        /// 定时任务秒
        /// </summary>
        int TaskSecend { get; }


        void CheckTask(DateTime signaltime);
    }
}
