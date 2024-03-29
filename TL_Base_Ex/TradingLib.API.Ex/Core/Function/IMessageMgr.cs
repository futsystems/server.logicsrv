﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;


namespace TradingLib.API
{
    public interface IMgrExchange
    {
        /// <summary>
        /// 将IPacket放到缓存 进行发送
        /// </summary>
        /// <param name="packet"></param>
        void Send(IPacket packet,bool fireSend);

        /// <summary>
        /// 获得某个通知列表
        /// </summary>
        /// <param name="predictate"></param>
        /// <returns></returns>
        IEnumerable<ILocation> GetNotifyTargets(Predicate<Manager> predictate);

        /// <summary>
        /// 向某个Manger列表发送一个通知
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmdstr"></param>
        /// <param name="obj"></param>
        /// <param name="predictate"></param>
        void Notify(string module, string cmdstr, object obj, Predicate<Manager> predictate);


        int OnLineTerminalNum { get; }
    }
}
