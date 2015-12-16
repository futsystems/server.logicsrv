using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.DataFarm.API
{
    /// <summary>
    /// Connection接口 用于实现客户端连接的维护与管理
    /// 由ServiceHost负责创建
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Connection所处ServiceHost
        /// </summary>
        IServiceHost ServiceHost { get; set; }

        /// <summary>
        /// 回话编号
        /// </summary>
        string SessionID { get; set; }

        /// <summary>
        /// 登入ID
        /// </summary>
        string LoginID { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        string IPAddress { get; set; }

        /// <summary>
        /// 最近的客户端心跳
        /// </summary>
        DateTime LastHeartBeat { get; set; }

        /// <summary>
        /// 向Connection发送消息
        /// </summary>
        /// <param name="packet"></param>
        void Send(IPacket packet);

    }
}
