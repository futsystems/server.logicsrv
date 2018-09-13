using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Common.Logging.Simple;

namespace TradingSrv.Win
{

    public class ControlLogFactoryAdapter : AbstractSimpleLoggerFactoryAdapter
    {
        public static event Action<string> SendDebugEvent;

        public static void Debug(string msg)
        {
            if (SendDebugEvent != null)
            {
                SendDebugEvent(msg);
            }
        }
        public ControlLogFactoryAdapter()
            : base(null)
        {
        }

        public ControlLogFactoryAdapter(LogLevel level, bool showDateTime, bool showLogName, bool showLevel, string dateTimeFormat)
            : base(level, showDateTime, showLogName, showLevel, dateTimeFormat)
        {

        }
        protected override ILog CreateLogger(string name, LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
        {
            return new GlobalLogger(name, level, showLevel, showDateTime, showLogName, dateTimeFormat);
        }
    }


    public class GlobalLogger : AbstractSimpleLogger
    {
        public GlobalLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
            : base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
        {

        }
        protected override void WriteInternal(LogLevel level, object message, Exception e)
        {
            if (level >= LogLevel.Info)
            {
                StringBuilder stringBuilder = new StringBuilder();
                this.FormatOutput(stringBuilder, level, message, e);
                ControlLogFactoryAdapter.Debug(stringBuilder.ToString());
            }
        }
    }


}
