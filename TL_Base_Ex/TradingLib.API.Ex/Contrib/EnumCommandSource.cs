using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 命令源
    /// 交易客户端的命令
    /// Web管理端的命令
    /// 命令行终端的命令
    /// 都是在得到消息时,调用对应的命令执行
    /// </summary>
    public enum QSEnumCommandSource
    {
        /// <summary>
        /// 交易消息交换,来自于客户端消息
        /// </summary>
        MessageExchange,
        /// <summary>
        /// 管理消息交换,来自于管理端消息
        /// </summary>
        MessageMgr,
        /// <summary>
        /// web端消息交换,来自于web端消息
        /// </summary>
        MessageWeb,
        /// <summary>
        /// 命令行,该类命令用于响应命令行的交互输入
        /// </summary>
        CLI,
    }
}
