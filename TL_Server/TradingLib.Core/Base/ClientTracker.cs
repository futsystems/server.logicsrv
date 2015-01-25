using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Collections;
using System.Collections.Concurrent;
using TradingLib.Common;

namespace TradingLib.Core
{

    public class ClientTracker<T>
        where T : ClientInfoBase, new()
    {
        /// <summary>
        /// 某个终端注册事件
        /// </summary>
        public event ClientInfoDelegate<T> ClientRegistedEvent;//客户端注册事件
        /// <summary>
        /// 某个终端从注销事件
        /// </summary>
        public event ClientInfoDelegate<T> ClientUnregistedEvent;//客户端注销事件

        

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
        /// 优化算法
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public T this[string ClientID]
        {
            get
            {
                T c;
                if (clientMap.TryGetValue(ClientID, out c))
                    return c;
                else
                    return null;
            }
        }

        /// <summary>
        /// 注册某个客户端
        /// </summary>
        /// <param name="ClientID"></param>
        public void RegistClient(T info)
        {
            if (IsRegisted(info.Location.ClientID)) return;

            clientMap.TryAdd(info.Location.ClientID, info);
            //对外触发客户端注册到服务器事件
            if (ClientRegistedEvent != null)
                ClientRegistedEvent(info);
        }

        /// <summary>
        /// 注销某个客户端
        /// </summary>
        /// <param name="ClientID"></param>
        public void UnRegistClient(string ClientID)
        {
            T tobedelete=null;
            clientMap.TryRemove(ClientID, out tobedelete);
            if (tobedelete != null)
            {
                if (ClientUnregistedEvent != null)
                    ClientUnregistedEvent(tobedelete);
            }
        }

        /// <summary>
        /// 根据记录的心跳时间,删除死掉的客户端session，参数时间为临界时间,比如应该在10:05分有心跳回报，但是客户端信息中心跳时间小于10:00的统一删除
        /// 因为客户端会按一定的时间给服务端发送心跳信息
        /// </summary>
        /// <param name="time"></param>
        public void DropDeadClient(DateTime deadtime)
        {
            IEnumerable<T> clients = clientMap.Values.Where(e=>e.FrontType != QSEnumFrontType.EVAccess && e.FrontType != QSEnumFrontType.WebAccess && e.HeartBeat<=deadtime);

            foreach (T c in clients)
            {
                UnRegistClient(c.Location.ClientID);
            }
        }


        /// <summary>
        /// 从某个ClientList恢复连接信息,主要是从数据库session信息中恢复Client连接信息
        /// </summary>
        /// <param name="clist"></param>
        public void Restore(List<T> clist)
        {
            foreach (T c in clist)
            {
                clientMap.TryAdd(c.Location.ClientID, c);
            }
        }

        /// <summary>
        /// 查询客户列别中从某个fontID中接入的客户端
        /// </summary>
        /// <param name="frontid"></param>
        /// <returns></returns>
        public int QryFrontConnected(string frontid)
        {
            IEnumerable<T> clients =
            from client in clientMap.Values
            where client.Location.FrontID == frontid
            select client;

            return clients.Count();
        }




    }
}
