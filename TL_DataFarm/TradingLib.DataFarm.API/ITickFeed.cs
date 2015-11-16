using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.DataFarm.API
{
    /// <summary>
    /// 实时行情Feed接口
    /// </summary>
    public interface ITickFeed
    {
        string Name { get; }
        /// <summary>
        /// 行情到达事件
        /// </summary>
        event Action<ITickFeed,Tick> TickEvent;

        /// <summary>
        /// 连接建立事件
        /// </summary>
        event Action<ITickFeed> ConnectEvent;

        /// <summary>
        /// 连接断开事件
        /// </summary>
        event Action<ITickFeed> DisconnectEvent;

        /// <summary>
        /// 启动
        /// </summary>
        void Start();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }
}