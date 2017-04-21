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
        /// 服务标识
        /// </summary>
        Providers ProviderName { get; set; }

        /// <summary>
        /// 收到客户端提交上来的消息,类别,消息体,前置,客户端地址
        /// </summary>
        event Action<Message,string,string> GotTLMessageEvent;
        /// <summary>
        /// 是否启用流控
        /// </summary>
        bool EnableTPTracker{get;set;}

        /// <summary>
        /// 消息处理线程数量
        /// </summary>
        int NumWorkers { get; set; }


        /// <summary>
        /// 通知所有前置某个消息
        /// </summary>
        /// <param name="body"></param>
        void NotifyFront(byte[] body);

        /// <summary>
        /// 向某个客户端发送消息
        /// </summary>
        /// <param name="body">消息内容</param>
        /// <param name="address">客户端地址</param>
        /// <param name="front">前置地址</param>
        void Send(byte[] body, string address, string front);
        /// <summary>
        /// 向行情分发系统发送行情数据
        /// </summary>
        /// <param name="k">Tick数据</param>
        void SendTick(Tick k);

        /// <summary>
        /// 向行情系统分发行情心跳,用于告知客户端行情连接有效[停盘时,可能行情通道没有数据发送]
        /// </summary>
        //void SendTickHeartBeat();

    }
}
