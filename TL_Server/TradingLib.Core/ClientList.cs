using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Collections;
using System.Collections.Concurrent;

namespace TradingLib.Core
{

    /// <summary>
    /// 客户端信息维护器,用于维护服务端的客户端列表，注册 注销 等相关操作
    /// 这里clientlist集成自 ConcurrentBag 线程安全的无序集合。支持多线程操作该类的实例
    /// </summary>
    public class ClientList :IClientList
    {
        public event ClientParamDel ClientRegistedEvent;//客户端注册事件
        public event ClientParamDel ClientUnRegistedEvent;//客户端注销事件

        private ConcurrentDictionary<string, IClientInfo> clientMap = new ConcurrentDictionary<string, IClientInfo>();
        public IClientInfo[] Clients { get { return clientMap.Values.ToArray(); } }
        /// <summary>
        /// 返回客户端列表总数
        /// </summary>
        public int NumClient
        {
            get { return clientMap.Count; }
        }
        /// <summary>
        /// 返回客户端列表中登入的用户
        /// </summary>
        public int NumClientLoggedIn
        {
            get
            {
                int r = 0;
                foreach (IClientInfo c in clientMap.Values)
                {
                    if (c.IsLoggedIn)
                        r++;
                }
                return r;
            }

        }
        /// <summary>
        /// 检查某个Client是否已经注册到服务器
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public bool IsRegisted(string ClientID)
        {
            return clientMap.Keys.Contains(ClientID);
            /*
            foreach (IClientInfo c in clientMap.Values)
            {
                if (c.ClientID == ClientID)
                    return true;
            }
            return false;**/
        }
        /// <summary>
        /// 通过交易账号返回对应的客户端ClientID
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string LoginID2ClientID(string loginId)
        {
            foreach (IClientInfo c in clientMap.Values)
            {
                if (c.LoginID == loginId)
                    return c.ClientID;
            }
            return null;
        }
        /// <summary>
        /// 通过ClientID返回某个已经注册的Client数据
        /// 优化算法
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public IClientInfo this[string ClientID]
        {
            get
            {
                IClientInfo c;
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
        public void RegistClient(IClientInfo nc)
        {
            if (IsRegisted(nc.ClientID)) return;

            clientMap.TryAdd(nc.ClientID, nc);
            //对外触发客户端注册到服务器事件
            if (ClientRegistedEvent != null)
                ClientRegistedEvent(nc);
        }
        /// <summary>
        /// 注销某个客户端
        /// </summary>
        /// <param name="ClientID"></param>
        public void UnRegistClient(string ClientID)
        {
            List<IClientInfo> tobedelete = new List<IClientInfo>();
            foreach (IClientInfo c in clientMap.Values)
            {
                if (c.ClientID == ClientID)
                {
                    tobedelete.Add(c);
                }

            }
            if (tobedelete.Count > 0)
            {
                if (ClientUnRegistedEvent != null)
                    ClientUnRegistedEvent(tobedelete[0]);
            }
            foreach (IClientInfo c in tobedelete)
            {
                //this.Remove(c);
                IClientInfo o;
                clientMap.TryRemove(c.ClientID, out o);
                //_log.ClearClient(cinfo);
            }


        }
        /// <summary>
        /// 根据记录的心跳时间,删除死掉的客户端session，参数时间为临界时间,比如应该在10:05分有心跳回报，但是客户端信息中心跳时间小于10:00的统一删除
        /// 因为客户端会按一定的时间给服务端发送心跳信息
        /// </summary>
        /// <param name="time"></param>
        public void DelDeadClient(DateTime time)
        {
            List<IClientInfo> tobedelete = new List<IClientInfo>();
            foreach (IClientInfo c in clientMap.Values)
            {
                if (c.HeartBeat <= time)
                {
                    tobedelete.Add(c);
                }

            }
            if (tobedelete.Count > 0)
            {
                if (ClientUnRegistedEvent != null)
                    ClientUnRegistedEvent(tobedelete[0]);
            }

            foreach (IClientInfo c in tobedelete)
            {
                IClientInfo o;
                clientMap.TryRemove(c.ClientID, out o);
                //UnRegistClient(c.ClientID)
            }

        }
        /// <summary>
        /// 当客户端请求登入时,为了确保唯一登入,我们将删除以该loginID登入的其他所有登入客户端信息
        /// </summary>
        /// <param name="account"></param>
        public void DelClientByLoginID(string loginid)
        {
            List<IClientInfo> tobedelete = new List<IClientInfo>();
            foreach (IClientInfo c in clientMap.Values)
            {
                if (c.LoginID == loginid)
                {
                    tobedelete.Add(c);
                }

            }
            foreach (IClientInfo c in tobedelete)
            {
                IClientInfo o;
                clientMap.TryRemove(c.ClientID, out o);

            }

        }
        /// <summary>
        /// 从某个ClientList恢复连接信息,主要是从数据库session信息中恢复Client连接信息
        /// </summary>
        /// <param name="clist"></param>
        public void Restore(List<IClientInfo> clist)
        {
            foreach (IClientInfo c in clist)
            {
                clientMap.TryAdd(c.ClientID, c);
            }
        }

        /// <summary>
        /// 查询客户列别中从某个fontID中接入的客户端
        /// </summary>
        /// <param name="frontid"></param>
        /// <returns></returns>
        public int QryFrontConnected(string frontid)
        {
            IEnumerable<IClientInfo> clients =
            from client in clientMap.Values
                where client.FrontID ==frontid
            select client;

            return clients.Count();
        }

    }
}
