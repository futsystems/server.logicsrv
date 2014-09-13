using System;

namespace TradingLib.API
{
    public interface ITransport:IService,IDebug,IDisposable
    {
        /// <summary>
        /// 服务标识
        /// </summary>
        Providers ProviderName { get; set; }
        /// <summary>
        /// 收到客户端提交上来的消息,类别,消息体,前置,客户端地址
        /// </summary>
        event HandleTLMessageDel GotTLMessageEvent;
        /// <summary>
        /// 是否启用流控
        /// </summary>
        bool EnableTPTracker{get;set;}
        /// <summary>
        /// 消息处理线程数量
        /// </summary>
        int NumWorkers { get; set; }
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
        void SendTickHeartBeat();

    }
}
