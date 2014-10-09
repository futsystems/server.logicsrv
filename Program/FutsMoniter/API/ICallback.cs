using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface ICallbackCentre
    {
        /// <summary>
        /// 注册回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="del"></param>
        void RegisterCallback(string module, string cmd, JsonReplyDel del);


        /// <summary>
        /// 注销回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="del"></param>
        void UnRegisterCallback(string module, string cmd, JsonReplyDel del);
    }
}
