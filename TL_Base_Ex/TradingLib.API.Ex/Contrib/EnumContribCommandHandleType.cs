using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 扩展命令处理类别
    /// 1.响应消息源消息
    /// 2.绑定到其他扩展模块事件
    /// </summary>
    public enum QSContribCommandHandleType
    {
        MessageHandler,
        EventHandler,
    }
}
