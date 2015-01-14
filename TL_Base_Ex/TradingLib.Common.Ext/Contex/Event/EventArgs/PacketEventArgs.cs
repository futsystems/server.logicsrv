using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 某个回话的某个消息包
    /// 交易客户端和管理端 通过向服务端发送消息包实现相关操作与请求
    /// 1.标识回话
    /// 2.定义内容
    /// </summary>
    public class PacketEventArgs : EventArgs
    {
        public PacketEventArgs(ISession session, IPacket packet,string frontid,string clientid)
        {
            this.Session = session;
            this.Packet = packet;
            this.FrontID = frontid;
            this.ClientID = clientid;
        }
        /// <summary>
        /// 回话Session
        /// </summary>
        public ISession Session { get; set; }

        /// <summary>
        /// 管理端发出的消息包
        /// 通过消息包可以获得消息类型和消息内容
        /// </summary>
        public IPacket Packet { get; set; }

        /// <summary>
        /// 前置
        /// </summary>
        public string FrontID { get; set; }

        /// <summary>
        /// 客户端地址
        /// </summary>
        public string ClientID { get; set; }
    }
}
