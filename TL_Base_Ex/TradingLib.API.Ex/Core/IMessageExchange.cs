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
        /// <param name="msg">消息体</param>
        /// <param name="type">消息类别</param>
        /// <param name="address">客户端唯一地址</param>
        //void Send(string msg, MessageTypes type, string address);
        void Send(IPacket packet);
        /// <summary>
        /// 查询满足某个条件的客户端地址
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        string[] FilterClient(string filter);


        decimal GetAvabilePrice(string symbol);
        /// <summary>
        /// 获得帐户比赛信息
        /// </summary>
        //event GetRaceInfoDel GetRaceInfoEvent;

        /// <summary>
        /// 通过第三方进行帐户认证
        /// </summary>
        //event LoginRequestDel<T> AuthUserEvent;

        /// <summary>
        /// 对外发送某个帐户的最新比赛状态
        /// </summary>
        /// <param name="acc"></param>
        //void newRaceInfo(IAccount acc);
    }
}
