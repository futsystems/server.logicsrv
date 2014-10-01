using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=true)]
    public class TaskAttr:TLAttribute
    {
        public QSEnumTaskType TaskType { get { return _type; } }

        public string Name { get { return _name; } }

        int _hour;
        int _miniute;
        int _secend;
        int _intervalsecends;
        string _name;
        string _description;
        QSEnumTaskType _type;
        public int IntervalSecends { get {
            if (_type == QSEnumTaskType.CIRCULATE)
                return _intervalsecends;
            else
                return 0;
        } }

        public int Hour { get {

            if (_type == QSEnumTaskType.SPECIALTIME)
                return _hour;
            else
                return 0;
        } }

        public int Minute
        {
            get
            {
                if (_type == QSEnumTaskType.SPECIALTIME)
                    return _miniute;
                else
                    return 0;
            }
        }

        public int Secend { get {

            if (_type == QSEnumTaskType.SPECIALTIME)
                return _secend;
            else
                return 0;
        } }

        /// <summary>
        /// 定时执行几点几分几秒执行该任务
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="hour">几点</param>
        /// <param name="minute">几分</param>
        /// <param name="secend">几秒</param>
        /// <param name="description">任务描述</param>
        public TaskAttr(string name,int hour, int minute, int secend,string description="任务描述")
        {
            _name = name;
            _hour = hour;
            _miniute = minute;
            _secend = secend;
            _type = QSEnumTaskType.SPECIALTIME;
            _description = description;
        }

        /// <summary>
        /// 每隔多少时间执行该任务
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="secends">任务间隔秒数</param>
        /// <param name="description">任务描述</param>
        public TaskAttr(string name,int secends,string description="任务描述")
        {
            _name = name;
            _intervalsecends = secends;
            _type = QSEnumTaskType.CIRCULATE;
            _description = description;
        }
    }
}
