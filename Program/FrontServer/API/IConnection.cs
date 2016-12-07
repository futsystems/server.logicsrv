using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace FrontServer
{
    public interface IConnection
    {
        /// <summary>
        /// Connection所在的ServiceHost对象
        /// </summary>
        IServiceHost ServiceHost { get; }

        /// <summary>
        /// 回话编号
        /// </summary>
        string SessionID { get; }

        /// <summary>
        /// 关闭会话
        /// </summary>
        void Close();
    }
}
