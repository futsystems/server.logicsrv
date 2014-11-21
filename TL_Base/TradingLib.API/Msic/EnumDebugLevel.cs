using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{
    /// <summary>
    /// 枚举日志输出级别
    /// </summary>
    public enum QSEnumDebugLevel
    {
        [Description("Verbose信息")]
        VERB = 10,//verbose输出
        [Description("调试信息")]
        DEBUG = 9,//调试输出 Logs to help debug errors in the application
        [Description("常规信息")]
        INFO = 8,//消息 Information to be able to keep track of state changes etc.
        [Description("警告信息")]
        WARNING = 7,//警告 Something did not go as we expected, but it's no problem.
        [Description("错误信息")]
        ERROR = 6,//错误 Something that should not fail failed, but we can still keep on going.
        [Description("必须显示的信息")]
        MUST = 5,//系统必须输出的信息
        [Description("致命错误信息")]
        FATAL = 4,//Something failed, and we cannot handle it properly.

        /// <summary>
        /// 红色信息
        /// </summary>
        COLORREAD=100,

        /// <summary>
        /// 绿色信息
        /// </summary>
        COLORBLUE=101,

        /// <summary>
        /// 黄色信息
        /// </summary>
        COLORYELLOW=102,

        /// <summary>
        /// 灰色
        /// </summary>
        COLORGREY=103,

    }
}
