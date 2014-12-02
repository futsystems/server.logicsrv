using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 系统内部实现了一个Task模型，用于定时的执行某些任务
    /// 任务分2种类型
    /// 1.几点几分几秒执行的任务
    /// 2.每隔多少时间执行的任务
    /// </summary>
    public partial class TaskCentre:BaseSrvObject,ICore
    {
        const string CoreName = "TaskCentre";
        public static Log Logger = new Log("TaskCentre_Error", true, true, Util.ProgramData(CoreName), true);//日志组件

        System.Timers.Timer _timer = null;

        public string CoreId { get { return this.PROGRAME; } }
        public TaskCentre():base(TaskCentre.CoreName)
        { 
            
        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            if (_timer == null)
            {
                _timer = new System.Timers.Timer();
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(TimeEvent);
                _timer.Interval = 1000;
                _timer.Enabled = true;
                _timer.Start();
                
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();
            _timer.Elapsed -= new System.Timers.ElapsedEventHandler(TimeEvent);
            _timer = null;
        }

        void TimeEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            foreach (ITask t in  TLCtxHelper.Ctx.TaskList)
            {
                t.CheckTask(e.SignalTime);
            }
        }

    }
    
}
