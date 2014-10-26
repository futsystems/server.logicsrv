using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Diagnostics;

namespace TradingLib.Common
{
    /// <summary>
    /// 服务端服务对象的基类,实现日志输出,邮件通知,以及服务端对象名称标识
    /// </summary>
    public class BaseSrvObject:IDisposable
    {
        /// <summary>
        /// 服务端对象名称
        /// </summary>
        protected string PROGRAME = "BaseSrvObject";

        public string Name { get { return PROGRAME; } }
        public string UUID { get { return _uuid; } }
        protected string _uuid; 
        public BaseSrvObject(string programe="BaseSrvObject")
        {
            try
            {
                PROGRAME = programe;
                _uuid = System.Guid.NewGuid().ToString();
                TLCtxHelper.Ctx.Register(this);
            }
            catch (Exception ex)
            {
                Util.Debug("BaseSrvObject init error:" + ex.ToString());
            }
        }

        bool disposed = false;

        public virtual void Dispose()
        {
            try
            {
                TLCtxHelper.Ctx.Unregister(this);
            }
            finally
            {
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        ~BaseSrvObject()
        {
            Console.WriteLine("descruct called");
            Dispose();
            
            
        }
        #region 邮件通知部分
        /// <summary>
        /// 发送邮件事件
        /// </summary>
        public event EmailDel SendEmailEvent;
        /// <summary>
        /// 邮件通知
        /// </summary>
        /// <param name="subject">邮件标题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="receivers">收件人列表</param>
        protected void Notify(string subject, string body, string[] receivers)
        {
            if (SendEmailEvent != null)
                SendEmailEvent(new Email(subject, body, receivers));
        }
        protected void Notify(string subject, string body, string receiver)
        {
            Notify(subject, body, new string[] { receiver });
        }
        protected void Notify(string subject, string body)
        { 
            Notify(subject,body,new string[]{});
        }
        #endregion


        #region 日志输出部分
        /// <summary>
        /// 日志输出事件
        /// </summary>
        //public event DebugDelegate SendDebugEvent;

        /// <summary>
        /// 对外发送日志事件
        /// </summary>
        public event ILogItemDel SendLogItemEvent;

        /// <summary>
        /// 日志输出事件
        /// </summary>
        //public event LogDelegate SendLogEvent;

        bool _debugEnable = true;
        /// <summary>
        /// 是否输出日志
        /// 如果禁用日志 则所有日志将不对外发送
        /// </summary>
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }

        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.INFO;
        /// <summary>
        /// 日志输出级别
        /// </summary>
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// 同时对外输出日志事件,用于被日志模块采集日志或分发
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
		//[Conditional("DEBUG")]
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            if (_debugEnable && (int)level <= (int)_debuglevel && SendLogItemEvent != null)
            {
                ILogItem item = new LogItem(msg, level, this.PROGRAME);
                SendLogItemEvent(item);
            }
        }

        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="msg"></param>
        //protected void msgdebug(string msg)
        //{
        //    if (SendDebugEvent != null)
        //        SendDebugEvent(msg);
        //}

        //private bool _noverb = true;
        ///// <summary>
        ///// 是否显示内部调试信息
        ///// </summary>
        //public bool VerboseDebugging
        //{
        //    get { return !_noverb; }
        //    set
        //    {
        //        _noverb = !value;
        //    }
        //}
        /// <summary>
        /// 内部输出调试日志
        /// </summary>
        /// <param name="msg"></param>
        //protected void v(string msg)
        //{
        //    if (!_noverb)
        //    {
        //        msgdebug(string.Format("[Verb] {0}:{1}",PROGRAME,msg));
        //        //2.调用日志事件对外触发日志事件
        //        if (SendLogEvent != null)
        //            SendLogEvent(PROGRAME, msg, QSEnumDebugLevel.VERB);
        //    }
        //}
        #endregion

    }
}
