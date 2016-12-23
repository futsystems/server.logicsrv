//Copyright 2013 by FutSystems,Inc.
//20161223 整理函数RegistClient与UnRegistClient


using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 客户端维护器
    /// 用于维护连接到服务端的客户端列表
    /// 客户端可以直连也可以通过前置进行连接
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClientTracker<T>
        where T : ClientInfoBase, new()
    {
        /// <summary>
        /// 某个终端注册事件
        /// </summary>
        public event Action<T> ClientRegistedEvent = delegate { };

        /// <summary>
        /// 某个终端从注销事件
        /// </summary>
        public event Action<T> ClientUnregistedEvent = delegate { };

        /// <summary>
        /// 客户端信息map
        /// </summary>
        private ConcurrentDictionary<string, T> clientMap = new ConcurrentDictionary<string, T>();
        
        /// <summary>
        /// 返回所有客户端
        /// </summary>
        public IEnumerable<T> Clients { get { return clientMap.Values; } }

        /// <summary>
        /// 检查某个Client是否已经注册到服务器
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public bool IsRegisted(string ClientID)
        {
            return clientMap.Keys.Contains(ClientID);
        }

        /// <summary>
        /// 通过ClientID返回某个已经注册的Client数据
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public T this[string ClientID]
        {
            get
            {
                T target;
                if (clientMap.TryGetValue(ClientID, out target))
                {
                    return target;
                }
                return null;
            }
        }

        /// <summary>
        /// 注册某个客户端
        /// </summary>
        /// <param name="ClientID"></param>
        public void RegistClient(T info)
        {
            if (clientMap.TryAdd(info.Location.ClientID, info))
            {
                ClientRegistedEvent(info);
            }
        }

        /// <summary>
        /// 注销某个客户端
        /// </summary>
        /// <param name="ClientID"></param>
        public void UnRegistClient(string ClientID)
        {
            T target=null;
            if (clientMap.TryRemove(ClientID, out target))
            {
                ClientUnregistedEvent(target);
            }
        }

        /// <summary>
        /// 根据记录的心跳时间,删除死掉的客户端session，参数时间为临界时间,比如应该在10:05分有心跳回报，但是客户端信息中心跳时间小于10:00的统一删除
        /// </summary>
        /// <param name="time"></param>
        public void DropDeadClient(DateTime deadtime)
        {
            //处理逻辑需要结合前置进行修改 如果前置处理心跳则服务端只处理前置ID为空的直连终端
            List<T> clients = clientMap.Values.Where(client => string.IsNullOrEmpty(client.Location.FrontID) && client.HeartBeat <= deadtime).ToList();
            foreach (T c in clients)
            {
                UnRegistClient(c.Location.ClientID);
            }
        }
    }
}
