using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class TaskProc : ITask
    {

        /// <summary>
        /// 通过BaseSrvObject和TaskAttr封装出来的info创建ITask
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static ITask CreateTask(BaseSrvObject obj, TaskInfo info)
        {
            
            switch (info.Attr.TaskType)
            {
                case QSEnumTaskType.CIRCULATE:
                    return new TaskProc(obj.UUID, info.Attr.Name, new TimeSpan(0, 0, 0, info.Attr.IntervalSecends, info.Attr.IntervalMilliSecends), delegate() { info.MethodInfo.Invoke(obj, null); });
                case QSEnumTaskType.SPECIALTIME:
                    return new TaskProc(obj.UUID, info.Attr.Name, info.Attr.CronExpression, delegate() { info.MethodInfo.Invoke(obj, null); });
                default:
                    return null;
            }
        }
        string _uuid =string.Empty;
        /// <summary>
        /// 对象UUID
        /// </summary>
        public string UUID { get { return _uuid; } }


        string _taskUUID = string.Empty;
        /// <summary>
        /// 每个任务有一个全局唯一的TaskUUID 用于通过TaskUUID来进行任务的查找
        /// </summary>
        public string TaskUUID { get { return _taskUUID; } }

        string _taskName = "Task";
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get { return _taskName; } set { _taskName = value; } }


        QSEnumTaskType _taskType = QSEnumTaskType.SPECIALTIME;
        /// <summary>
        /// 任务类别
        /// </summary>
        public QSEnumTaskType TaskType { get { return _taskType; } set { _taskType = value; } }


        TimeSpan _taskInterval = new TimeSpan(0, 0, 0, 0);
        /// <summary>
        /// 任务执行间隔
        /// </summary>
        public TimeSpan TaskInterval { get { return _taskInterval; } set { _taskInterval = value; } }
        

        DateTime _lastTime = new DateTime(1970, 01, 01);
        /// <summary>
        /// 上次执行任务时间
        /// </summary>
        public DateTime LastTime { get { return _lastTime; } set { _lastTime = value; } }

        /// <summary>
        /// 回调任务
        /// </summary>
        VoidDelegate taskfunc;


        string _cronstr = string.Empty;
        public string CronExpression { get { return _cronstr; } }


        /// <summary>
        /// 执行特定时间的任务
        /// </summary>
        public void DoTask(DateTime triggertime)
        {
            if (_taskType == QSEnumTaskType.CIRCULATE)
            {
                //Util.Debug(this.TaskName + string.Format("sig sec:{0} millisec:{1} interval sec:{2} millisec:{3}" , signalTime.Second,signalTime.Millisecond,_taskInterval.Seconds,_taskInterval.Milliseconds),QSEnumDebugLevel.INFO);
                if (triggertime.Subtract(_lastTime) >= _taskInterval)
                {
                    try
                    {
                        if (taskfunc != null)
                        {
                            taskfunc();
                        }
                        _lastTime = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        Util.Debug("Task Error:" + ex.ToString());
                       
                    }
                }
                return;
            }
            if (_taskType == QSEnumTaskType.SPECIALTIME)
            {
                if (taskfunc != null)
                {
                    taskfunc();
                }
            }
        }


        /// <summary>
        /// 特定时间执行的任务
        /// 几点,几分,几秒定时运行某个任务
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="secend"></param>
        /// <param name="func"></param>
        public TaskProc(string uuid,string taskName,int hour, int minute, int secend, VoidDelegate func)
        {
            _taskUUID = System.Guid.NewGuid().ToString();
            _uuid = uuid;
            _taskName = taskName;
            _taskType = QSEnumTaskType.SPECIALTIME;//特定时间执行的任务
            _cronstr = string.Format("{0} {1} {2} * * ?", secend, minute, hour);
            taskfunc = func;
        }

        public TaskProc(string uuid, string taskName, string cronstr, VoidDelegate func)
        {
            _taskUUID = System.Guid.NewGuid().ToString();
            _uuid = uuid;
            _taskName = taskName;
            _taskType = QSEnumTaskType.SPECIALTIME;
            taskfunc = func;
            _cronstr = cronstr;
        }

        /// <summary>
        /// 循环执行任务
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="taskName"></param>
        /// <param name="interval"></param>
        /// <param name="func"></param>
        public TaskProc(string uuid,string taskName,TimeSpan interval, VoidDelegate func)
        {
            _taskUUID = System.Guid.NewGuid().ToString();
            _uuid = uuid;
            _taskName = taskName;
            _taskType = QSEnumTaskType.CIRCULATE;//循环执行任务
            _taskInterval = interval;
            taskfunc = func;

            
        }
    }
    
}
