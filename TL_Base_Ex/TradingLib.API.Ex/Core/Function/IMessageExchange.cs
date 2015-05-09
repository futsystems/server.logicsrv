﻿using System;
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
        //void Send(IPacket packet);

        /// <summary>
        /// 获得某个交易帐户的Client对象
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        //IEnumerable<ClientInfoBase> GetNotifyTargets(string account);


        void ManualInsertOrder(Order o);

        void ManualInsertTrade(Trade t);
    }
}