using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.MoniterControl
{
    /// <summary>
    /// 响应类回调特性
    /// 比如 提交查询然后服务端给返回
    /// 从而实现回调函数的自动注册与注销
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CallbackAttr:Attribute
    {

        public string Module { get; private set; }

        public string Cmd { get; private set; }

        public CallbackAttr(string module, string cmd)
        {
            this.Module = module;
            this.Cmd = cmd;
        }
    }

    /// <summary>
    /// 通知类回调特性
    /// 服务端状态发生变化 主动推送一条消息
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class NotifyAttr : Attribute
    {

        public string Module { get; private set; }

        public string Cmd { get; private set; }

        public NotifyAttr(string module, string cmd)
        {
            this.Module = module;
            this.Cmd = cmd;
        }
    }
}
