using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;


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

        IConnectionState IState { get; }

        /// <summary>
        /// 是否是XLProtocol协议
        /// 如果是该协议类别 逻辑部分在MQServer中进行统一处理
        /// Service只负责处理协议转换部分
        /// </summary>
        bool IsXLProtocol { get; }

        /// <summary>
        /// 关闭会话
        /// </summary>
        void Close();

        /// <summary>
        /// 连接状态
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// 应答XLPacketData
        /// </summary>
        /// <param name="data"></param>
        void ResponseXLPacket(XLPacketData data,uint requestID,bool isLast);

        /// <summary>
        /// 通知XLPacketData
        /// </summary>
        /// <param name="data"></param>
        void NotifyXLPacket(XLPacketData data);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        void Send(byte[] data);
    }
}
