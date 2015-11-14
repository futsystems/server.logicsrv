using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.API
{
    /// <summary>
    /// TLSocket接口
    /// 用于封装不同的底层通讯方式
    /// </summary>
    public interface ITLSocket
    {
        /// <summary>
        /// 回话编号
        /// </summary>
        //string SessionID { get; set; }

        /// <summary>
        /// 是否处于连接状态
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 建立连接
        /// </summary>
        void Connect();

        /// <summary>
        /// 断开连接
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 消息事件
        /// </summary>
        event Action<MessageTypes,string> MessageEvent;
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="msg"></param>
        void Send(byte[] msg);
    }
}
