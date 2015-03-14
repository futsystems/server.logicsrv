using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 日志项目
    /// </summary>
    public class LogItem:ILogItem
    {
        public LogItem()
        {
            this.Message = string.Empty;
            this.Level = QSEnumDebugLevel.INFO;
            this.Programe = string.Empty;
            this.Time = 0;
            this.Millisecond = 0;
        }
        public LogItem(string message, QSEnumDebugLevel level, string programe)
        {
            this.Message = message;
            this.Level = level;
            this.Programe = programe;
            this.Time = Util.ToTLTime();
            this.Millisecond = DateTime.Now.Millisecond;
        }

        /// <summary>
        /// 日志产生时间
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// 日志产生微秒
        /// </summary>
        public int Millisecond { get; set; }

        /// <summary>
        /// 日志发送者 说明该日志是从哪个功能模块发送
        /// </summary>
        public string Programe { get; set; }

        /// <summary>
        /// 日志级别 通过日志级别 我们可以进行日志过滤
        /// </summary>
        public QSEnumDebugLevel Level { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message { get; set; }


        string ILogItem.ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Time.ToString());
            sb.Append(" ");
            sb.Append(this.Millisecond.ToString().PadLeft(3,'0'));
            sb.Append(" ");
            sb.Append("[");
            sb.Append(this.Level.ToString());
            sb.Append("] ");
            sb.Append(this.Programe);
            sb.Append(":");
            sb.Append(this.Message);

            return sb.ToString();
        }
        public static string Serialize(ILogItem item)
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(item.Time.ToString());
            sb.Append(d);
            sb.Append(item.Millisecond.ToString());
            sb.Append(d);
            sb.Append(item.Programe);
            sb.Append(d);
            sb.Append(item.Level.ToString());
            sb.Append(d);
            sb.Append(item.Message);
            return sb.ToString();
        }

        public static ILogItem Deserialize(string content)
        {
            string[] rec = content.Split(new char[] { ',' },5);
            ILogItem item = new LogItem();

            item.Time = int.Parse(rec[0]);
            item.Millisecond = int.Parse(rec[1]);
            item.Programe = rec[2];
            item.Level = (QSEnumDebugLevel)Enum.Parse(typeof(QSEnumDebugLevel), rec[3]);
            item.Message = rec[4];

            return item;
        }
    }
}
