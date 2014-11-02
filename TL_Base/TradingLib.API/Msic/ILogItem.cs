using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 日志内容接口
    /// </summary>
    public interface ILogItem
    {
        
        /// <summary>
        /// 日志产生时间
        /// </summary>
        int Time { get; set; }

        /// <summary>
        /// 日志产生时的毫秒时间戳
        /// </summary>
        int Millisecond { get; set; }

        /// <summary>
        /// 日志发送者 说明该日志是从哪个功能模块发送
        /// </summary>
        string Programe { get; set; }

        /// <summary>
        /// 日志级别 通过日志级别 我们可以进行日志过滤
        /// </summary>
        QSEnumDebugLevel Level { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// 生成用于显示的字符串
        /// </summary>
        string ToString();
    }
}
