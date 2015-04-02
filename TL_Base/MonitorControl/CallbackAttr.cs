using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.GUI
{
    /// <summary>
    /// 用于对函数进行标注
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
}
