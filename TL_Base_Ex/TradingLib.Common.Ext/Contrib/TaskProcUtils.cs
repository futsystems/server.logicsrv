using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.API
{
    public static class ITaskUtils
    {
        static string GetSpecialTime(this ITask task)
        {
            return string.Format("{0}:{1}:{2}", task.TaskHour, task.TaskMinute, task.TaskSecend);
        }

        static string GetCirulateTime(this ITask task)
        {
            return string.Format("每{0}秒", task.TaskInterval.TotalSeconds);
        }

        public static string GetTimeStr(this ITask task)
        { 
            switch (task.TaskType)
            { 
                case QSEnumTaskType.CIRCULATE:
                    return string.Format("{0} {1}",Util.GetEnumDescription(task.TaskType),task.GetCirulateTime());
                case QSEnumTaskType.SPECIALTIME:
                    return string.Format("{0} {1}", Util.GetEnumDescription(task.TaskType), task.GetSpecialTime());
                default:
                    return "未知";
            }
        }

        /// <summary>
        /// 获得任务描述
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static string GetTaskMemo(this ITask task,bool pad=true)
        {
            if (pad)
            {
                return Util.padLeftEx(task.TaskName, 40) + task.GetTimeStr();
            }
            return task.TaskName + " " + task.GetTimeStr(); 
        }

    }
}
