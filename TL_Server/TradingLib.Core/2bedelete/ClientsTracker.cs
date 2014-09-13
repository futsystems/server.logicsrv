//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using System.Collections;
//using System.Collections.Concurrent;

//namespace TradingLib.Core
//{

//    /// <summary>
//    /// 客户端信息维护器,用于维护服务端的客户端列表，注册 注销 等相关操作
//    /// 这里clientlist集成自 ConcurrentBag 线程安全的无序集合。支持多线程操作该类的实例
//    /// </summary>
//    public class ClientsTracker
//    {
//        /// <summary>
//        /// 某个终端注册事件
//        /// </summary>
//        public event ClientParamDel ClientRegistedEvent;//客户端注册事件
//        /// <summary>
//        /// 某个终端从注销事件
//        /// </summary>
//        public event ClientParamDel ClientUnRegistedEvent;//客户端注销事件


//        /// <summary>
//        /// 客户端管数据结构
//        /// </summary>
//        private ConcurrentDictionary<string, IClientInfo> clientMap = new ConcurrentDictionary<string, IClientInfo>();

//        /// <summary>
//        /// 所有客户端连接
//        /// </summary>
//        public IClientInfo[] Clients { get { return clientMap.Values.ToArray(); } }

//        /// <summary>
//        /// 返回客户端列表总数
//        /// </summary>
//        public int NumClient
//        {
//            get { return clientMap.Count; }
//        }
//        /// <summary>
//        /// 返回客户端列表中登入的用户
//        /// </summary>
//        public int NumClientLoggedIn
//        {
//            get
//            {
//                return clientMap.Values.Where(e => e.IsLoggedIn == true).Count();
//            }

//        }
//        /// <summary>
//        /// 检查某个Client是否已经注册到服务器
//        /// </summary>
//        /// <param name="ClientID"></param>
//        /// <returns></returns>
//        public bool IsRegisted(string ClientID)
//        {
//            return clientMap.Keys.Contains(ClientID);
//        }

//        /// <summary>
//        /// 绑定某个Account的有效Client地址
//        /// </summary>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        public string[] AddListForAccount(string account)
//        {
//            IEnumerable<string> address = from client in clientMap.Values
//                                          where client.AccountID == account
//                                          select client.Address;
//            return address.ToArray();
//        }

//        /// <summary>
//        /// 绑定某个UID的有效Client地址
//        /// </summary>
//        /// <param name="uid"></param>
//        /// <returns></returns>
//        public string[] AddListForUID(int uid)
//        {
//            IEnumerable<string> address = from client in clientMap.Values
//                                          where client.UID == uid
//                                          select client.Address;
//            return address.ToArray();
//        }

//        /// <summary>
//        /// 通过ClientID返回某个已经注册的Client数据
//        /// 优化算法
//        /// </summary>
//        /// <param name="ClientID"></param>
//        /// <returns></returns>
//        public IClientInfo this[string address]
//        {
//            get
//            {
//                IClientInfo c;
//                if (clientMap.TryGetValue(address, out c))
//                    return c;
//                else
//                    return null;
//            }

//        }

//        /// <summary>
//        /// 注册某个客户端
//        /// </summary>
//        /// <param name="ClientID"></param>
//        public void RegistClient(IClientInfo nc)
//        {
//            if (IsRegisted(nc.Address)) return;

//            clientMap.TryAdd(nc.Address, nc);
//            //对外触发客户端注册到服务器事件
//            if (ClientRegistedEvent != null)
//                ClientRegistedEvent(nc);
//        }

//        /// <summary>
//        /// 注销某个客户端
//        /// </summary>
//        /// <param name="ClientID"></param>
//        public void UnRegistClient(string address)
//        {
//            List<IClientInfo> tobedelete = new List<IClientInfo>();
//            foreach (IClientInfo c in clientMap.Values)
//            {
//                if (c.Address == address)
//                {
//                    tobedelete.Add(c);
//                }

//            }
//            if (tobedelete.Count > 0)
//            {
//                //对外触发客户端注销事件
//                if (ClientUnRegistedEvent != null)
//                    ClientUnRegistedEvent(tobedelete[0]);
//            }
//            foreach (IClientInfo c in tobedelete)
//            {
//                //this.Remove(c);
//                IClientInfo o;
//                clientMap.TryRemove(c.Address, out o);
//                //_log.ClearClient(cinfo);
//            }
//        }

//        /// <summary>
//        /// 根据记录的心跳时间,删除死掉的客户端session，参数时间为临界时间,比如应该在10:05分有心跳回报，但是客户端信息中心跳时间小于10:00的统一删除
//        /// 因为客户端会按一定的时间给服务端发送心跳信息
//        /// </summary>
//        /// <param name="time"></param>
//        public void DelDeadClient(DateTime time)
//        {
//            List<IClientInfo> tobedelete = new List<IClientInfo>();
//            foreach (IClientInfo c in clientMap.Values)
//            {
//                //最近心跳时间超过 死亡期限 同时 前置类型不为Ev前置的 则需要清理死亡连接信息
//                if (c.FrontType != QSEnumFrontType.EvAccess && c.HeartBeat <= time)
//                {
//                    tobedelete.Add(c);
//                }

//            }
//            if (tobedelete.Count > 0)
//            {
//                if (ClientUnRegistedEvent != null)
//                    ClientUnRegistedEvent(tobedelete[0]);
//            }

//            foreach (IClientInfo c in tobedelete)
//            {
//                IClientInfo o;
//                clientMap.TryRemove(c.Address, out o);
//            }

//        }

//        /// <summary>
//        /// 当客户端请求登入时
//        /// 不同前置对应不同的客户端,在删除同帐户客户端时,是指在同一种类型的接入前置进行的
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="fronttype"></param>
//        /// <param name="allowmutlfront"></param>
//        public void DelClientByAccount(string account, QSEnumFrontType fronttype, bool allowmutlfront)
//        {
//            List<IClientInfo> tobedelete = new List<IClientInfo>();
//            foreach (IClientInfo c in clientMap.Values)
//            {
//                if (!allowmutlfront)//不允许多个前置登入
//                {
//                    if (c.AccountID == account)//不允许前置则交易帐号相同则就进行删除
//                    {
//                        tobedelete.Add(c);
//                    }
//                }
//                else//允许多个前置接入 交易帐号相同,前置相同 才进行删除
//                {
//                    if (c.AccountID == account && c.FrontType == fronttype)
//                    {
//                        tobedelete.Add(c);
//                    }
//                }
//            }
//            foreach (IClientInfo c in tobedelete)
//            {
//                IClientInfo o;
//                clientMap.TryRemove(c.Address, out o);
//            }
//        }

//        /// <summary>
//        /// 从某个ClientList恢复连接信息,主要是从数据库session信息中恢复Client连接信息
//        /// </summary>
//        /// <param name="clist"></param>
//        public void Restore(List<IClientInfo> clist)
//        {
//            foreach (IClientInfo c in clist)
//            {
//                clientMap.TryAdd(c.Address, c);
//            }
//        }

//        /// <summary>
//        /// 查询客户列别中从某个fontID中接入的客户端
//        /// </summary>
//        /// <param name="frontid"></param>
//        /// <returns></returns>
//        public int QryFrontConnected(string frontid)
//        {
//            return clientMap.Values.Where(e => e.FrontID.Equals(frontid)).Count();
//        }

//    }
//}
