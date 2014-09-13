using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public enum QSEnumPacketType
    { 
        /// <summary>
        /// 未知
        /// </summary>
        UNKNOWN=0,
        /// <summary>
        /// 请求
        /// </summary>
        REQUEST=1,
        /// <summary>
        /// 应答类返回
        /// </summary>
        RSPRESPONSE=2,
        /// <summary>
        /// 通知类返回 对交易帐户进行通知
        /// 交易客户端通过Account寻找对应客户端
        /// 管理客户端通过Account寻找有权限的管理客户端
        /// </summary>
        NOTIFYRESPONSE=3,

        /// <summary>
        /// 设定通知类型为定向地址通知
        /// </summary>
        LOCATIONNOTIFYRESPONSE=4,
    }
    /// <summary>
    /// 通讯消息Message用于系统底层通讯
    /// Package基于Message构成了具体消息的逻辑结构
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        /// 对应的逻辑包类别
        /// </summary>
        QSEnumPacketType PacketType { get; }

        /// <summary>
        /// 前置ID
        /// </summary>
        string FrontID { get;}

        /// <summary>
        /// 请求数据包客户端Client
        /// </summary>
        string ClientID { get;}
        /// <summary>
        /// 获得消息
        /// </summary>
        byte[] Data { get;}

        /// <summary>
        /// 对应的消息类型
        /// </summary>
        MessageTypes Type { get;}

        /// <summary>
        /// 对应的消息内容
        /// </summary>
        string Content { get;}

        /// <summary>
        /// 序列化字符串到对象
        /// </summary>
        /// <param name="packetstr"></param>
        void Deserialize(string packetstr);

        /// <summary>
        /// 将对象内容序列化成字符串
        /// </summary>
        /// <returns></returns>
        string Serialize();

        /// <summary>
        /// 输出string用于打印
        /// </summary>
        /// <returns></returns>
        string ToString();
        
    }
}
