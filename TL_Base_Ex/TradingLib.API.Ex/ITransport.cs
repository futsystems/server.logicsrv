using System;
using TradingLib.Common;

namespace TradingLib.API
{
    /// <summary>
    /// 底层数据传输接口
    /// </summary>
    public interface ITransport:IDisposable
    {
        /// <summary>
        /// 当前服务状态是否有效
        /// </summary>
        bool IsLive { get; }

        /// <summary>
        /// 启动服务
        /// </summary>
        void Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        void Stop();


        /// <summary>
        /// 收到客户端提交上来的消息,类别,消息体,前置,客户端地址
        /// </summary>
        event Action<IPacket, string> NewPacketEvent;

        /// <summary>
        /// 向某个客户端发送消息
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="address"></param>
        /// <param name="front"></param>
        void Send(IPacket packet, string cliendtId);

        /// <summary>
        /// 向行情分发系统发送行情数据
        /// </summary>
        /// <param name="k">Tick数据</param>
        void Publish(Tick k);
    }
}
