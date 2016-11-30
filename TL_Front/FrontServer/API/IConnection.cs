using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace CTPService
{
    public interface IConnection
    {
        /// <summary>
        /// 回话编号
        /// </summary>
        string SessionID { get; set; }

        /// <summary>
        /// 向客户端发送数据包
        /// 逻辑服务器返回的数据包转换成客户端协议支持的数据包对外发送
        /// </summary>
        /// <param name="packet"></param>
        void SendToClient(IPacket packet);

        /// <summary>
        /// 向逻辑服务器发送数据包
        /// 客户端提交上来的请求转换成内部数据格式 向逻辑服务器发送
        /// </summary>
        /// <param name="packet"></param>
        void ForwardToLogic(IPacket packet);

        /// <summary>
        /// 关闭会话
        /// </summary>
        void Close();
    }
}
