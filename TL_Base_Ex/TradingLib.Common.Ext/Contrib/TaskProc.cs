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
        /// <summary>
        /// ����UUID
        /// </summary>
        public string UUID { get { return _uuid; } }


        string _taskName = "Task";
        /// <summary>
        /// ��������
        /// </summary>
        public string TaskName { get { return _taskName; } set { _taskName = value; } }


        QSEnumTaskType _taskType = QSEnumTaskType.SPECIALTIME;
        /// <summary>
        /// �������
        /// </summary>
        public QSEnumTaskType TaskType { get { return _taskType; } set { _taskType = value; } }


        TimeSpan _taskInterval = new TimeSpan(0, 0, 0);
        /// <summary>
        /// ����ִ�м��
        /// </summary>
        public TimeSpan TaskInterval { get { return _taskInterval; } set { _taskInterval = value; } }
        
        DateTime _lastTime = new DateTime(1970, 01, 01);
        /// <summary>
        /// �ϴ�ִ������ʱ��
        /// </summary>
        public DateTime LastTime { get { return _lastTime; } set { _lastTime = value; } }

        public int TaskHour { get { return this.Hour; } }
        public int TaskMinute { get { return this.Minute; } }
        public int TaskSecend { get { return this.Secend; } }

        public int Hour=0;
        public int Minute=0;
        public int Secend=0;

        VoidDelegate taskfunc;//����ص�����


        /// <summary>
        /// ִ���ض�ʱ�������
        /// </summary>
        public void DoTask()
        {
            if (taskfunc != null)
            {
                taskfunc();
            } 
        }

        TaskProcWrapper _taskproc=null;
        TaskProcWrapper GetTaskProcWrapper()
        {
            if (_taskproc == null)
            {
                _taskproc = new TaskProcWrapper(this);
            }
            return _taskproc;
        }

        /// <summary>
        /// ִ�ж�ʱ����
        /// ����ϴ�ִ��ʱ���뵱ǰʱ��ļ��,��������趨��ʱ����,��ִ������
        /// </summary>
        /// <param name="signalTime">��������ʱ��</param>
        public void CheckTask(DateTime signalTime)
        {
            if (_taskType == QSEnumTaskType.CIRCULATE)
            {
                if (signalTime.Subtract(_lastTime) >= _taskInterval)
                {
                    GetTaskProcWrapper().DoTask();
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
                        GetTaskProcWrapper().DoTask();
                    }
                }

                if (this.Hour < 0 && this.Minute >= 0 && this.Secend >= 0)
                {
                    if (intMinute == this.Minute && intSecond == this.Secend)
                    {
                        GetTaskProcWrapper().DoTask();
                    }
                }

                if (this.Hour >= 0 && this.Minute >= 0 && this.Secend >= 0)
                {
                    if (intHour == this.Hour && intMinute == this.Minute && intSecond == this.Secend)
                    {
                        GetTaskProcWrapper().DoTask();
                    }
                }
                return;
            }
                   
        }

        /// <summary>
        /// �ض�ʱ��ִ�е�����
        /// ����,����,���붨ʱ����ĳ������
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="secend"></param>
        /// <param name="func"></param>
        public TaskProc(string uuid,string taskName,int hour, int minute, int secend, VoidDelegate func)
        {
            _uuid = uuid;
            _taskName = taskName;
            _taskType = QSEnumTaskType.SPECIALTIME;//�ض�ʱ��ִ�е�����
            Hour = hour;
            Minute = minute;
            Secend = secend;
            taskfunc = func;
        }

        public TaskProc(string uuid,string taskName,TimeSpan interval, VoidDelegate func)
        {
            _uuid = uuid;
            _taskName = taskName;
            _taskType = QSEnumTaskType.CIRCULATE;//ѭ��ִ������
            _taskInterval = interval;
            taskfunc = func;
        }

    }

    /// <summary>
    /// ��TaskProc����try catch�ṹ������,����¼�����������
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

                //������ʱ����
                if (_proc.TaskType == QSEnumTaskType.SPECIALTIME)
                {
                    TLCtxHelper.EventSystem.FireSpecialTimeEvent(this, TaskEventArgs.TaskSuccess(_proc));
                }
            }
            catch (Exception ex)
            {
                Util.Debug("Task Error:" + ex.ToString());
                //��������ִ���쳣
                TLCtxHelper.EventSystem.FireTaskErrorEvent(this, TaskEventArgs.TaskFail(_proc, ex));
            }
        }
    }
    
}
