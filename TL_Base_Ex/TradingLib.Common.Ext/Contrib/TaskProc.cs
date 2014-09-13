using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class TaskProc : ITask
    {
        string _uuid =string.Empty;
        public string UUID { get { return _uuid; } }
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


        TimeSpan _taskInterval = new TimeSpan(0, 0, 0);
        /// <summary>
        /// 任务执行间隔
        /// </summary>
        public TimeSpan TaskInterval { get { return _taskInterval; } set { _taskInterval = value; } }
        
        DateTime _lastTime = new DateTime(1970, 01, 01);
        /// <summary>
        /// 上次执行任务时间
        /// </summary>
        public DateTime LastTime { get { return _lastTime; } set { _lastTime = value; } }

        public int Hour=0;
        public int Minute=0;
        public int Secend=0;

        VoidDelegate taskfunc;//任务回调函数


        /// <summary>
        /// 执行特定时间的任务
        /// </summary>
        public void DoTask()
        {
            if (taskfunc != null)
            {
                //LibUtil.Debug("Task[" + _taskName + "]" + " Start running");
                //TaskCentre.Logger.GotDebug("Task["+_taskName+"]"+" Start running");
                taskfunc();
            } 
        }

        /// <summary>
        /// 执行定时任务
        /// 检查上次执行时间与当前时间的间隔,如果大于设定的时间间隔,则执行任务
        /// </summary>
        /// <param name="signalTime">触发检查的时间</param>
        public void CheckTask(DateTime signalTime)
        {
            //LibUtil.Debug("check task");
            if (_taskType == QSEnumTaskType.CIRCULATE)
            {
                if (signalTime.Subtract(_lastTime) >= _taskInterval)
                {
                    new TaskProcWrapper(this).DoTask();
                    _lastTime = DateTime.Now;
                }
                return;
            }     
            if (_taskType == QSEnumTaskType.SPECIALTIME)
            {
                int intHour = signalTime.Hour;
                int intMinute = signalTime.Minute;
                int intSecond = signalTime.Second;
                if (this.Hour < 0 && this.Minute < 0 && this.Secend >= 0)
                {
                    if (intSecond == this.Secend)
                    {
                        new TaskProcWrapper(this).DoTask();
                    }
                }

                if (this.Hour < 0 && this.Minute >= 0 && this.Secend >= 0)
                {
                    if (intMinute == this.Minute && intSecond == this.Secend)
                    {
                        new TaskProcWrapper(this).DoTask();
                    }
                }

                if (this.Hour >= 0 && this.Minute >= 0 && this.Secend >= 0)
                {
                    if (intHour == this.Hour && intMinute == this.Minute && intSecond == this.Secend)
                    {
                        new TaskProcWrapper(this).DoTask();
                    }
                }
                return;
            }
                   
        }

        public string TaskMemo
        {
            get
            {
                return _taskName.PadRight(25,' ')+(_taskType == QSEnumTaskType.CIRCULATE ?"循环-":"定时-") + GetTaskTimeStr();
            }
        }

        public string TypeName { get { return (_taskType == QSEnumTaskType.CIRCULATE ? "循环" : "定时"); } }

        public string TimeStr { get { return GetTaskTimeStr(); } }

        string GetTaskTimeStr()
        {
            if (_taskType == QSEnumTaskType.CIRCULATE)
            {
                return (_taskInterval.TotalSeconds.ToString() + "秒");
            }
            else
            {
                return this.Hour.ToString() + ":" + this.Minute.ToString() + ":" + this.Secend.ToString();
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
            _uuid = uuid;
            _taskName = taskName;
            _taskType = QSEnumTaskType.SPECIALTIME;//特定时间执行的任务
            Hour = hour;
            Minute = minute;
            Secend = secend;
            taskfunc = func;
        }

        public TaskProc(string uuid,string taskName,TimeSpan interval, VoidDelegate func)
        {
            _uuid = uuid;
            _taskName = taskName;
            _taskType = QSEnumTaskType.CIRCULATE;//循环执行任务
            _taskInterval = interval;
            taskfunc = func;
        }

    }

    /// <summary>
    /// 将TaskProc置于try catch结构中运行,并记录错误运行输出
    /// </summary>
    internal class TaskProcWrapper
    {
        TaskProc _proc;
        public TaskProcWrapper(TaskProc proc)
        {
            _proc = proc;
        }

        public void DoTask()
        {
            try
            {
                _proc.DoTask();
            }
            catch (Exception ex)
            {
                //在执行定时任务的时候发生错误,记录该错误到taskcentre_error日志
                //TaskCentre.Logger.GotDebug("["+_proc.TaskName +"]"+" error:"+ex.ToString());
            }
        }
    }
    
}
