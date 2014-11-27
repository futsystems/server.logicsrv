using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;

namespace TradingLib.API
{
    /// <summary>
    /// 核心消息交换
    /// 用于接收客户端的消息进行处理并转发成相关的操作逻辑到核心与扩展模块
    /// 同时接受核心组件与扩展组件产生的消息回报转发给对应的客户端
    /// </summary>
    public interface  IMessageExchange
    {
        /// <summary>
        /// 向某个地址的客户端发送一条消息
        /// </summary>
        void Send(IPacket packet);

        /// <summary>
        /// 查询满足某个条件的客户端地址
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        string[] FilterClient(string filter);

        /// <summary>
        /// 获得某个合约当前有效价格
        /// 通过DataRouter进行获取
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        decimal GetAvabilePrice(string symbol);

        /// <summary>
        /// 获得市场快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Tick GetTickSnapshot(string symbol);


        /// <summary>
        /// 判定合约行情是否处于live状态
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        bool IsSymbolTickLive(string symbol);


        /// <summary>
        /// 获得某个Router分解委托
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        Order SentRouterOrder(long val);
    }
}
